using System;
using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Sources.TvTuner.Devices
{
	public static class TvTunerConsole
	{
		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IEnumerable<IConsoleNodeBase> GetConsoleNodes(ITvTuner instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			yield break;
		}

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="addRow"></param>
		public static void BuildConsoleStatus(ITvTuner instance, AddStatusRowDelegate addRow)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IEnumerable<IConsoleCommand> GetConsoleCommands(ITvTuner instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			yield return new GenericConsoleCommand<string>("SetChannel", "SetChannel <Number>", c => instance.SetChannel(c));
			yield return new GenericConsoleCommand<char>("SendNumber", "SendNumber <Digit>", c => instance.SendNumber(c));
			yield return new ConsoleCommand("Enter", "Sends the enter command", () => instance.Enter());
			yield return new ConsoleCommand("Clear", "Sends the clear command", () => instance.Clear());
			yield return new ConsoleCommand("ChannelUp", "Increments the channel", () => instance.ChannelUp());
			yield return new ConsoleCommand("ChannelDown", "Decrements the channel", () => instance.ChannelDown());

			yield return new ConsoleCommand("Repeat", "Sends the repeat command", () => instance.Repeat());
			yield return new ConsoleCommand("Rewind", "Sends the rewind command", () => instance.Rewind());
			yield return new ConsoleCommand("FastForward", "Sends the fast-forward command", () => instance.FastForward());
			yield return new ConsoleCommand("Stop", "Sends the stop command", () => instance.Stop());
			yield return new ConsoleCommand("Play", "Sends the play command", () => instance.Play());
			yield return new ConsoleCommand("Pause", "Sends the pause command", () => instance.Pause());
			yield return new ConsoleCommand("Record", "Sends the record command", () => instance.Record());

			yield return new ConsoleCommand("PageUp", "Sends the page-up command", () => instance.PageUp());
			yield return new ConsoleCommand("PageDown", "Sends the page-up command", () => instance.PageDown());
			yield return new ConsoleCommand("TopMenu", "Sends the top-menu command", () => instance.TopMenu());
			yield return new ConsoleCommand("PopupMenu", "Sends the popup-menu command", () => instance.PopupMenu());
			yield return new ConsoleCommand("Return", "Sends the return command", () => instance.Return());
			yield return new ConsoleCommand("Info", "Sends the info command", () => instance.Info());
			yield return new ConsoleCommand("Eject", "Sends the eject command", () => instance.Eject());
			yield return new ConsoleCommand("Power", "Sends the power command", () => instance.Power());
			yield return new ConsoleCommand("Red", "Sends the red command", () => instance.Red());
			yield return new ConsoleCommand("Green", "Sends the green command", () => instance.Green());
			yield return new ConsoleCommand("Yellow", "Sends the yellow command", () => instance.Yellow());
			yield return new ConsoleCommand("Blue", "Sends the blue command", () => instance.Blue());
			yield return new ConsoleCommand("Up", "Sends the up command", () => instance.Up());
			yield return new ConsoleCommand("Down", "Sends the down command", () => instance.Down());
			yield return new ConsoleCommand("Left", "Sends the left command", () => instance.Left());
			yield return new ConsoleCommand("Right", "Sends the right command", () => instance.Right());
			yield return new ConsoleCommand("Select", "Sends the select command", () => instance.Select());
		}
	}
}
