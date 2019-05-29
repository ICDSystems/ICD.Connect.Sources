using System;
using System.Collections.Generic;
using System.Net.Http;
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

namespace ICD.Connect.Sources.Roku
{
    public sealed class RokuDevice : AbstractDevice<RokuDeviceSettings>
    {
        private readonly UriProperties m_UriProperties;
        private IWebPort m_Port;

        /// <summary>
        /// Constructor.
        /// </summary>
        public RokuDevice()
        {
            m_UriProperties = new UriProperties();
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
                    port = factory.GetPortById((int)settings.Port) as IWebPort;
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

            settings.Port = m_Port == null ? (int?)null : m_Port.Id;

            settings.Copy(m_UriProperties);
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
		private void Keypress(eRokuKeys key)
		{
			string unused;
			m_Port.Post(string.Format("/keypress/{0}",key.ToString()), new byte[0], out unused);
		}
		/// <summary>
		/// Sends an HTTP POST command to press and hold a specific key
		/// </summary>
		/// <param name="key"></param>
		private void Keydown(eRokuKeys key)
		{
			string unused;
			m_Port.Post(string.Format("/keydown/{0}", key.ToString()), new byte[0], out unused);
		}
		/// <summary>
		/// Sends an HTTP POST command to release a specific key
		/// </summary>
		/// <param name="key"></param>
		private void Keyup(eRokuKeys key)
		{
			string unused;
			m_Port.Post(string.Format("/keyup/{0}", key.ToString()), new byte[0], out unused);
		}
		#endregion

		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in base.GetConsoleCommands())
				yield return command;

			string keyHelp =
				string.Format("Keys: <{0}>", StringUtils.ArrayFormat(EnumUtils.GetValues<eRokuKeys>()));

			yield return new GenericConsoleCommand<eRokuKeys>("Keypress", "Press and release key. " + keyHelp, k => Keypress(k));
			yield return new GenericConsoleCommand<eRokuKeys>("Keydown", "Press and hold key. " + keyHelp, k => Keydown(k));
			yield return new GenericConsoleCommand<eRokuKeys>("Keyup", "Release key. " + keyHelp, k => Keyup(k));
		}

		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}
    }
}
