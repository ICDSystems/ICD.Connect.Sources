using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Mock.Source;

namespace ICD.Connect.Sources.TvTuner
{
	public sealed class MockTvTunerDevice : AbstractTvTunerDevice<MockTvTunerSettings>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public MockTvTunerDevice()
		{
			// Assume the tv tuner has a single source output
			MockRouteSourceControl sourceControl = new MockRouteSourceControl(this, 0);
			sourceControl.SetActiveTransmissionState(1, eConnectionType.Audio | eConnectionType.Video, true);

			Controls.Add(sourceControl);
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return true;
		}

		/// <summary>
		/// Goes to the given channel number.
		/// </summary>
		/// <param name="number"></param>
		public override void SetChannel(string number)
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
		/// Sends a single number to the tuner for manual channel selection.
		/// </summary>
		/// <param name="number"></param>
		public override void SendNumber(char number)
		{
		}
	}
}
