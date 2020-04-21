using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;

namespace ICD.Connect.Sources.TvTuner.Controls
{
	public abstract class AbstractTvTunerControl<TDevice> : AbstractDeviceControl<TDevice>, ITvTunerControl
		where TDevice : IDevice
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		protected AbstractTvTunerControl(TDevice parent, int id)
			: base(parent, id)
		{
		}

		#region Channels

		/// <summary>
		/// Goes to the given channel number.
		/// </summary>
		/// <param name="number"></param>
		public abstract void SetChannel(string number);

		/// <summary>
		/// Sends a single number to the tuner for manual channel selection.
		/// </summary>
		/// <param name="number"></param>
		public abstract void SendNumber(char number);

		/// <summary>
		/// Sends the enter command.
		/// </summary>
		public abstract void Enter();

		/// <summary>
		/// Sends the clear command.
		/// </summary>
		public abstract void Clear();

		/// <summary>
		/// Goes to the next channel.
		/// </summary>
		public abstract void ChannelUp();

		/// <summary>
		/// Goes to the previous channel.
		/// </summary>
		public abstract void ChannelDown();

		#endregion

		#region Playback

		/// <summary>
		/// Sends the repeat command.
		/// </summary>
		public abstract void Repeat();

		/// <summary>
		/// Sends the rewind command.
		/// </summary>
		public abstract void Rewind();

		/// <summary>
		/// Sends the fast-forward command.
		/// </summary>
		public abstract void FastForward();

		/// <summary>
		/// Sends the stop command.
		/// </summary>
		public abstract void Stop();

		/// <summary>
		/// Sends the play command.
		/// </summary>
		public abstract void Play();

		/// <summary>
		/// Sends the pause command.
		/// </summary>
		public abstract void Pause();

		/// <summary>
		/// Sends the record command.
		/// </summary>
		public abstract void Record();

		#endregion

		#region Menus

		/// <summary>
		/// Goes to the above page.
		/// </summary>
		public abstract void PageUp();

		/// <summary>
		/// Goes to the below page.
		/// </summary>
		public abstract void PageDown();

		/// <summary>
		/// Sends the top-menu command.
		/// </summary>
		public abstract void TopMenu();

		/// <summary>
		/// Sends the popup-menu command.
		/// </summary>
		public abstract void PopupMenu();

		/// <summary>
		/// Sends the return command.
		/// </summary>
		public abstract void Return();

		/// <summary>
		/// Sends the info command.
		/// </summary>
		public abstract void Info();

		/// <summary>
		/// Sends the eject command.
		/// </summary>
		public abstract void Eject();

		/// <summary>
		/// Sends the power command.
		/// </summary>
		public abstract void Power();

		/// <summary>
		/// Sends the red command.
		/// </summary>
		public abstract void Red();

		/// <summary>
		/// Sends the green command.
		/// </summary>
		public abstract void Green();

		/// <summary>
		/// Sends the yellow command.
		/// </summary>
		public abstract void Yellow();

		/// <summary>
		/// Sends the blue command.
		/// </summary>
		public abstract void Blue();

		/// <summary>
		/// Sends the up command.
		/// </summary>
		public abstract void Up();

		/// <summary>
		/// Sends the down command.
		/// </summary>
		public abstract void Down();

		/// <summary>
		/// Sends the left command.
		/// </summary>
		public abstract void Left();

		/// <summary>
		/// Sends the right command.
		/// </summary>
		public abstract void Right();

		/// <summary>
		/// Sends the select command.
		/// </summary>
		public abstract void Select();

		#endregion

		#region Console

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return new GenericConsoleCommand<string>("SetChannel", "SetChannel <Number>", c => SetChannel(c));
			yield return new GenericConsoleCommand<char>("SendNumber", "SendNumber <Digit>", c => SendNumber(c));
			yield return new ConsoleCommand("Enter", "Sends the enter command", () => Enter());
			yield return new ConsoleCommand("Clear", "Sends the clear command", () => Clear());
			yield return new ConsoleCommand("ChannelUp", "Increments the channel", () => ChannelUp());
			yield return new ConsoleCommand("ChannelDown", "Decrements the channel", () => ChannelDown());

			yield return new ConsoleCommand("Repeat", "Sends the repeat command", () => Repeat());
			yield return new ConsoleCommand("Rewind", "Sends the rewind command", () => Rewind());
			yield return new ConsoleCommand("FastForward", "Sends the fast-forward command", () => FastForward());
			yield return new ConsoleCommand("Stop", "Sends the stop command", () => Stop());
			yield return new ConsoleCommand("Play", "Sends the play command", () => Play());
			yield return new ConsoleCommand("Pause", "Sends the pause command", () => Pause());
			yield return new ConsoleCommand("Record", "Sends the record command", () => Record());

			yield return new ConsoleCommand("PageUp", "Sends the page-up command", () => PageUp());
			yield return new ConsoleCommand("PageDown", "Sends the page-up command", () => PageDown());
			yield return new ConsoleCommand("TopMenu", "Sends the top-menu command", () => TopMenu());
			yield return new ConsoleCommand("PopupMenu", "Sends the popup-menu command", () => PopupMenu());
			yield return new ConsoleCommand("Return", "Sends the return command", () => Return());
			yield return new ConsoleCommand("Info", "Sends the info command", () => Info());
			yield return new ConsoleCommand("Eject", "Sends the eject command", () => Eject());
			yield return new ConsoleCommand("Power", "Sends the power command", () => Power());
			yield return new ConsoleCommand("Red", "Sends the red command", () => Red());
			yield return new ConsoleCommand("Green", "Sends the green command", () => Green());
			yield return new ConsoleCommand("Yellow", "Sends the yellow command", () => Yellow());
			yield return new ConsoleCommand("Blue", "Sends the blue command", () => Blue());
			yield return new ConsoleCommand("Up", "Sends the up command", () => Up());
			yield return new ConsoleCommand("Down", "Sends the down command", () => Down());
			yield return new ConsoleCommand("Left", "Sends the left command", () => Left());
			yield return new ConsoleCommand("Right", "Sends the right command", () => Right());
			yield return new ConsoleCommand("Select", "Sends the select command", () => Select());
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
