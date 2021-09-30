using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.Ports.Web;
using ICD.Connect.Protocol.Network.Settings;
using ICD.Connect.Routing.Mock.Source;
using ICD.Connect.Settings;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Protocol.Network.Ports.Web.WebQueues;

namespace ICD.Connect.Sources.Roku
{
	public sealed class RokuDevice : AbstractDevice<RokuDeviceSettings>
	{
		private const long APP_REFRESH_MILLISECONDS = 10 * 60 * 1000;

		private const string APPS_QUERY = "/query/apps";
		private const string ACTIVE_APP_QUERY = "/query/active-app";
		private const string DEVICE_INFORMATION_QUERY = "/query/device-info";

		#region Private Fields

		private readonly SafeCriticalSection m_Section;
		private readonly UriProperties m_UriProperties;
		private readonly List<RokuApp> m_AppList;
		private readonly SafeTimer m_AppTimer;

		private readonly WebQueue m_RequestQueue;
		private IWebPort m_Port;
		private RokuApp m_ActiveApp;
		private RokuDeviceInformation m_DeviceInformation;

		#endregion

		#region Properties

		public RokuApp ActiveApp { get { return m_Section.Execute(() => m_ActiveApp); } }

		public RokuDeviceInformation DeviceInformation { get { return m_Section.Execute(() => m_DeviceInformation); } }

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public RokuDevice()
		{
			m_Section = new SafeCriticalSection();
			m_UriProperties = new UriProperties();
			m_AppList = new List<RokuApp>();
			m_AppTimer = SafeTimer.Stopped(RefreshApps);

			m_RequestQueue = new WebQueue();
		}

		#endregion

		#region Methods

		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			m_RequestQueue.Dispose();
			m_AppTimer.Dispose();
		}

		protected override bool GetIsOnlineStatus()
		{
			return m_Port != null && m_Port.IsOnline;
		}

		public IEnumerable<RokuApp> GetRokuApps()
		{
			return m_Section.Execute(() => m_AppList.ToArray());
		}

		public string GetAppIconUrl(int appId)
		{
			IcdUriBuilder builder = new IcdUriBuilder(m_Port.Uri)
			{
				Path = string.Format("/query/icon/{0}", appId)
			};

			return builder.ToString();
		}

		#endregion

		#region API

		public enum eRokuKeys
		{
			Home,
			Rev,
			Fwd,
			Play,
			Select,
			Left,
			Right,
			Down,
			Up,
			Back,
			InstantReplay,
			Info,
			Backspace,
			Search,
			Enter
		}

		public void RefreshApps()
		{
			Get(APPS_QUERY, response => ParseApps(response.DataAsString));
		}

		public void RefreshActiveApp()
		{
			Get(ACTIVE_APP_QUERY, response => ParseActiveApp(response.DataAsString));
		}

		public void RefreshDeviceInformation()
		{
			Get(DEVICE_INFORMATION_QUERY, response => ParseDeviceInformation(response.DataAsString));
		}

		/// <summary>
		/// Sends an HTTP POST command to press and release a specific key
		/// </summary>
		/// <param name="key"></param>
		public void Keypress(eRokuKeys key)
		{
			Post(string.Format("/keypress/{0}", key), response => {});
		}

		/// <summary>
		/// Sends an HTTP POST command to press and hold a specific key
		/// </summary>
		/// <param name="key"></param>
		public void Keydown(eRokuKeys key)
		{
			Post(string.Format("/keydown/{0}", key), response => {});
		}

		/// <summary>
		/// Sends an HTTP POST command to release a specific key
		/// </summary>
		/// <param name="key"></param>
		public void Keyup(eRokuKeys key)
		{
			Post(string.Format("/keyup/{0}", key), response => {});
		}

		public void KeypressKeyboard(string message)
		{
		    foreach (char messageChar in message)
		        Post(string.Format("/keypress/Lit_{0}", messageChar), response => {});
		}

		public void LaunchApp(int appId)
		{
			Post(string.Format("/launch/{0}", appId), response => {});
		}

		public void InstallApp(int channelId)
		{
			Post(string.Format("/install/{0}", channelId), response => {});
		}

		public void RokuSearch(IEnumerable<KeyValuePair<eRokuSearchPar, string>> parameters)
		{
			string query = new RokuSearchQueryBuilder().Append(parameters).ToString();
			Post(string.Format("/search/browse{0}", query), response => {});
		}

