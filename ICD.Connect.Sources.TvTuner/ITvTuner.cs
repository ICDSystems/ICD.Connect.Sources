using ICD.Common.Properties;
using ICD.Connect.Devices;

namespace ICD.Connect.Sources.TvTuner
{
	/// <summary>
	/// A TV Tuner device provides functionality for changing channel.
	/// </summary>
	public interface ITvTuner : IDevice
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
		[PublicAPI]
		void SendNumber(char number);
	}

	/// <summary>
	/// Provides extension methods for ITvTuner devices.
	/// </summary>
	[PublicAPI]
	public static class TvTunerExtensions
	{
		/// <summary>
		/// Sets the tv channel.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="channel"></param>
		[PublicAPI]
		public static void SetChannel(this ITvTuner extends, int channel)
		{
			extends.SetChannel(channel.ToString());
		}
	}
}
