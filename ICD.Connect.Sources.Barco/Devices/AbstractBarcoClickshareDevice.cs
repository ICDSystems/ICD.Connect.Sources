using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Logging.Activities;
using ICD.Common.Logging.LoggingContexts;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Comparers;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Devices.Telemetry;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.Ports.Web;
using ICD.Connect.Protocol.Network.Settings;
using ICD.Connect.Settings;
using ICD.Connect.Sources.Barco.API;
using ICD.Connect.Sources.Barco.Devices.Controls;
using ICD.Connect.Sources.Barco.Responses.Common;
using ICD.Connect.Telemetry.Attributes;

namespace ICD.Connect.Sources.Barco.Devices
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
		private const int INFO_UPDATE_OCCURRENCE = 10;

		private const string PORT_ACCEPT = "application/json";

		// ReSharper disable once StaticFieldInGenericType
		private static readonly Dictionary<Version, Func<IBarcoClickshareApi>> s_ApiVersions =
			new Dictionary<Version, Func<IBarcoClickshareApi>>(new UndefinedVersionEqualityComparer())
			{
				{new Version("1.0.0.0"), () => new BarcoClickshareApiV1()},
				{new Version("2.0.0.0"), () => new BarcoClickshareApiV2()}
			};

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

		private readonly IcdHashSet<Button> m_Buttons;
		private readonly SafeCriticalSection m_ButtonsSection;

		private readonly UriProperties m_UriProperties;
		private readonly WebProxyProperties m_WebProxyProperties;

		private IBarcoClickshareApi m_Api;
		private IWebPort m_Port;

		private bool m_Sharing;
		private Version m_Version;
		private Version m_SoftwareVersion;
		private long m_SharingUpdateInterval;
		private int m_UpdateCount;
		private int m_ConsecutivePortFailures;

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
		public Version Version
		{
			get { return m_Version; }
			private set
			{
				if (value == m_Version)
					return;

				m_Version = value;

				Logger.LogSetTo(eSeverity.Informational, "Version", m_Version);

				OnVersionChanged.Raise(this, new StringEventArgs(m_Version.ToString()));
			}
		}

		/// <summary>
		/// Gets the software version running on the clickshare.
		/// </summary>
		[PublicAPI]
		[PropertyTelemetry(DeviceTelemetryNames.DEVICE_FIRMWARE_VERSION, null, DeviceTelemetryNames.DEVICE_FIRMWARE_VERSION_CHANGED)]
		public Version SoftwareVersion
		{
			get { return m_SoftwareVersion; }
			private set
			{
				if (value == m_SoftwareVersion)
					return;

				m_SoftwareVersion = value;

				Logger.LogSetTo(eSeverity.Informational, "Software Version", m_SoftwareVersion);

				OnSoftwareVersionChanged.Raise(this, new StringEventArgs(m_SoftwareVersion.ToString()));
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
				try
				{
					if (value == m_Sharing)
						return;

					m_Sharing = value;

					Logger.LogSetTo(eSeverity.Informational, "Sharing", m_Sharing);

					OnSharingStatusChanged.Raise(this, new BoolEventArgs(m_Sharing));
				}
				finally
				{
					Activities.LogActivity(m_Sharing
						                       ? new Activity(Activity.ePriority.High, "Sharing", "Sharing", eSeverity.Informational)
						                       : new Activity(Activity.ePriority.Low, "Sharing", "Not Sharing", eSeverity.Informational));
				}
			}
		}

		[PropertyTelemetry(DeviceTelemetryNames.DEVICE_DHCP_STATUS, null, DeviceTelemetryNames.DEVICE_DHCP_STATUS_CHANGED)]
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

		[PropertyTelemetry(DeviceTelemetryNames.DEVICE_IP_ADDRESS, null, DeviceTelemetryNames.DEVICE_IP_ADDRESS_CHANGED)]
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

		[PropertyTelemetry(DeviceTelemetryNames.DEVICE_IP_SUBNET, null, DeviceTelemetryNames.DEVICE_IP_SUBNET_CHANGED)]
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

		[PropertyTelemetry(DeviceTelemetryNames.DEVICE_IP_GATEWAY, null, DeviceTelemetryNames.DEVICE_IP_GATEWAY_CHANGED)]
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

		[PropertyTelemetry(DeviceTelemetryNames.DEVICE_HOSTNAME, null, DeviceTelemetryNames.DEVICE_HOSTNAME_CHANGED)]
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

		[PropertyTelemetry(DeviceTelemetryNames.DEVICE_IP_ADDRESS_SECONDARY, null, DeviceTelemetryNames.DEVICE_IP_ADDRESS_SECONDARY_CHANGED)]
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

		[PropertyTelemetry(DeviceTelemetryNames.DEVICE_MAC_ADDRESS_SECONDARY, null, DeviceTelemetryNames.DEVICE_MAC_ADDRESS_SECONDARY_CHANGED)]
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
			m_UriProperties = new UriProperties();
			m_WebProxyProperties = new WebProxyProperties();

			m_SharingUpdateInterval = SHARING_UPDATE_INTERVAL;

			m_Buttons = new IcdHashSet<Button>();
			m_ButtonsSection = new SafeCriticalSection();

			m_SharingTimer = SafeTimer.Stopped(SharingTimerCallback);
			m_SharingTimerSection = new SafeCriticalSection();

			// Initialize activities
			Sharing = false;
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

			ConfigurePort(port);

			Unsubscribe(m_Port);
			m_Port = port;
			Subscribe(m_Port);

			if (m_Port != null)
				m_Port.Accept = PORT_ACCEPT;

			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Configures the given port for communication with the device.
		/// </summary>
		/// <param name="port"></param>
		private void ConfigurePort(IWebPort port)
		{
			// URI
			if (port != null)
			{
				port.ApplyDeviceConfiguration(m_UriProperties);
				port.ApplyDeviceConfiguration(m_WebProxyProperties);
			}
		}

		/// <summary>
		/// Gets the cached buttons.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Button> GetButtons()
		{
			return m_ButtonsSection.Execute(() => m_Buttons.OrderBy(p => p.Id).ToArray());
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
				Sharing = m_Api.GetSharingState(m_Port);

				// If the sharing state changed or we've updated enough times, check the version and buttons table.
				if (Sharing != oldSharing || m_UpdateCount == 0)
				{
					Version = m_Api.GetVersion(m_Port);
					SoftwareVersion = m_Api.GetSoftwareVersion(m_Port);
					UpdateButtons(m_Api.GetButtonsTable(m_Port));
					Model = m_Api.GetModel(m_Port);
					SerialNumber = m_Api.GetSerialNumber(m_Port);
					UpdateLanInfo(m_Api.GetLan(m_Port));
					UpdateWlanInfo(m_Api.GetWlan(m_Port));
				}

				m_UpdateCount = (m_UpdateCount + 1) % INFO_UPDATE_OCCURRENCE;
			}
			catch (Exception e)
			{
				Logger.Log(eSeverity.Error, "Error communicating with {0} - {1}", m_Port.Uri, e.Message);
				IncrementUpdateInterval();
				return;
			}

			ResetUpdateInterval();
		}

		/// <summary>
		/// Updates the buttons from json.
		/// </summary>
		/// <param name="buttons"></param>
		private void UpdateButtons(IEnumerable<Button> buttons)
		{
			bool changed;

			m_ButtonsSection.Enter();

			try
			{
				IcdHashSet<Button> newButtons = buttons.ToIcdHashSet();

				changed = !newButtons.SetEquals(m_Buttons);

				if (changed)
				{
					m_Buttons.Clear();
					m_Buttons.AddRange(newButtons);
				}
			}
			finally
			{
				m_ButtonsSection.Leave();
			}

			if (changed)
				OnButtonsChanged.Raise(this);
		}

		private void UpdateLanInfo(LanInfo info)
		{
			LanDhcpEnabled = string.Equals(info.Addressing, "DHCP", StringComparison.OrdinalIgnoreCase);
			LanIpAddress = info.IpAddress;
			LanSubnetMask = info.SubnetMask;
			LanGateway = info.DefaultGateway;
			LanHostname = info.Hostname;
		}

		private void UpdateWlanInfo(WlanInfo info)
		{
			WlanIpAddress = info.IpAddress;
			WlanMacAddress = info.MacAddress;
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

			settings.Copy(m_UriProperties);
			settings.Copy(m_WebProxyProperties);
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			SetPort(null);

			m_UriProperties.ClearUriProperties();
			m_WebProxyProperties.ClearProxyProperties();
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(T settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			m_UriProperties.Copy(settings);
			m_WebProxyProperties.Copy(settings);

			IWebPort port = null;

			if (settings.Port != null)
			{
				try
				{
					port = factory.GetPortById((int)settings.Port) as IWebPort;
				}
				catch (KeyNotFoundException)
				{
					Logger.Log(eSeverity.Error, "No Web Port with id {0}", settings.Port);
				}	
			}
			SetPort(port);

			try
			{
				m_Api = ApiFactory(settings.ApiVersion);
			}
			catch (Exception e)
			{
				Logger.Log(eSeverity.Error, "No API for version {0}", settings.ApiVersion);
				Logger.Log(eSeverity.Error, "API factory failed - {0}", e.Message);
			}
		}

		private static IBarcoClickshareApi ApiFactory(string versionString)
		{
			Version version = new Version(versionString);
			Func<IBarcoClickshareApi> factory;
			if (!s_ApiVersions.TryGetValue(version, out factory))
				throw new Exception("Unable to determine clickshare API version");

			return factory();
		}

		/// <summary>
		/// Override to add controls to the device.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		/// <param name="addControl"></param>
		protected override void AddControls(T settings, IDeviceFactory factory, Action<IDeviceControl> addControl)
		{
			base.AddControls(settings, factory, addControl);

			addControl(new BarcoClickshareRouteSourceControl<AbstractBarcoClickshareDevice<T>, T>(this, 0));
		}

		/// <summary>
		/// Override to add actions on StartSettings
		/// This should be used to start communications with devices and perform initial actions
		/// </summary>
		protected override void StartSettingsFinal()
		{
			base.StartSettingsFinal();

			// Start the clickshare polling timer
			ResetUpdateInterval();
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
