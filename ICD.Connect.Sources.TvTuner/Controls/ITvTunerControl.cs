using ICD.Connect.Devices.Controls;

namespace ICD.Connect.Sources.TvTuner.Controls
{
	public interface ITvTunerControl : IDeviceControl
	{
		/// <summary>
		/// Goes to the given channel number.
		/// </summary>
		/// <param name="number"></param>
		void SetChannel(string number);

		/// <summary>
		/// Goes to the next channel.
		/// </summary>
		void ChannelUp();

		/// <summary>
		/// Goes to the previous channel.
		/// </summary>
		void ChannelDown();

		/// <summary>
		/// Sends a single number to the tuner for manual channel selection.
		/// </summary>
		/// <param name="number"></param>
		void SendNumber(char number);
	}
}
