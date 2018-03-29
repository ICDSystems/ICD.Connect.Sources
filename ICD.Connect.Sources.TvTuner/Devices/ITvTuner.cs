using ICD.Common.Properties;
using ICD.Connect.API.Attributes;
using ICD.Connect.Devices;
using ICD.Connect.Sources.TvTuner.Proxies;

namespace ICD.Connect.Sources.TvTuner.Devices
{
	/// <summary>
	/// A TV Tuner device provides functionality for changing channel.
	/// </summary>
	[ApiClass(typeof(ProxyTvTuner), typeof(IDevice))]
	public interface ITvTuner : IDevice
	{
		#region Channels

		/// <summary>
		/// Goes to the given channel number.
		/// </summary>
		/// <param name="number"></param>
		[ApiMethod(TvTunerApi.METHOD_SET_CHANNEL, TvTunerApi.HELP_METHOD_SET_CHANNEL)]
		void SetChannel(string number);

		/// <summary>
		/// Sends a single number to the tuner for manual channel selection.
		/// </summary>
		/// <param name="number"></param>
		[ApiMethod(TvTunerApi.METHOD_SEND_NUMBER, TvTunerApi.HELP_METHOD_SEND_NUMBER)]
		void SendNumber(char number);

		/// <summary>
		/// Sends the clear command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_CLEAR, TvTunerApi.HELP_METHOD_CLEAR)]
		void Clear();

		/// <summary>
		/// Sends the enter command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_ENTER, TvTunerApi.HELP_METHOD_ENTER)]
		void Enter();

		/// <summary>
		/// Goes to the next channel.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_CHANNEL_UP, TvTunerApi.HELP_METHOD_CHANNEL_UP)]
		void ChannelUp();

		/// <summary>
		/// Goes to the previous channel.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_CHANNEL_DOWN, TvTunerApi.HELP_METHOD_CHANNEL_DOWN)]
		void ChannelDown();

		#endregion

		#region Playback

		/// <summary>
		/// Sends the repeat command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_REPEAT, TvTunerApi.HELP_METHOD_REPEAT)]
		void Repeat();

		/// <summary>
		/// Sends the rewind command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_REWIND, TvTunerApi.HELP_METHOD_REWIND)]
		void Rewind();

		/// <summary>
		/// Sends the fast-forward command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_FAST_FORWARD, TvTunerApi.HELP_METHOD_FAST_FORWARD)]
		void FastForward();

		/// <summary>
		/// Sends the stop command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_STOP, TvTunerApi.HELP_METHOD_STOP)]
		void Stop();

		/// <summary>
		/// Sends the play command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_PLAY, TvTunerApi.HELP_METHOD_PLAY)]
		void Play();

		/// <summary>
		/// Sends the pause command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_PAUSE, TvTunerApi.HELP_METHOD_PAUSE)]
		void Pause();

		/// <summary>
		/// Sends the record command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_RECORD, TvTunerApi.HELP_METHOD_RECORD)]
		void Record();

		#endregion

		#region Menus

		/// <summary>
		/// Goes to the above page.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_PAGE_UP, TvTunerApi.HELP_METHOD_PAGE_UP)]
		void PageUp();

		/// <summary>
		/// Goes to the below page.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_PAGE_DOWN, TvTunerApi.HELP_METHOD_PAGE_DOWN)]
		void PageDown();

		/// <summary>
		/// Sends the top-menu command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_TOP_MENU, TvTunerApi.HELP_METHOD_TOP_MENU)]
		void TopMenu();

		/// <summary>
		/// Sends the popup-menu command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_POPUP_MENU, TvTunerApi.HELP_METHOD_POPUP_MENU)]
		void PopupMenu();

		/// <summary>
		/// Sends the return command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_RETURN, TvTunerApi.HELP_METHOD_RETURN)]
		void Return();

		/// <summary>
		/// Sends the info command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_INFO, TvTunerApi.HELP_METHOD_INFO)]
		void Info();

		/// <summary>
		/// Sends the eject command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_EJECT, TvTunerApi.HELP_METHOD_EJECT)]
		void Eject();

		/// <summary>
		/// Sends the power command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_POWER, TvTunerApi.HELP_METHOD_POWER)]
		void Power();

		/// <summary>
		/// Sends the red command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_RED, TvTunerApi.HELP_METHOD_RED)]
		void Red();

		/// <summary>
		/// Sends the green command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_GREEN, TvTunerApi.HELP_METHOD_GREEN)]
		void Green();

		/// <summary>
		/// Sends the yellow command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_YELLOW, TvTunerApi.HELP_METHOD_YELLOW)]
		void Yellow();

		/// <summary>
		/// Sends the blue command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_BLUE, TvTunerApi.HELP_METHOD_BLUE)]
		void Blue();

		/// <summary>
		/// Sends the up command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_UP, TvTunerApi.HELP_METHOD_UP)]
		void Up();

		/// <summary>
		/// Sends the down command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_DOWN, TvTunerApi.HELP_METHOD_DOWN)]
		void Down();

		/// <summary>
		/// Sends the left command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_LEFT, TvTunerApi.HELP_METHOD_LEFT)]
		void Left();

		/// <summary>
		/// Sends the right command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_RIGHT, TvTunerApi.HELP_METHOD_RIGHT)]
		void Right();

		/// <summary>
		/// Sends the select command.
		/// </summary>
		[ApiMethod(TvTunerApi.METHOD_SELECT, TvTunerApi.HELP_METHOD_SELECT)]
		void Select();

		#endregion
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
