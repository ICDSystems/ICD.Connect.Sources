using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.Devices;

namespace ICD.Connect.Sources.TvTuner.Devices
{
	public abstract class AbstractTvTunerDevice<TSettings> : AbstractDevice<TSettings>, ITvTuner
		where TSettings : ITvTunerSettings, new()
	{
		/// <summary>
		/// Goes to the given channel number.
		/// </summary>
		/// <param name="number"></param>
		public abstract void SetChannel(string number);

		/// <summary>
		/// Goes to the next channel.
		/// </summary>
		public abstract void ChannelUp();

		/// <summary>
		/// Goes to the previous channel.
		/// </summary>
		public abstract void ChannelDown();

		/// <summary>
		/// Sends a single number to the tuner for manual channel selection.
		/// </summary>
		/// <param name="number"></param>
		public abstract void SendNumber(char number);

		#region Console

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return new ConsoleCommand("ChannelUp", "Increments the channel", () => ChannelUp());
			yield return new ConsoleCommand("ChannelDown", "Decrements the channel", () => ChannelDown());
			yield return new GenericConsoleCommand<string>("SetChannel", "SetChannel <Number>", c => SetChannel(c));
			yield return new GenericConsoleCommand<char>("SendNumber", "SendNumber <Digit>", c => SendNumber(c));
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
