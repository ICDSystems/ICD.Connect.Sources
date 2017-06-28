using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Services;
using ICD.Common.Services.Logging;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.Connect.Devices;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.WebPorts;
using ICD.Connect.Settings.Core;
using Newtonsoft.Json.Linq;

namespace ICD.Connect.Sources.Barco
{
	/// <summary>
	/// Provides functionality for managing a Barco Clickshare.
	/// </summary>
	public abstract class AbstractBarcoClickshareDevice<T> : AbstractDevice<T>
		where T : AbstractBarcoClickshareDeviceSettings, new()
	{
		// We poll the device over HTTP/HTTPS repeatedly to detect changes in version, sharing
		// state, and button table.
		//
		// These requests are expensive, so we'll do the following:
		// 1 - Checking sharing status is important, so we do that frequently.
		// 2 - Checking version and button info is less important, so we do that less frequently.
		//
		// We will also update the button table when the sharing status changes.
		// (determine which button started/stopped sharing).
		//
		// Finally, there's no point polling as frequently if a request fails, so we gradually
		// increment the timer length after each failure.
		private const long SHARING_UPDATE_INTERVAL = 1000 * 5;
		private const long FAILURE_UPDATE_INTERVAL_INCREMENT = 1000 * 10;
		private const long FAILURE_UPDATE_INTERVAL_LIMIT = 1000 * 60;

		// The number of times to check "sharing" before checking version/buttons
		private const int INFO_UPDATE_OCCURANCE = 10;

		private const int STATUS_SUCCESS = 200;
		//private const int STATUS_BAD_FORMAT = 400;
		//private const int STATUS_NOT_WRITABLE = 403;
		//private const int STATUS_BAD_PATH = 404;
		//private const int STATUS_ERROR = 500;

		private const string REQUEST_VERSION = "CurrentVersion";
		private const string KEY_BUTTONS_TABLE = "/Buttons/ButtonTable";
		private const string KEY_DEVICE_SHARING = "/DeviceInfo/Sharing";
		private const string KEY_SOFTWARE_VERSION = "/Software/FirmwareVersion";

		private const string DEFAULT_VERSION = "v1.0";

		private const string PORT_ACCEPT = "application/json";

		/// <summary>
		/// Raised when we receive a new API version.
		/// </summary>
		[PublicAPI]
		public event EventHandler<StringEventArgs> OnVersionChanged;

		/// <summary>
		/// Raised when we receive a new software version.
		/// </summary>
		[PublicAPI]
		public event EventHandler<StringEventArgs> OnSoftwareVersionChanged;

		/// <summary>
		/// Raised when the first button starts sharing, or all buttons have stopped sharing.
		/// </summary>
		[PublicAPI]
		public event EventHandler<BoolEventArgs> OnSharingStatusChanged;

		/// <summary>
		/// Raised when the buttons collection changes.
		/// </summary>
		[PublicAPI]
		public event EventHandler OnButtonsChanged;

		private readonly SafeTimer m_SharingTimer;
		private readonly SafeCriticalSection m_SharingTimerSection;

		private readonly Dictionary<int, BarcoClickshareButton> m_Buttons;
		private readonly SafeCriticalSection m_ButtonsSection;

		private IWebPort m_Port;
		private bool m_Sharing;
		private string m_Version;
		private string m_SoftwareVersion;
		private long m_SharingUpdateInterval;
		private int m_UpdateCount;

		#region Properties

		/// <summary>
		/// Gets the version of the api.
		/// </summary>
		[PublicAPI]
		public string Version
		{
			get { return m_Version; }
			private set
			{
				if (value == m_Version)
					return;

				m_Version = value;

				OnVersionChanged.Raise(this, new StringEventArgs(m_Version));
			}
		}

		/// <summary>
		/// Gets the software version running on the clickshare.
		/// </summary>
		[PublicAPI]
		public string SoftwareVersion
		{
			get { return m_SoftwareVersion; }
			private set
			{
				if (value == m_SoftwareVersion)
					return;

				m_SoftwareVersion = value;

				OnSoftwareVersionChanged.Raise(this, new StringEventArgs(m_SoftwareVersion));
			}
		}

		/// <summary>
		/// Returns true if a source is sharing.
		/// </summary>
		[PublicAPI]
		public bool Sharing
		{
			get { return m_Sharing; }
			private set
			{
				if (value == m_Sharing)
					return;

				m_Sharing = value;

				OnSharingStatusChanged.Raise(this, new BoolEventArgs(m_Sharing));
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractBarcoClickshareDevice()
		{
			m_Version = DEFAULT_VERSION;
			m_SharingUpdateInterval = SHARING_UPDATE_INTERVAL;

			m_Buttons = new Dictionary<int, BarcoClickshareButton>();
			m_ButtonsSection = new SafeCriticalSection();

			m_SharingTimer = new SafeTimer(SharingTimerCallback, m_SharingUpdateInterval, m_SharingUpdateInterval);
			m_SharingTimerSection = new SafeCriticalSection();

			Controls.Add(new BarcoClickshareRouteSourceControl<AbstractBarcoClickshareDevice<T>, T>(this, 0));
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			OnVersionChanged = null;
			OnSharingStatusChanged = null;
			OnButtonsChanged = null;

			m_SharingTimer.Dispose();

			SetPort(null);

			base.DisposeFinal(disposing);
		}

		/// <summary>
		/// Sets the port for communication with the device.
		/// </summary>
		/// <param name="port"></param>
		[PublicAPI]
		public void SetPort(IWebPort port)
		{
			if (port == m_Port)
				return;

			Unsubscribe(m_Port);
			m_Port = port;
			Subscribe(m_Port);

			if (m_Port != null)
				m_Port.Accept = PORT_ACCEPT;
		}

		/// <summary>
		/// Gets the cached buttons.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<KeyValuePair<int, BarcoClickshareButton>> GetButtons()
		{
			return m_ButtonsSection.Execute(() => m_Buttons.OrderBy(p => p.Key).ToArray());
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Called periodically to update the sharing status. This is the "heart" of the device.
		/// </summary>
		private void SharingTimerCallback()
		{
			// Still updating since last time
			if (!m_SharingTimerSection.TryEnter())
				return;

			try
			{
				PollDevice();
			}
			finally
			{
				m_SharingTimerSection.Leave();
			}
		}

		protected virtual void PollDevice()
		{
			// No port, or someone else is using it?
			if (m_Port == null || m_Port.Busy)
				return;

			try
			{
				bool oldSharing = Sharing;
				ParsePortData(m_Port.Get(Version + KEY_DEVICE_SHARING), ParseSharingState);

				// If the sharing state changed or we've updated enough times, check the version and buttons table.
				if (Sharing != oldSharing || m_UpdateCount == 0)
				{
					if (Version == DEFAULT_VERSION)
						ParsePortData(m_Port.Get(REQUEST_VERSION), ParseVersion);
					if (string.IsNullOrEmpty(SoftwareVersion))
						ParsePortData(m_Port.Get(Version + KEY_SOFTWARE_VERSION), ParseSoftwareVersion);

					ParsePortData(m_Port.Get(Version + KEY_BUTTONS_TABLE), ParseButtonsTable);
				}

				m_UpdateCount = (m_UpdateCount + 1) % INFO_UPDATE_OCCURANCE;
			}
			catch (Exception e)
			{
				Log(eSeverity.Error, "{0} error communicating with {1} - {2}", GetType().Name, m_Port.Address, e.Message);
				IncrementUpdateInterval();
			}
		}

		/// <summary>
		/// Called when data is received from the physical device.
		/// </summary>
		/// <param name="response"></param>
		/// <param name="dataCallback"></param>
		private void ParsePortData(string response, Action<JObject> dataCallback)
		{
			response = (response ?? string.Empty).Trim();
			if (string.IsNullOrEmpty(response))
				return;

			JObject json;

			try
			{
				json = JObject.Parse(response);
			}
			catch (Exception)
			{
				Log(eSeverity.Error, "Failed to parse json: {0}", response);
				IncrementUpdateInterval();
				return;
			}

			int status = (int)json.SelectToken("status");

			if (status != STATUS_SUCCESS)
			{
				string message = (string)json.SelectToken("message");
				ParseError(status, message);
				return;
			}

			JObject data = (JObject)json.SelectToken("data");
			dataCallback(data);
			ResetUpdateInterval();
		}

		/// <summary>
		/// Updates sharing status from JSON.
		/// </summary>
		/// <param name="data"></param>
		private void ParseSharingState(JObject data)
		{
			Sharing = (bool)data.SelectToken("value");
		}

		/// <summary>
		/// Updates buttons from JSON.
		/// </summary>
		/// <param name="data"></param>
		private void ParseButtonsTable(JObject data)
		{
			JObject buttons = (JObject)data.SelectToken("value");
			UpdateButtons(buttons);
		}

		/// <summary>
		/// Updates version from JSON.
		/// </summary>
		/// <param name="data"></param>
		private void ParseVersion(JObject data)
		{
			Version = (string)data.SelectToken("value");
		}

		/// <summary>
		/// Updates software version from JSON.
		/// </summary>
		/// <param name="data"></param>
		private void ParseSoftwareVersion(JObject data)
		{
			SoftwareVersion = (string)data.SelectToken("value");
		}

		/// <summary>
		/// Updates the buttons from json.
		/// </summary>
		/// <param name="buttons"></param>
		private void UpdateButtons(JObject buttons)
		{
			bool changed;

			m_ButtonsSection.Enter();

			try
			{
				Dictionary<int, BarcoClickshareButton> newButtons = new Dictionary<int, BarcoClickshareButton>();

				foreach (KeyValuePair<string, JToken> token in buttons)
				{
					int index = int.Parse(token.Key);
					BarcoClickshareButton button = BarcoClickshareButton.FromJson(token.Value);
					newButtons[index] = button;
				}

				changed = !newButtons.DictionaryEqual(m_Buttons);

				if (changed)
				{
					m_Buttons.Clear();
					m_Buttons.Update(newButtons);
				}
			}
			finally
			{
				m_ButtonsSection.Leave();
			}

			if (changed)
				OnButtonsChanged.Raise(this);
		}

		/// <summary>
		/// Logs the error response from the device.
		/// </summary>
		/// <param name="status"></param>
		/// <param name="message"></param>
		private void ParseError(int status, string message)
		{
			message = string.Format("Error Code {0} - {1}", status, message);
			Log(eSeverity.Error, message);

			IncrementUpdateInterval();
		}

		/// <summary>
		/// Resets the update interval to the initial default.
		/// </summary>
		private void ResetUpdateInterval()
		{
			m_SharingUpdateInterval = SHARING_UPDATE_INTERVAL;
			m_SharingTimer.Reset(m_SharingUpdateInterval, m_SharingUpdateInterval);
		}

		/// <summary>
		/// Increments the update interval up to the failure limit.
		/// </summary>
		private void IncrementUpdateInterval()
		{
			m_SharingUpdateInterval += FAILURE_UPDATE_INTERVAL_INCREMENT;
			if (m_SharingUpdateInterval > FAILURE_UPDATE_INTERVAL_LIMIT)
				m_SharingUpdateInterval = FAILURE_UPDATE_INTERVAL_LIMIT;

			m_SharingTimer.Reset(m_SharingUpdateInterval, m_SharingUpdateInterval);
		}

		/// <summary>
		/// Logs to logging core.
		/// </summary>
		/// <param name="severity"></param>
		/// <param name="message"></param>
		/// <param name="args"></param>
		private void Log(eSeverity severity, string message, params object[] args)
		{
			message = string.Format(message, args);
			message = string.Format("{0} - {1}", GetType().Name, message);

			ServiceProvider.GetService<ILoggerService>().AddEntry(severity, message);
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_Port != null && m_Port.IsOnline;
		}

		#endregion

		#region Port Callbacks

		/// <summary>
		/// Subscribe to the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Subscribe(IWebPort port)
		{
			if (port == null)
				return;

			port.OnIsOnlineStateChanged += PortOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Unsubscribe(IWebPort port)
		{
			if (port == null)
				return;

			port.OnIsOnlineStateChanged -= PortOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Called when the port online state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void PortOnIsOnlineStateChanged(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateCachedOnlineStatus();
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(T settings)
		{
			base.CopySettingsFinal(settings);

			settings.Port = m_Port == null ? (int?)null : m_Port.Id;
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			SetPort(null);
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(T settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			IWebPort port = null;

			if (settings.Port != null)
				port = factory.GetPortById((int)settings.Port) as IWebPort;

			if (port == null)
				Log(eSeverity.Error, "No Web Port with id {0}", settings.Port);

			SetPort(port);
		}

		#endregion
	}
}
