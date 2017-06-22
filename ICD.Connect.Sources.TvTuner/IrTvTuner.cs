using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Services.Logging;
using ICD.Connect.Devices;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Mock.Source;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Sources.TvTuner
{
	/// <summary>
	/// Controls a generic TvTuner via an IR Port.
	/// </summary>
	public sealed class IrTvTuner : AbstractDevice<IrTvTunerSettings>, ITvTuner
	{
		private IIrPort m_Port;

		/// <summary>
		/// Constructor.
		/// </summary>
		public IrTvTuner()
		{
			// Assume the tv tuner has a single source output
			MockRouteSourceControl sourceControl = new MockRouteSourceControl(this, 0);
			sourceControl.SetActiveTransmissionState(1, eConnectionType.Audio | eConnectionType.Video, true);

			Controls.Add(sourceControl);
		}

		#region Methods

		/// <summary>
		/// Sets the port.
		/// </summary>
		/// <param name="port"></param>
		[PublicAPI]
		public void SetIrPort(IIrPort port)
		{
			if (port == m_Port)
				return;

			Unsubscribe(m_Port);
			m_Port = port;
			Subscribe(m_Port);

			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Goes to the given channel number.
		/// </summary>
		/// <param name="number"></param>
		public void SetChannel(string number)
		{
			foreach (char character in number)
				SendNumber(character);
		}

		/// <summary>
		/// Goes to the next channel.
		/// </summary>
		public void ChannelUp()
		{
			SendNumber('+');
		}

		/// <summary>
		/// Goes to the previous channel.
		/// </summary>
		public void ChannelDown()
		{
			SendNumber('-');
		}

		/// <summary>
		/// Sends a single number to the tuner for manual channel selection.
		/// </summary>
		/// <param name="number"></param>
		public void SendNumber(char number)
		{
			m_Port.PressAndRelease(number.ToString());
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(IrTvTunerSettings settings)
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

			SetIrPort(null);
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(IrTvTunerSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			IIrPort port = null;

			if (settings.Port != null)
				port = factory.GetPortById((int)settings.Port) as IIrPort;
			if (port == null)
				Logger.AddEntry(eSeverity.Error, "Port {0} is not an IR Port", settings.Port);

			SetIrPort(port);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribe to the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Subscribe(IIrPort port)
		{
			if (port == null)
				return;

			port.OnIsOnlineStateChanged += PortOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Unsubscribe(IIrPort port)
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
		private void PortOnIsOnlineStateChanged(object sender, BoolEventArgs args)
		{
			UpdateCachedOnlineStatus();
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
	}
}