		private void Get(string path, Action<WebPortResponse> callback)
		{
			path = Uri.EscapeUriString(path);
			IcdWebRequest request = CreateRequest(path, IcdWebRequest.eWebRequestType.Get, null, callback);
			m_RequestQueue.Enqueue(request);
		}

		private void Post(string path, Action<WebPortResponse> callback)
		{
			path = Uri.EscapeUriString(path);
			IcdWebRequest request = CreateRequest(path, IcdWebRequest.eWebRequestType.Post, new byte[0], callback);
			m_RequestQueue.Enqueue(request);
		}

		#endregion

		#region Private Methods

		private IcdWebRequest CreateRequest(string path, IcdWebRequest.eWebRequestType requestType, byte[] data, Action<WebPortResponse> callback)
        {
			return new IcdWebRequest
			{
				RelativeOrAbsoluteUri = path,
				RequestType = requestType,
				Data = data,
				Callback = callback
			};
        }

		private void ParseApps(string xml)
        {
	        m_Section.Enter();

	        try
	        {
		        IEnumerable<RokuApp> apps = RokuApp.ReadAppsFromXml(xml);

		        m_AppList.Clear();
		        m_AppList.AddRange(apps);
	        }
	        finally
	        {
		        m_Section.Leave();
	        }
		}

		private void ParseActiveApp(string xml)
        {
	        m_Section.Enter();

	        try
	        {
		        RokuApp activeApp = RokuApp.ReadActiveAppFromXml(xml);
		        m_ActiveApp = activeApp;
	        }
	        finally
	        {
		        m_Section.Leave();
	        }
		}

