using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.WebPorts;
using ICD.Connect.Settings.Core;
using ICD.Connect.Telemetry.Attributes;
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
		#region Constants

		private const long SHARING_UPDATE_INTERVAL = 1000 * 5;
		private const long FAILURE_UPDATE_INTERVAL_INCREMENT = 1000 * 10;
		private const long FAILURE_UPDATE_INTERVAL_LIMIT = 1000 * 60;
		private const int MAX_PORT_FAILURES_FOR_OFFLINE = 4;

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
		private const string KEY_DEVICE_MODEL = "/DeviceInfo/ModelName";
		private const string KEY_DEVICE_SERIAL = "/DeviceInfo/SerialNumber";
		private const string KEY_LAN = "/Network/Lan";
		private const string KEY_WLAN = "/Network/Wlan";
		private const string KEY_NETWORK_ADDRESSING = "Addressing";
		private const string KEY_NETWORK_IP_ADDRESS = "IpAddress";
		private const string KEY_NETWORK_SUBNET_MASK = "SubnetMask";
		private const string KEY_NETWORK_DEFAULT_GATEWAY = "DefaultGateway";
		private const string KEY_NETWORK_HOSTNAME = "Hostname";
		private const string KEY_NETWORK_MAC_ADDRESS = "MacAddress";


		private const string DEFAULT_VERSION = "v1.0";

		private const string PORT_ACCEPT = "application/json";

		#endregion

		#region Events

		/// <summary>
		/// Raised when we receive a new API version.
		/// </summary>
		[PublicAPI]
		public event EventHandler<StringEventArgs> OnVersionChanged;

		/// <summary>
		/// Raised when we receive a new software version.
		/// </summary>
		[PublicAPI]
		[EventTelemetry(DeviceTelemetryNames.DEVICE_FIRMWARE_VERSION_CHANGED)]
		public event EventHandler<StringEventArgs> OnSoftwareVersionChanged;

		/// <summary>
		/// Raised when the first button starts sharing, or all buttons have stopped sharing.
		/// </summary>
		[PublicAPI]
		public event EventHandler<BoolEventArgs> OnSharingStatusChanged;


		[EventTelemetry(DeviceTelemetryNames.DEVICE_MODEL_CHANGED)]
		public event EventHandler<StringEventArgs> OnModelChanged;


		[EventTelemetry(DeviceTelemetryNames.DEVICE_SERIAL_NUMBER_CHANGED)]
		public event EventHandler<StringEventArgs> OnSerialNumberChanged;

		/// <summary>
		/// Raised when the buttons collection changes.
		/// </summary>
		[PublicAPI]
		public event EventHandler OnButtonsChanged;

		[EventTelemetry(DeviceTelemetryNames.DEVICE_DHCP_STATUS_CHANGED)]
		public event EventHandler<BoolEventArgs> OnLanDhcpEnabledChanged;

		[EventTelemetry(DeviceTelemetryNames.DEVICE_IP_ADDRESS_CHANGED)]
		public event EventHandler<StringEventArgs> OnLanIpAddressChanged;

		[EventTelemetry(DeviceTelemetryNames.DEVICE_IP_SUBNET_CHANGED)]
		public event EventHandler<StringEventArgs> OnLanSubnetMaskChanged;

		[EventTelemetry(DeviceTelemetryNames.DEVICE_IP_GATEWAY_CHANGED)]
		public event EventHandler<StringEventArgs> OnLanGatewayChanged;

		[EventTelemetry(DeviceTelemetryNames.DEVICE_HOSTNAME_CHANGED)]
		public event EventHandler<StringEventArgs> OnLanHostnameChanged;

		[EventTelemetry(DeviceTelemetryNames.DEVICE_IP_ADDRESS_SECONDARY_CHANGED)]
		public event EventHandler<StringEventArgs> OnWlanIpAddressChanged;

		[EventTelemetry(DeviceTelemetryNames.DEVICE_MAC_ADDRESS_SECONDARY_CHANGED)]
		public event EventHandler<StringEventArgs> OnWlanMacAddressChanged;
		#endregion

		#region Fields

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
		private int m_ConsecutivePortFailures;
		private string m_Model;
		private string m_SerialNumber;

		private bool m_LanDhcpEnabled;
		private string m_LanIpAddress;
		private string m_LanSubnetMask;
		private string m_LanGateway;
		private string m_LanHostname;

		private string m_WlanIpAddress;
		private string m_WlanMacAddress;


		#endregion

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

				Log(eSeverity.Informational, "{0} version set to {1}", this, m_Version);

				OnVersionChanged.Raise(this, new StringEventArgs(m_Version));
			}
		}

		/// <summary>
		/// Gets the software version running on the clickshare.
		/// </summary>
		[PublicAPI]
		[DynamicPropertyTelemetry(DeviceTelemetryNames.DEVICE_FIRMWARE_VERSION, DeviceTelemetryNames.DEVICE_FIRMWARE_VERSION_CHANGED)]
		public string SoftwareVersion
		{
			get { return m_SoftwareVersion; }
			private set
			{
				if (value == m_SoftwareVersion)
					return;

				m_SoftwareVersion = value;

				Log(eSeverity.Informational, "{0} software version set to {1}", this, m_SoftwareVersion);

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

				Log(eSeverity.Informational, "{0} sharing state set to {1}", this, m_Sharing);

				OnSharingStatusChanged.Raise(this, new BoolEventArgs(m_Sharing));
			}
		}

		[DynamicPropertyTelemetry(DeviceTelemetryNames.DEVICE_MODEL, DeviceTelemetryNames.DEVICE_MODEL_CHANGED)]
		public string Model
		{
			get { return m_Model; }
			private set
			{
				if (m_Model == value)
					return;

				m_Model = value;

				OnModelChanged.Raise(this, new StringEventArgs(value));
			}
		}

		[DynamicPropertyTelemetry(DeviceTelemetryNames.DEVICE_SERIAL_NUMBER, DeviceTelemetryNames.DEVICE_SERIAL_NUMBER_CHANGED)]
		public string SerialNumber
		{
			get { return m_SerialNumber;}
			private set
			{
				if (m_SerialNumber == value)
					return;

				m_SerialNumber = value;

				OnSerialNumberChanged.Raise(this, new StringEventArgs(value));
			}
		}

		[DynamicPropertyTelemetry(DeviceTelemetryNames.DEVICE_DHCP_STATUS, DeviceTelemetryNames.DEVICE_DHCP_STATUS_CHANGED)]
		public bool LanDhcpEnabled
		{
			get { return m_LanDhcpEnabled; }
			private set
			{
				if (m_LanDhcpEnabled == value)
					return;

				m_LanDhcpEnabled = value;

				OnLanDhcpEnabledChanged.Raise(this, new BoolEventArgs(value));
			}
		}

		[DynamicPropertyTelemetry(DeviceTelemetryNames.DEVICE_IP_ADDRESS, DeviceTelemetryNames.DEVICE_IP_ADDRESS_CHANGED)]
		public string LanIpAddress
		{
			get
			{
				return m_LanIpAddress;
			}
			private set
			{
				if (m_LanIpAddress == value)
					return;

				m_LanIpAddress = value;

				OnLanIpAddressChanged.Raise(this, new StringEventArgs(value));
			}
		}

		[DynamicPropertyTelemetry(DeviceTelemetryNames.DEVICE_IP_SUBNET, DeviceTelemetryNames.DEVICE_IP_SUBNET_CHANGED)]
		public string LanSubnetMask
		{
			get
			{
				return m_LanSubnetMask;
			}
			private set
			{
				if (m_LanSubnetMask == value)
					return;

				m_LanSubnetMask = value;

				OnLanSubnetMaskChanged.Raise(this, new StringEventArgs(value));
			}
		}

		[DynamicPropertyTelemetry(DeviceTelemetryNames.DEVICE_IP_GATEWAY, DeviceTelemetryNames.DEVICE_IP_GATEWAY_CHANGED)]
		public string LanGateway
		{
			get { return m_LanGateway; }
			private set
			{
				if (m_LanGateway == value)
					return;

				m_LanGateway = value;

				OnLanGatewayChanged.Raise(this, new StringEventArgs(value));
			}
		}

		[DynamicPropertyTelemetry(DeviceTelemetryNames.DEVICE_HOSTNAME, DeviceTelemetryNames.DEVICE_HOSTNAME_CHANGED)]
		public string LanHostname
		{
			get { return m_LanHostname; }
			private set
			{
				if (m_LanHostname == value)
					return;

				m_LanHostname = value;

				OnLanHostnameChanged.Raise(this, new StringEventArgs(value));
			}
		}

		[DynamicPropertyTelemetry(DeviceTelemetryNames.DEVICE_IP_ADDRESS_SECONDARY, DeviceTelemetryNames.DEVICE_IP_ADDRESS_SECONDARY_CHANGED)]
		public string WlanIpAddress
		{
			get
			{
				return m_WlanIpAddress;
			}
			private set
			{
				if (m_WlanIpAddress == value)
					return;

				m_WlanIpAddress = value;

				OnWlanIpAddressChanged.Raise(this, new StringEventArgs(value));
			}
		}

		[DynamicPropertyTelemetry(DeviceTelemetryNames.DEVICE_MAC_ADDRESS_SECONDARY, DeviceTelemetryNames.DEVICE_MAC_ADDRESS_SECONDARY_CHANGED)]
		public string WlanMacAddress
		{
			get { return m_WlanMacAddress; }
			private set
			{
				if (m_WlanMacAddress == value)
					return;

				m_WlanMacAddress = value;

				OnWlanMacAddressChanged.Raise(this, new StringEventArgs(value));
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
			OnSoftwareVersionChanged = null;
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

			UpdateCachedOnlineStatus();
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
				PollSharingState();

				// If the sharing state changed or we've updated enough times, check the version and buttons table.
				if (Sharing != oldSharing || m_UpdateCount == 0)
				{
					PollVersion();
					PollSoftwareVersion();
					PollButtonsTable();
					PollModel();
					PollSerialNumber();
					PollLan();
					PollWlan();
				}

				m_UpdateCount = (m_UpdateCount + 1) % INFO_UPDATE_OCCURANCE;
			}
			catch (Exception e)
			{
				Log(eSeverity.Error, "Error communicating with {0} - {1}", m_Port.Address, e.Message);
				IncrementUpdateInterval();
			}
		}

		private void PollSharingState()
		{
			string response;

			if (m_Port.Get(DEFAULT_VERSION + KEY_DEVICE_SHARING, out response))
				ParsePortData(response, ParseSharingState);
			else
				IncrementUpdateInterval();
		}

		private void PollVersion()
		{
			if (Version != DEFAULT_VERSION)
				return;

			string response;

			if (m_Port.Get(REQUEST_VERSION, out response))
				ParsePortData(response, ParseVersion);
			else
				IncrementUpdateInterval();
		}

		private void PollSoftwareVersion()
		{
			if (!string.IsNullOrEmpty(SoftwareVersion))
				return;

			string response;

			if (m_Port.Get(DEFAULT_VERSION + KEY_SOFTWARE_VERSION, out response))
				ParsePortData(response, ParseSoftwareVersion);
			else
				IncrementUpdateInterval();
		}

		private void PollButtonsTable()
		{
			string response;

			if (m_Port.Get(DEFAULT_VERSION + KEY_BUTTONS_TABLE, out response))
				ParsePortData(response, ParseButtonsTable);
			else
				IncrementUpdateInterval();
		}

		private void PollModel()
		{
			string response;

			if (m_Port.Get(DEFAULT_VERSION + KEY_DEVICE_MODEL, out response))
				ParsePortData(response, ParseModel);
			else
				IncrementUpdateInterval();
		}

		private void PollSerialNumber()
		{
			string response;

			if (m_Port.Get(DEFAULT_VERSION + KEY_DEVICE_SERIAL, out response))
				ParsePortData(response, ParseSerialNumber);
			else
				IncrementUpdateInterval();
		}

		private void PollLan()
		{
			string response;

			if (m_Port.Get(DEFAULT_VERSION + KEY_LAN, out response))
				ParsePortData(response, ParseLan);
			else
				IncrementUpdateInterval();
		}

		private void PollWlan()
		{
			string response;

			if (m_Port.Get(DEFAULT_VERSION + KEY_WLAN, out response))
				ParsePortData(response, ParseWlan);
			else
				IncrementUpdateInterval();
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

			try
			{
				JObject json = JObject.Parse(response);

				int status = (int)json.SelectToken("status");

				if (status != STATUS_SUCCESS)
				{
					string message = (string)json.SelectToken("message");
					ParseError(status, message);
					return;
				}

				JObject data = (JObject)json.SelectToken("data");
				dataCallback(data);
			}
			catch (Exception e)
			{
				Log(eSeverity.Error, e, "Failed to parse json - {0}", response);
				IncrementUpdateInterval();
				return;
			}

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
			Version = data.SelectToken("value").ToString();
		}

		/// <summary>
		/// Updates software version from JSON.
		/// </summary>
		/// <param name="data"></param>
		private void ParseSoftwareVersion(JObject data)
		{
			SoftwareVersion = data.SelectToken("value").ToString();
		}

		private void ParseModel(JObject data)
		{
			Model = data.SelectToken("value").ToString();
		}

		private void ParseSerialNumber(JObject data)
		{
			SerialNumber = data.SelectToken("value").ToString();
		}

		private void ParseLan(JObject data)
		{
			JToken response = data.SelectToken("value");

			LanDhcpEnabled = (response.SelectToken(KEY_NETWORK_ADDRESSING).ToString()).Equals("DHCP", StringComparison.OrdinalIgnoreCase);
			LanIpAddress = response.SelectToken(KEY_NETWORK_IP_ADDRESS).ToString();
			LanSubnetMask = response.SelectToken(KEY_NETWORK_SUBNET_MASK).ToString();
			LanGateway = response.SelectToken(KEY_NETWORK_DEFAULT_GATEWAY).ToString();
			LanHostname = response.SelectToken(KEY_NETWORK_HOSTNAME).ToString();
		}

		private void ParseWlan(JObject data)
		{
			JToken response = data.SelectToken("value");

			WlanIpAddress = response.SelectToken(KEY_NETWORK_IP_ADDRESS).ToString();
			WlanMacAddress = response.SelectToken(KEY_NETWORK_MAC_ADDRESS).ToString();
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

			m_ConsecutivePortFailures = 0;
			UpdateCachedOnlineStatus();
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

			m_ConsecutivePortFailures++;
			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Logs to logging core.
		/// </summary>
		/// <param name="severity"></param>
		/// <param name="exception"></param>
		/// <param name="message"></param>
		/// <param name="args"></param>
		private void Log(eSeverity severity, Exception exception, string message, params object[] args)
		{
			message = string.Format(message, args);
			message = string.Format("{0} - {1}", this, message);

			ServiceProvider.GetService<ILoggerService>().AddEntry(severity, exception, message);
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_Port != null && m_ConsecutivePortFailures < MAX_PORT_FAILURES_FOR_OFFLINE;
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
		/// <param name="args"></param>
		private void PortOnIsOnlineStateChanged(object sender, DeviceBaseOnlineStateApiEventArgs args)
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
			{
				try
				{
					port = factory.GetPortById((int)settings.Port) as IWebPort;
				}
				catch (KeyNotFoundException)
				{
					Log(eSeverity.Error, "No Web Port with id {0}", settings.Port);
				}	
			}

			SetPort(port);
		}

		#endregion

		#region Console

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Version", Version);
			addRow("Software Version", SoftwareVersion);
			addRow("Sharing", Sharing);
		}

		#endregion
	}
}
