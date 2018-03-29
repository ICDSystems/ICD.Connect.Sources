using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices;

namespace ICD.Connect.Sources.TvTuner.Devices
{
	public abstract class AbstractTvTunerDevice<TSettings> : AbstractDevice<TSettings>, ITvTuner
		where TSettings : ITvTunerSettings, new()
	{
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
		/// Sends the clear command.
		/// </summary>
		public abstract void Clear();

		/// <summary>
		/// Sends the enter command.
		/// </summary>
		public abstract void Enter();

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
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			TvTunerConsole.BuildConsoleStatus(this, addRow);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			foreach (IConsoleCommand command in TvTunerConsole.GetConsoleCommands(this))
				yield return command;
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			foreach (IConsoleNodeBase node in GetBaseConsoleNodes())
				yield return node;

			foreach (IConsoleNodeBase node in TvTunerConsole.GetConsoleNodes(this))
				yield return node;
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleNodeBase> GetBaseConsoleNodes()
		{
			return base.GetConsoleNodes();
		}

		#endregion
	}
}
