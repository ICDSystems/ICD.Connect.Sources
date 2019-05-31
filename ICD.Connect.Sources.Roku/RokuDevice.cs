using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.Devices;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.Ports.Web;
using ICD.Connect.Protocol.Network.Settings;
using ICD.Connect.Settings;
using System.Collections.Generic;

namespace ICD.Connect.Sources.Roku
{
	public sealed class RokuDevice : AbstractDevice<RokuDeviceSettings>
	{
		private readonly UriProperties m_UriProperties;
		private readonly List<RokuApp> m_AppList;

		private IWebPort m_Port;

		/// <summary>
		/// Constructor.
		/// </summary>
		public RokuDevice()
		{
			m_UriProperties = new UriProperties();
			m_AppList = new List<RokuApp>();
		}

		/// <summary>
		/// Sets the port for communication with the service.
		/// </summary>
		/// <param name="port"></param>
		[PublicAPI]
		public void SetPort(IWebPort port)
		{
			if (port == m_Port)
				return;

			ConfigurePort(port);

			Unsubscribe(m_Port);

			if (port != null)
				port.Accept = "application/xml";

			m_Port = port;
			Subscribe(m_Port);

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
				port.ApplyDeviceConfiguration(m_UriProperties);
		}

		protected override bool GetIsOnlineStatus()
		{
			return m_Port != null && m_Port.IsOnline;
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
					Log(eSeverity.Error, "No web port with id {0}", settings.Port);
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

			m_UriProperties.Clear();
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

		#endregion

		#region GET Methods

		public void RefreshApps()
		{
			string xml;
			m_Port.Get("/query/apps", out xml);
			IEnumerable<RokuApp> apps = RokuApp.ReadAppsFromXml(xml);

			m_AppList.Clear();
			m_AppList.AddRange(apps);
		}

		private void Get(string path)
		{
			path = Uri.EscapeUriString(path);

			string unused;
			m_Port.Get(path, out unused);
		}
		#endregion

		#region Keypress Methods

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

		/// <summary>
		/// Sends an HTTP POST command to press and release a specific key
		/// </summary>
		/// <param name="key"></param>
		public void Keypress(eRokuKeys key)
		{
			Post(string.Format("/keypress/{0}", key.ToString()));
		}

		/// <summary>
		/// Sends an HTTP POST command to press and hold a specific key
		/// </summary>
		/// <param name="key"></param>
		public void Keydown(eRokuKeys key)
		{
			Post(string.Format("/keydown/{0}", key.ToString()));
		}

		/// <summary>
		/// Sends an HTTP POST command to release a specific key
		/// </summary>
		/// <param name="key"></param>
		public void Keyup(eRokuKeys key)
		{
			Post(string.Format("/keyup/{0}", key.ToString()));
		}

		#endregion


		public void LaunchApp(int appId)
		{
			Post(string.Format("/launch/{0}", appId.ToString()));
		}

		public void InstallApp(int channelId)
		{
			Post(string.Format("/install/{0}", channelId.ToString()));
		}

		#region Search

		public void RokuSearch(IEnumerable<KeyValuePair<eRokuSearchPar, string>> parameters)
		{
			string query = new RokuSearchQueryBuilder().Append(parameters).ToString();
			Post(string.Format("/search/browse{0}", query));
		}

		private void Post(string path)
		{
			path = Uri.EscapeUriString(path);

			string unused;
			m_Port.Post(path, new byte[0], out unused);
		}

		#endregion

		#region Console

		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			string keyHelp =
				string.Format("Keys: <{0}>", StringUtils.ArrayFormat(EnumUtils.GetValues<eRokuKeys>()));
			string searchHelp =
				("search Parameters: <[Keyword, Title]>");
			string multiSearchHelp =
				string.Format("Search Parameters: <{0}>", StringUtils.ArrayFormat(EnumUtils.GetValues<eRokuSearchPar>()));

			yield return new ConsoleCommand("RefreshApps", "Rebuilds the collections of channels installed on the Roku device", () => RefreshAndPrintApps());
			yield return new ConsoleCommand("PrintApps", "Prints the collections of channels installed on the Roku device", () => PrintApps());
			yield return new GenericConsoleCommand<eRokuKeys>("Keypress", "Press and release key. " + keyHelp, k => Keypress(k));
			yield return new GenericConsoleCommand<eRokuKeys>("Keydown", "Press and hold key. " + keyHelp, k => Keydown(k));
			yield return new GenericConsoleCommand<eRokuKeys>("Keyup", "Release key. " + keyHelp, k => Keyup(k));
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
			TableBuilder builder = new TableBuilder("Name", "AppID", "Type", "SubType", "Version");

			foreach (var App in m_AppList)
				builder.AddRow(App.Name, App.AppID, App.Type, App.SubType, App.Version);

			return builder.ToString();
		}

		#endregion
	}
}