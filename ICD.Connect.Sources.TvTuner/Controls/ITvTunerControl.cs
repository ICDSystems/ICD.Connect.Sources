using ICD.Connect.Devices.Controls;

namespace ICD.Connect.Sources.TvTuner.Controls
{
	public interface ITvTunerControl : IDeviceControl
	{
		#region Channels

		/// <summary>
		/// Goes to the given channel number.
		/// </summary>
		/// <param name="number"></param>
		void SetChannel(string number);

		/// <summary>
		/// Sends a single number to the tuner for manual channel selection.
		/// </summary>
		/// <param name="number"></param>
		void SendNumber(char number);

		/// <summary>
		/// Sends the clear command.
		/// </summary>
		void Clear();

		/// <summary>
		/// Sends the enter command.
		/// </summary>
		void Enter();

		/// <summary>
		/// Goes to the next channel.
		/// </summary>
		void ChannelUp();

		/// <summary>
		/// Goes to the previous channel.
		/// </summary>
		void ChannelDown();

		#endregion

		#region Playback

		/// <summary>
		/// Sends the repeat command.
		/// </summary>
		void Repeat();

		/// <summary>
		/// Sends the rewind command.
		/// </summary>
		void Rewind();

		/// <summary>
		/// Sends the fast-forward command.
		/// </summary>
		void FastForward();

		/// <summary>
		/// Sends the stop command.
		/// </summary>
		void Stop();

		/// <summary>
		/// Sends the play command.
		/// </summary>
		void Play();

		/// <summary>
		/// Sends the pause command.
		/// </summary>
		void Pause();

		/// <summary>
		/// Sends the record command.
		/// </summary>
		void Record();

		#endregion

		#region Menus

		/// <summary>
		/// Goes to the above page.
		/// </summary>
		void PageUp();

		/// <summary>
		/// Goes to the below page.
		/// </summary>
		void PageDown();

		/// <summary>
		/// Sends the top-menu command.
		/// </summary>
		void TopMenu();

		/// <summary>
		/// Sends the popup-menu command.
		/// </summary>
		void PopupMenu();

		/// <summary>
		/// Sends the return command.
		/// </summary>
		void Return();

		/// <summary>
		/// Sends the info command.
		/// </summary>
		void Info();

		/// <summary>
		/// Sends the eject command.
		/// </summary>
		void Eject();

		/// <summary>
		/// Sends the power command.
		/// </summary>
		void Power();

		/// <summary>
		/// Sends the red command.
		/// </summary>
		void Red();

		/// <summary>
		/// Sends the green command.
		/// </summary>
		void Green();

		/// <summary>
		/// Sends the yellow command.
		/// </summary>
		void Yellow();

		/// <summary>
		/// Sends the blue command.
		/// </summary>
		void Blue();

		/// <summary>
		/// Sends the up command.
		/// </summary>
		void Up();

		/// <summary>
		/// Sends the down command.
		/// </summary>
		void Down();

		/// <summary>
		/// Sends the left command.
		/// </summary>
		void Left();

		/// <summary>
		/// Sends the right command.
		/// </summary>
		void Right();

		/// <summary>
		/// Sends the select command.
		/// </summary>
		void Select();

		#endregion
	}
}