		private void ParseDeviceInformation(string xml)
		{
			m_Section.Enter();

			try
			{
				RokuDeviceInformation deviceInformation = RokuDeviceInformation.ReadDeviceInformationFromXml(xml);
				m_DeviceInformation = deviceInformation;
			}
			finally
			{
				m_Section.Leave();
			}
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(RokuDeviceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			m_UriProperties.Copy(settings);

			IWebPort port = null;

			if (settings.Port != null)
			{
				try
				{
					port = factory.GetPortById((int) settings.Port) as IWebPort;
				}
				catch (KeyNotFoundException)
				{
					Logger.Log(eSeverity.Error, "No web port with id {0}", settings.Port);
				}
			}

			SetPort(port);
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			SetPort(null);

			m_UriProperties.ClearUriProperties();
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(RokuDeviceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Port = m_Port == null ? (int?) null : m_Port.Id;

			settings.Copy(m_UriProperties);
		}

		/// <summary>
		/// Override to add controls to the device.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		/// <param name="addControl"></param>
		protected override void AddControls(RokuDeviceSettings settings, IDeviceFactory factory, Action<IDeviceControl> addControl)
		{
			base.AddControls(settings, factory, addControl);

			addControl(new MockRouteSourceControl(this, 0));
		}

		#endregion

		#region Port Callbacks

		/// <summary>
		/// Sets the port for communication with the service.
		/// </summary>
		/// <param name="port"></param>
		[PublicAPI]
		public void SetPort(IWebPort port)
		{
			if (port == m_Port)
				return;

			m_AppTimer.Stop();

			ConfigurePort(port);

			Unsubscribe(m_Port);

			if (port != null)
				port.Accept = "application/xml";

			m_Port = port;
			Subscribe(m_Port);

			m_RequestQueue.SetPort(m_Port);

			UpdateCachedOnlineStatus();

			if (port != null)
			{
				RefreshDeviceInformation();
				m_AppTimer.Reset(0, APP_REFRESH_MILLISECONDS);
			}
		}

		/// <summary>
		/// Configures the given port for communication with the device.
		/// </summary>
		/// <param name="port"></param>
		private void ConfigurePort(IWebPort port)
		{
			// URI
			if (port != null)
				port.ApplyDeviceConfiguration(m_UriProperties);
		}

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

		private void PortOnIsOnlineStateChanged(object sender, DeviceBaseOnlineStateApiEventArgs e)
		{
			UpdateCachedOnlineStatus();
		}

		#endregion

		#region Console

		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			string keyHelp =
				string.Format("Keys: <{0}>", StringUtils.ArrayFormat(EnumUtils.GetValues<eRokuKeys>()));
			const string searchHelp = ("search Parameters: <[Keyword, Title]>");
			string multiSearchHelp =
				string.Format("Search Parameters: <{0}>", StringUtils.ArrayFormat(EnumUtils.GetValues<eRokuSearchPar>()));

			yield return new ConsoleCommand("RefreshApps", "Rebuilds the collections of channels installed on the Roku device", () => RefreshAndPrintApps());
			yield return new ConsoleCommand("PrintApps", "Prints the collections of channels installed on the Roku device", () => PrintApps());
			yield return new ConsoleCommand("ActiveApp", "Prints the information of the active application", () => RefreshAndPrintActiveApp());
			yield return new ConsoleCommand("DeviceInfo", "Retrieves and displays information about the Roku Device", () => RefreshAndPrintDeviceInformation());
			yield return new GenericConsoleCommand<eRokuKeys>("Keypress", "Press and release key. " + keyHelp, k => Keypress(k));
			yield return new GenericConsoleCommand<eRokuKeys>("Keydown", "Press and hold key. " + keyHelp, k => Keydown(k));
			yield return new GenericConsoleCommand<eRokuKeys>("Keyup", "Release key. " + keyHelp, k => Keyup(k));
			yield return new GenericConsoleCommand<string>("Keyboard", "When there is an on-screen keyboard types a message", k => KeypressKeyboard(k));
			yield return new GenericConsoleCommand<int>("Launch", "Launches the channel identified by AppID", T => LaunchApp(T));
			yield return new GenericConsoleCommand<int>("Install",
				"Exits the current channel, and launches the Channel Store details screen of the channel identified by AppID",
				T => InstallApp(T));
			yield return new GenericConsoleCommand<eRokuSearchPar, string>("Search",
				"Searches with the parameter and your term. " + searchHelp, (k, t) => RokuSearch(k, t));
			yield return
				new GenericConsoleCommand<eRokuSearchPar, string, eRokuSearchPar, string>(
					"MultiSearch", "Searches with two parameters, and 1 term per parameter (keyword or title must be a parameter). " + multiSearchHelp, (k, t, h, q) => RokuSearch(k, t, h, q));
		}

		private string RefreshAndPrintApps()
		{
			RefreshApps();
			return PrintApps();
		}

		private string RefreshAndPrintActiveApp()
		{
			RefreshActiveApp();
			return PrintActiveApp();
		}

		private string RefreshAndPrintDeviceInformation()
		{
			RefreshDeviceInformation();
			return PrintDeviceInformation();
		}

		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		private void RokuSearch(eRokuSearchPar par, string term)
		{
			KeyValuePair<eRokuSearchPar, string>[] query =
			{
				new KeyValuePair<eRokuSearchPar, string>(par, term)
			};

			RokuSearch(query);
		}

		private void RokuSearch(eRokuSearchPar par, string term, eRokuSearchPar secondPar, string secondTerm)
		{
			KeyValuePair<eRokuSearchPar, string>[] query =
			{
				new KeyValuePair<eRokuSearchPar, string>(par, term),
				new KeyValuePair<eRokuSearchPar, string>(secondPar, secondTerm)
			};

			RokuSearch(query);
		}

		private string PrintApps()
		{
			m_Section.Enter();

			try
			{
				TableBuilder builder = new TableBuilder("Name", "AppID", "Type", "SubType", "Version");

				foreach (RokuApp app in m_AppList)
					builder.AddRow(app.Name, app.AppId, app.Type, app.SubType, app.Version);

				return builder.ToString();
			}
			finally
			{
				m_Section.Leave();	
			}
		}

		private string PrintActiveApp()
		{
			m_Section.Enter();

			try
			{
				TableBuilder builder = new TableBuilder("Name", "AppID", "Type", "SubType", "Version");

				if (m_ActiveApp != null)
					builder.AddRow(m_ActiveApp.Name, m_ActiveApp.AppId, m_ActiveApp.Type, m_ActiveApp.SubType, m_ActiveApp.Version);

				return builder.ToString();
			}
			finally
			{
				m_Section.Leave();
			}
		}

		private string PrintDeviceInformation()
		{
			m_Section.Enter();

			try
			{
				TableBuilder builder = new TableBuilder("Type of Info", "details");
				if (m_DeviceInformation != null)
				{
					builder.AddRow("UDN", m_DeviceInformation.Udn);
					builder.AddRow("Serial Number", m_DeviceInformation.SerialNumber);
					builder.AddRow("Device ID", m_DeviceInformation.DeviceId);
					builder.AddRow("Advertising ID", m_DeviceInformation.AdvertisingId);
					builder.AddRow("Vendor Name", m_DeviceInformation.VendorName);
					builder.AddRow("Model Name", m_DeviceInformation.ModelName);
					builder.AddRow("Model Number", m_DeviceInformation.ModelNumber);
					builder.AddRow("Model Region", m_DeviceInformation.ModelRegion);
					builder.AddRow("Is Tv", m_DeviceInformation.IsTv);
					builder.AddRow("Is Stick", m_DeviceInformation.IsStick);
					builder.AddRow("Supports Ethernet", m_DeviceInformation.SupportsEthernet);
					builder.AddRow("Wifi MAC", m_DeviceInformation.WifiMac);
					builder.AddRow("Wifi Driver", m_DeviceInformation.WifiDriver);
					builder.AddRow("Software Version", m_DeviceInformation.SoftwareVersion);
					builder.AddRow("Network Name", m_DeviceInformation.NetworkName);
					builder.AddRow("Friendly Device Name", m_DeviceInformation.FriendlyDeviceName);
					builder.AddRow("Friendly Model Name", m_DeviceInformation.FriendlyModelName);
					builder.AddRow("Default Device Name", m_DeviceInformation.DefaultDeviceName);
					builder.AddRow("User Device Name", m_DeviceInformation.UserDeviceName);
					builder.AddRow("Build Number", m_DeviceInformation.BuildNumber);
					builder.AddRow("Software Version", m_DeviceInformation.SoftwareVersion);
					builder.AddRow("Software Build", m_DeviceInformation.SoftwareBuild);
					builder.AddRow("Secure Device", m_DeviceInformation.SecureDevice);
					builder.AddRow("Language", m_DeviceInformation.Language);
					builder.AddRow("Country", m_DeviceInformation.Country);
					builder.AddRow("Locale", m_DeviceInformation.Locale);
					builder.AddRow("Time Zone", m_DeviceInformation.TimeZone);
					builder.AddRow("Time zone Name", m_DeviceInformation.TimeZoneName);
					builder.AddRow("Time Zone Tz", m_DeviceInformation.TimeZoneTz);
					builder.AddRow("Time Zone Offset", m_DeviceInformation.TimeZoneOffset);
					builder.AddRow("Clock Format", m_DeviceInformation.ClockFormat);
					builder.AddRow("Up Time", m_DeviceInformation.UpTime);
					builder.AddRow("Power Mode", m_DeviceInformation.PowerMode);
					builder.AddRow("Supports Suspend", m_DeviceInformation.SupportsSuspend);
					builder.AddRow("Supports Find Remote", m_DeviceInformation.SupportsFindRemote);
					builder.AddRow("Supports Audio Guide", m_DeviceInformation.SupportsAudioGuide);
					builder.AddRow("Supports RVA", m_DeviceInformation.SupportsRva);
					builder.AddRow("Developer Enabled", m_DeviceInformation.DeveloperEnabled);
					builder.AddRow("Keyed Developer ID", m_DeviceInformation.KeyedDelevoperId);
					builder.AddRow("Search Enabled", m_DeviceInformation.SearchEnabled);
					builder.AddRow("Search Channels Enabled", m_DeviceInformation.SearchChannelsEnabled);
					builder.AddRow("Voice Search Enabled", m_DeviceInformation.VoiceSearchEnabled);
					builder.AddRow("Notifications Enabled", m_DeviceInformation.NotificationsEnabled);
					builder.AddRow("Notifications First Use", m_DeviceInformation.NotificationsFirstUse);
					builder.AddRow("Supports Private Listening", m_DeviceInformation.SupportsPrivateListening);
					builder.AddRow("Headphones Connected", m_DeviceInformation.HeadphonesConnected);
					builder.AddRow("Supports ECS Textedit", m_DeviceInformation.SupportsEcsTextedit);
					builder.AddRow("Supports ECS Microphone", m_DeviceInformation.SupportsEcsMicrophone);
					builder.AddRow("Supports Wake On WLAN", m_DeviceInformation.SupportsWakeOnWlan);
					builder.AddRow("Has Play On Roku", m_DeviceInformation.HasPlayOnRoku);
					builder.AddRow("Has Mobile Screensaver", m_DeviceInformation.HasMobileScreensaver);
					builder.AddRow("Support URL", m_DeviceInformation.SupportUrl);
					builder.AddRow("Grandcentral Version", m_DeviceInformation.GrandcentralVersion);
					builder.AddRow("Davinci Version", m_DeviceInformation.DavinciVersion);
				}
				return builder.ToString();
			}
			finally
			{
				m_Section.Leave();
			}
		}
		#endregion
	}
}