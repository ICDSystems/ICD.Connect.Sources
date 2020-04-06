using ICD.Connect.Devices.Mock;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Mock.Source;
using ICD.Connect.Settings;
using ICD.Connect.Sources.TvTuner.Controls;

namespace ICD.Connect.Sources.TvTuner.Devices
{
	public sealed class MockTvTunerDevice : AbstractTvTunerDevice<MockTvTunerSettings>, IMockDevice
	{
		private bool m_IsOnline;

		public bool DefaultOffline { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		public MockTvTunerDevice()
		{
			// Assume the tv tuner has a single source output
			MockRouteSourceControl sourceControl = new MockRouteSourceControl(this, 0);
			sourceControl.SetActiveTransmissionState(1, eConnectionType.Audio | eConnectionType.Video, true);

			Controls.Add(sourceControl);
			Controls.Add(new TvTunerControl(this, 1));
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_IsOnline;
		}

		/// <summary>
		/// Goes to the given channel number.
		/// </summary>
		/// <param name="number"></param>
		public override void SetChannel(string number)
		{
		}

		/// <summary>
		/// Sends the enter command.
		/// </summary>
		public override void Enter()
		{
		}

		/// <summary>
		/// Goes to the next channel.
		/// </summary>
		public override void ChannelUp()
		{
		}

		/// <summary>
		/// Goes to the previous channel.
		/// </summary>
		public override void ChannelDown()
		{
		}

		/// <summary>
		/// Sends the repeat command.
		/// </summary>
		public override void Repeat()
		{
		}

		/// <summary>
		/// Sends the rewind command.
		/// </summary>
		public override void Rewind()
		{
		}

		/// <summary>
		/// Sends the fast-forward command.
		/// </summary>
		public override void FastForward()
		{
		}

		/// <summary>
		/// Sends the stop command.
		/// </summary>
		public override void Stop()
		{
		}

		/// <summary>
		/// Sends the play command.
		/// </summary>
		public override void Play()
		{
		}

		/// <summary>
		/// Sends the pause command.
		/// </summary>
		public override void Pause()
		{
		}

		/// <summary>
		/// Sends the record command.
		/// </summary>
		public override void Record()
		{
		}

		/// <summary>
		/// Goes to the above page.
		/// </summary>
		public override void PageUp()
		{
		}

		/// <summary>
		/// Goes to the below page.
		/// </summary>
		public override void PageDown()
		{
		}

		/// <summary>
		/// Sends the top-menu command.
		/// </summary>
		public override void TopMenu()
		{
		}

		/// <summary>
		/// Sends the popup-menu command.
		/// </summary>
		public override void PopupMenu()
		{
		}

		/// <summary>
		/// Sends the return command.
		/// </summary>
		public override void Return()
		{
		}

		/// <summary>
		/// Sends the info command.
		/// </summary>
		public override void Info()
		{
		}

		/// <summary>
		/// Sends the eject command.
		/// </summary>
		public override void Eject()
		{
		}

		/// <summary>
		/// Sends the power command.
		/// </summary>
		public override void Power()
		{
		}

		/// <summary>
		/// Sends the red command.
		/// </summary>
		public override void Red()
		{
		}

		/// <summary>
		/// Sends the green command.
		/// </summary>
		public override void Green()
		{
		}

		/// <summary>
		/// Sends the yellow command.
		/// </summary>
		public override void Yellow()
		{
		}

		/// <summary>
		/// Sends the blue command.
		/// </summary>
		public override void Blue()
		{
		}

		/// <summary>
		/// Sends the up command.
		/// </summary>
		public override void Up()
		{
		}

		/// <summary>
		/// Sends the down command.
		/// </summary>
		public override void Down()
		{
		}

		/// <summary>
		/// Sends the left command.
		/// </summary>
		public override void Left()
		{
		}

		/// <summary>
		/// Sends the right command.
		/// </summary>
		public override void Right()
		{
		}

		/// <summary>
		/// Sends the select command.
		/// </summary>
		public override void Select()
		{
		}

		/// <summary>
		/// Sends a single number to the tuner for manual channel selection.
		/// </summary>
		/// <param name="number"></param>
		public override void SendNumber(char number)
		{
		}

		/// <summary>
		/// Sends the clear command.
		/// </summary>
		public override void Clear()
		{
		}

		public void SetIsOnlineState(bool isOnline)
		{
			m_IsOnline = isOnline;

			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(MockTvTunerSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			MockDeviceHelper.ApplySettings(this, settings);
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(MockTvTunerSettings settings)
		{
			base.CopySettingsFinal(settings);

			MockDeviceHelper.CopySettings(this, settings);
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			MockDeviceHelper.ClearSettings(this);
		}
	}
}
