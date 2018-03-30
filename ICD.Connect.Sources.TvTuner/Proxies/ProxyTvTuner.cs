using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices.Proxies.Devices;
using ICD.Connect.Sources.TvTuner.Devices;

namespace ICD.Connect.Sources.TvTuner.Proxies
{
	public sealed class ProxyTvTuner : AbstractProxyDevice, IProxyTvTuner
	{
		/// <summary>
		/// Goes to the given channel number.
		/// </summary>
		/// <param name="number"></param>
		public void SetChannel(string number)
		{
			CallMethod(TvTunerApi.METHOD_SET_CHANNEL, number);
		}

		/// <summary>
		/// Sends a single number to the tuner for manual channel selection.
		/// </summary>
		/// <param name="number"></param>
		public void SendNumber(char number)
		{
			CallMethod(TvTunerApi.METHOD_SEND_NUMBER, number);
		}

		/// <summary>
		/// Sends the clear command.
		/// </summary>
		public void Clear()
		{
			CallMethod(TvTunerApi.METHOD_CLEAR);
		}

		/// <summary>
		/// Sends the enter command.
		/// </summary>
		public void Enter()
		{
			CallMethod(TvTunerApi.METHOD_ENTER);
		}

		/// <summary>
		/// Goes to the next channel.
		/// </summary>
		public void ChannelUp()
		{
			CallMethod(TvTunerApi.METHOD_CHANNEL_UP);
		}

		/// <summary>
		/// Goes to the previous channel.
		/// </summary>
		public void ChannelDown()
		{
			CallMethod(TvTunerApi.METHOD_CHANNEL_DOWN);
		}

		/// <summary>
		/// Sends the repeat command.
		/// </summary>
		public void Repeat()
		{
			CallMethod(TvTunerApi.METHOD_REPEAT);
		}

		/// <summary>
		/// Sends the rewind command.
		/// </summary>
		public void Rewind()
		{
			CallMethod(TvTunerApi.METHOD_REWIND);
		}

		/// <summary>
		/// Sends the fast-forward command.
		/// </summary>
		public void FastForward()
		{
			CallMethod(TvTunerApi.METHOD_FAST_FORWARD);
		}

		/// <summary>
		/// Sends the stop command.
		/// </summary>
		public void Stop()
		{
			CallMethod(TvTunerApi.METHOD_STOP);
		}

		/// <summary>
		/// Sends the play command.
		/// </summary>
		public void Play()
		{
			CallMethod(TvTunerApi.METHOD_PLAY);
		}

		/// <summary>
		/// Sends the pause command.
		/// </summary>
		public void Pause()
		{
			CallMethod(TvTunerApi.METHOD_PAUSE);
		}

		/// <summary>
		/// Sends the record command.
		/// </summary>
		public void Record()
		{
			CallMethod(TvTunerApi.METHOD_RECORD);
		}

		/// <summary>
		/// Goes to the above page.
		/// </summary>
		public void PageUp()
		{
			CallMethod(TvTunerApi.METHOD_PAGE_UP);
		}

		/// <summary>
		/// Goes to the below page.
		/// </summary>
		public void PageDown()
		{
			CallMethod(TvTunerApi.METHOD_PAGE_UP);
		}

		/// <summary>
		/// Sends the top-menu command.
		/// </summary>
		public void TopMenu()
		{
			CallMethod(TvTunerApi.METHOD_TOP_MENU);
		}

		/// <summary>
		/// Sends the popup-menu command.
		/// </summary>
		public void PopupMenu()
		{
			CallMethod(TvTunerApi.METHOD_POPUP_MENU);
		}

		/// <summary>
		/// Sends the return command.
		/// </summary>
		public void Return()
		{
			CallMethod(TvTunerApi.METHOD_RETURN);
		}

		/// <summary>
		/// Sends the info command.
		/// </summary>
		public void Info()
		{
			CallMethod(TvTunerApi.METHOD_INFO);
		}

		/// <summary>
		/// Sends the eject command.
		/// </summary>
		public void Eject()
		{
			CallMethod(TvTunerApi.METHOD_EJECT);
		}

		/// <summary>
		/// Sends the power command.
		/// </summary>
		public void Power()
		{
			CallMethod(TvTunerApi.METHOD_POWER);
		}

		/// <summary>
		/// Sends the red command.
		/// </summary>
		public void Red()
		{
			CallMethod(TvTunerApi.METHOD_RED);
		}

		/// <summary>
		/// Sends the green command.
		/// </summary>
		public void Green()
		{
			CallMethod(TvTunerApi.METHOD_GREEN);
		}

		/// <summary>
		/// Sends the yellow command.
		/// </summary>
		public void Yellow()
		{
			CallMethod(TvTunerApi.METHOD_YELLOW);
		}

		/// <summary>
		/// Sends the blue command.
		/// </summary>
		public void Blue()
		{
			CallMethod(TvTunerApi.METHOD_BLUE);
		}

		/// <summary>
		/// Sends the up command.
		/// </summary>
		public void Up()
		{
			CallMethod(TvTunerApi.METHOD_UP);
		}

		/// <summary>
		/// Sends the down command.
		/// </summary>
		public void Down()
		{
			CallMethod(TvTunerApi.METHOD_DOWN);
		}

		/// <summary>
		/// Sends the left command.
		/// </summary>
		public void Left()
		{
			CallMethod(TvTunerApi.METHOD_LEFT);
		}

		/// <summary>
		/// Sends the right command.
		/// </summary>
		public void Right()
		{
			CallMethod(TvTunerApi.METHOD_RIGHT);
		}

		/// <summary>
		/// Sends the select command.
		/// </summary>
		public void Select()
		{
			CallMethod(TvTunerApi.METHOD_SELECT);
		}

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
