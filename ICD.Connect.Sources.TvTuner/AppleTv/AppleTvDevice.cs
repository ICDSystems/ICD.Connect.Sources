using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Ports.IrPort;
using ICD.Connect.Protocol.Settings;
using ICD.Connect.Routing.Mock.Source;
using ICD.Connect.Settings;

namespace ICD.Connect.Sources.TvTuner.AppleTv
{
	public sealed class AppleTvDevice : AbstractDevice<AppleTvDeviceSettings>, IAppleTvDevice
	{
		#region Private Members

		private readonly AppleTvIrCommands m_Commands;
		private readonly IrDriverProperties m_IrDriverProperties;

		private IIrPort m_Port;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public AppleTvDevice()
		{
			m_Commands = new AppleTvIrCommands();
			m_IrDriverProperties = new IrDriverProperties();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Override to add controls to the device.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		/// <param name="addControl"></param>
		protected override void AddControls(AppleTvDeviceSettings settings, IDeviceFactory factory, Action<IDeviceControl> addControl)
		{
			base.AddControls(settings, factory, addControl);

			addControl(new MockRouteSourceControl(this, 0));
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_Port != null && m_Port.IsOnline;
		}

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

		#endregion

		#region Private Methods

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

		/// <summary>
		/// Sends a press and release of the given command to the IR port.
		/// </summary>
		/// <param name="command"></param>
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

		#region Commands

		/// <summary>
		/// Sends the UP command.
		/// </summary>
		public void Up()
		{
			PressAndRelease(m_Commands.CommandUp);
		}

		/// <summary>
		/// Sends the DOWN command.
		/// </summary>
		public void Down()
		{
			PressAndRelease(m_Commands.CommandDown);
		}

		/// <summary>
		/// Sends the LEFT command.
		/// </summary>
		public void Left()
		{
			PressAndRelease(m_Commands.CommandLeft);
		}

		/// <summary>
		/// Sends the RIGHT command.
		/// </summary>
		public void Right()
		{
			PressAndRelease(m_Commands.CommandRight);
		}

		/// <summary>
		/// Sends the SELECT command.
		/// </summary>
		public void Select()
		{
			PressAndRelease(m_Commands.CommandSelect);
		}

		/// <summary>
		/// Sends the MENU command.
		/// </summary>
		public void Menu()
		{
			PressAndRelease(m_Commands.CommandMenu);
		}

		/// <summary>
		/// Sends the PLAY/PAUSE toggle command.
		/// </summary>
		public void PlayPauseToggle()
		{
			PressAndRelease(m_Commands.CommandPlayPauseToggle);
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

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(AppleTvDeviceSettings settings, IDeviceFactory factory)
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
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(AppleTvDeviceSettings settings)
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

		#endregion

		#region Console

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("IrPort", m_Port);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return new ConsoleCommand("Up", "Sends an 'UP' press & release through the IR port", () => Up());
			yield return new ConsoleCommand("Down", "Sends a 'DOWN' press & release through the IR port", () => Down());
			yield return new ConsoleCommand("Left", "Sends a 'LEFT' press & release through the IR port", () => Left());
			yield return new ConsoleCommand("Right", "Sends a 'RIGHT' press & release through the IR port", () => Right());
			yield return new ConsoleCommand("Select", "Sends a 'SELECT' press & release through the IR port", () => Select());
			yield return new ConsoleCommand("Menu", "Sends a 'MENU' press & release through the IR port", () => Menu());
			yield return new ConsoleCommand("PPT", "Sends a 'PLAY/PAUSE' press & release through the IR port", () => PlayPauseToggle());
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}