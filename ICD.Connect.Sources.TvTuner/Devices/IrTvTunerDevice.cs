using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Ports.IrPort;
using ICD.Connect.Protocol.Settings;
using ICD.Connect.Routing.Mock.Source;
using ICD.Connect.Settings;
using ICD.Connect.Sources.TvTuner.Controls;

namespace ICD.Connect.Sources.TvTuner.Devices
{
	/// <summary>
	/// Controls a generic TvTuner via an IR Port.
	/// </summary>
	public sealed class IrTvTunerDevice : AbstractTvTunerDevice<IrTvTunerSettings>
	{
		private readonly IrTvTunerCommands m_Commands;
		private readonly IrDriverProperties m_IrDriverProperties;
		
		private IIrPort m_Port;

		/// <summary>
		/// Constructor.
		/// </summary>
		public IrTvTunerDevice()
		{
			m_Commands = new IrTvTunerCommands();
			m_IrDriverProperties = new IrDriverProperties();
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

			ConfigurePort(port);

			Unsubscribe(m_Port);
			m_Port = port;
			Subscribe(m_Port);

			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Configures the given port for communication with the device.
		/// </summary>
		/// <param name="port"></param>
		private void ConfigurePort(IIrPort port)
		{
			// IR
			if (port != null)
				port.ApplyDeviceConfiguration(m_IrDriverProperties);
		}

		#endregion

		#region Channels

		/// <summary>
		/// Goes to the given channel number.
		/// </summary>
		/// <param name="number"></param>
		public override void SetChannel(string number)
		{
			foreach (char character in number)
				PressAndRelease(character);
		}

		/// <summary>
		/// Sends a single number to the tuner for manual channel selection.
		/// </summary>
		/// <param name="number"></param>
		public override void SendNumber(char number)
		{
			PressAndRelease(number);
		}

		/// <summary>
		/// Sends the clear command.
		/// </summary>
		public override void Clear()
		{
			PressAndRelease(m_Commands.CommandClear);
		}

		/// <summary>
		/// Sends the enter command.
		/// </summary>
		public override void Enter()
		{
			PressAndRelease(m_Commands.CommandEnter);
		}

		/// <summary>
		/// Goes to the next channel.
		/// </summary>
		public override void ChannelUp()
		{
			PressAndRelease(m_Commands.CommandChannelUp);
		}

		/// <summary>
		/// Goes to the previous channel.
		/// </summary>
		public override void ChannelDown()
		{
			PressAndRelease(m_Commands.CommandChannelDown);
		}

		#endregion

		#region Playback

		/// <summary>
		/// Sends the repeat command.
		/// </summary>
		public override void Repeat()
		{
			PressAndRelease(m_Commands.CommandRepeat);
		}

		/// <summary>
		/// Sends the rewind command.
		/// </summary>
		public override void Rewind()
		{
			PressAndRelease(m_Commands.CommandRewind);
		}

		/// <summary>
		/// Sends the fast-forward command.
		/// </summary>
		public override void FastForward()
		{
			PressAndRelease(m_Commands.CommandFastForward);
		}

		/// <summary>
		/// Sends the stop command.
		/// </summary>
		public override void Stop()
		{
			PressAndRelease(m_Commands.CommandStop);
		}

		/// <summary>
		/// Sends the play command.
		/// </summary>
		public override void Play()
		{
			PressAndRelease(m_Commands.CommandPlay);
		}

		/// <summary>
		/// Sends the pause command.
		/// </summary>
		public override void Pause()
		{
			PressAndRelease(m_Commands.CommandPause);
		}

		/// <summary>
		/// Sends the record command.
		/// </summary>
		public override void Record()
		{
			PressAndRelease(m_Commands.CommandRecord);
		}

		#endregion

		#region Menus

		/// <summary>
		/// Goes to the above page.
		/// </summary>
		public override void PageUp()
		{
			PressAndRelease(m_Commands.CommandPageUp);
		}

		/// <summary>
		/// Goes to the below page.
		/// </summary>
		public override void PageDown()
		{
			PressAndRelease(m_Commands.CommandPageDown);
		}

		/// <summary>
		/// Sends the top-menu command.
		/// </summary>
		public override void TopMenu()
		{
			PressAndRelease(m_Commands.CommandTopMenu);
		}

		/// <summary>
		/// Sends the popup-menu command.
		/// </summary>
		public override void PopupMenu()
		{
			PressAndRelease(m_Commands.CommandPopupMenu);
		}

		/// <summary>
		/// Sends the return command.
		/// </summary>
		public override void Return()
		{
			PressAndRelease(m_Commands.CommandReturn);
		}

		/// <summary>
		/// Sends the info command.
		/// </summary>
		public override void Info()
		{
			PressAndRelease(m_Commands.CommandInfo);
		}

		/// <summary>
		/// Sends the eject command.
		/// </summary>
		public override void Eject()
		{
			PressAndRelease(m_Commands.CommandEject);
		}

		/// <summary>
		/// Sends the power command.
		/// </summary>
		public override void Power()
		{
			PressAndRelease(m_Commands.CommandPower);
		}

		/// <summary>
		/// Sends the red command.
		/// </summary>
		public override void Red()
		{
			PressAndRelease(m_Commands.CommandRed);
		}

		/// <summary>
		/// Sends the green command.
		/// </summary>
		public override void Green()
		{
			PressAndRelease(m_Commands.CommandGreen);
		}

		/// <summary>
		/// Sends the yellow command.
		/// </summary>
		public override void Yellow()
		{
			PressAndRelease(m_Commands.CommandYellow);
		}

		/// <summary>
		/// Sends the blue command.
		/// </summary>
		public override void Blue()
		{
			PressAndRelease(m_Commands.CommandBlue);
		}

		/// <summary>
		/// Sends the up command.
		/// </summary>
		public override void Up()
		{
			PressAndRelease(m_Commands.CommandUp);
		}

		/// <summary>
		/// Sends the down command.
		/// </summary>
		public override void Down()
		{
			PressAndRelease(m_Commands.CommandDown);
		}

		/// <summary>
		/// Sends the left command.
		/// </summary>
		public override void Left()
		{
			PressAndRelease(m_Commands.CommandLeft);
		}

		/// <summary>
		/// Sends the right command.
		/// </summary>
		public override void Right()
		{
			PressAndRelease(m_Commands.CommandRight);
		}

		/// <summary>
		/// Sends the select command.
		/// </summary>
		public override void Select()
		{
			PressAndRelease(m_Commands.CommandSelect);
		}

		#endregion

		#region Private Methods

		private void PressAndRelease(char command)
		{
			PressAndRelease(command.ToString());
		}

		private void PressAndRelease(string command)
		{
			if (m_Port == null)
			{
				Logger.Log(eSeverity.Error, "Unable to send command - port is null.");
				return;
			}

			m_Port.PressAndRelease(command);
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
			settings.Commands.Update(m_Commands);

			settings.Copy(m_IrDriverProperties);
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			SetIrPort(null);

			m_Commands.Clear();
			m_IrDriverProperties.ClearIrProperties();
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(IrTvTunerSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			m_Commands.Update(settings.Commands);
			m_IrDriverProperties.Copy(settings);

			IIrPort port = null;

			if (settings.Port != null)
			{
				try
				{
					port = factory.GetPortById((int)settings.Port) as IIrPort;
				}
				catch (KeyNotFoundException)
				{
					Logger.Log(eSeverity.Error, "Port {0} is not an IR Port", settings.Port);
				}
			}

			SetIrPort(port);
		}

		/// <summary>
		/// Override to add controls to the device.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		/// <param name="addControl"></param>
		protected override void AddControls(IrTvTunerSettings settings, IDeviceFactory factory, Action<IDeviceControl> addControl)
		{
			base.AddControls(settings, factory, addControl);

			addControl(new MockRouteSourceControl(this, 0));
			addControl(new TvTunerControl(this, 1));
		}

		#endregion

		#region Port Callbacks

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
		private void PortOnIsOnlineStateChanged(object sender, DeviceBaseOnlineStateApiEventArgs args)
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
