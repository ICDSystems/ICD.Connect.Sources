using System;

namespace ICD.Connect.Sources.TvTuner.Devices
{
    public sealed class IrTvTunerCommands
    {
		#region Defaults

		private const string COMMAND_CLEAR = "clear";
		private const string COMMAND_ENTER = "enter";
		private const string COMMAND_CHANNEL_UP = "+";
		private const string COMMAND_CHANNEL_DOWN = "-";

		private const string COMMAND_REPEAT = "repeat";
		private const string COMMAND_REWIND = "rewind";
		private const string COMMAND_FAST_FORWARD = "fastforward";
		private const string COMMAND_STOP = "stop";
		private const string COMMAND_PLAY = "play";
		private const string COMMAND_PAUSE = "pause";
		private const string COMMAND_RECORD = "record";

		private const string COMMAND_PAGE_UP = "pageup";
		private const string COMMAND_PAGE_DOWN = "pagedown";
		private const string COMMAND_TOP_MENU = "topmenu";
		private const string COMMAND_POPUP_MENU = "popupmenu";
		private const string COMMAND_RETURN = "return";
		private const string COMMAND_INFO = "info";
		private const string COMMAND_EJECT = "eject";
		private const string COMMAND_POWER = "power";
		private const string COMMAND_RED = "red";
		private const string COMMAND_GREEN = "green";
		private const string COMMAND_YELLOW = "yellow";
		private const string COMMAND_BLUE = "blue";
		private const string COMMAND_UP = "up";
		private const string COMMAND_DOWN = "down";
		private const string COMMAND_LEFT = "left";
		private const string COMMAND_RIGHT = "right";
		private const string COMMAND_SELECT = "select";

		#endregion

		#region Backing Fields

		private string m_CommandClear;
		private string m_CommandEnter;
		private string m_CommandChannelUp;
		private string m_CommandChannelDown;

		private string m_CommandRepeat;
		private string m_CommandRewind;
		private string m_CommandFastForward;
		private string m_CommandStop;
		private string m_CommandPlay;
		private string m_CommandPause;
		private string m_CommandRecord;

		private string m_CommandPageUp;
		private string m_CommandPageDown;
		private string m_CommandTopMenu;
		private string m_CommandPopupMenu;
		private string m_CommandReturn;
		private string m_CommandInfo;
		private string m_CommandEject;
		private string m_CommandPower;
		private string m_CommandRed;
		private string m_CommandGreen;
		private string m_CommandYellow;
		private string m_CommandBlue;
		private string m_CommandUp;
		private string m_CommandDown;
		private string m_CommandLeft;
		private string m_CommandRight;
		private string m_CommandSelect;

		#endregion

		#region Properties

		public string CommandClear { get { return m_CommandClear ?? COMMAND_CLEAR; } set { m_CommandClear = value; } }
		public string CommandEnter { get { return m_CommandEnter ?? COMMAND_ENTER; } set { m_CommandEnter = value; } }
		public string CommandChannelUp { get { return m_CommandChannelUp ?? COMMAND_CHANNEL_UP; } set { m_CommandChannelUp = value; } }
		public string CommandChannelDown { get { return m_CommandChannelDown ?? COMMAND_CHANNEL_DOWN; } set { m_CommandChannelDown = value; } }

		public string CommandRepeat { get { return m_CommandRepeat ?? COMMAND_REPEAT; } set { m_CommandRepeat = value; } }
		public string CommandRewind { get { return m_CommandRewind ?? COMMAND_REWIND; } set { m_CommandRewind = value; } }
		public string CommandFastForward { get { return m_CommandFastForward ?? COMMAND_FAST_FORWARD; } set { m_CommandFastForward = value; } }
		public string CommandStop { get { return m_CommandStop ?? COMMAND_STOP; } set { m_CommandStop = value; } }
		public string CommandPlay { get { return m_CommandPlay ?? COMMAND_PLAY; } set { m_CommandPlay = value; } }
		public string CommandPause { get { return m_CommandPause ?? COMMAND_PAUSE; } set { m_CommandPause = value; } }
		public string CommandRecord { get { return m_CommandRecord ?? COMMAND_RECORD; } set { m_CommandRecord = value; } }

		public string CommandPageUp { get { return m_CommandPageUp ?? COMMAND_PAGE_UP; } set { m_CommandPageUp = value; } }
		public string CommandPageDown { get { return m_CommandPageDown ?? COMMAND_PAGE_DOWN; } set { m_CommandPageDown = value; } }
		public string CommandTopMenu { get { return m_CommandTopMenu ?? COMMAND_TOP_MENU; } set { m_CommandTopMenu = value; } }
		public string CommandPopupMenu { get { return m_CommandPopupMenu ?? COMMAND_POPUP_MENU; } set { m_CommandPopupMenu = value; } }
		public string CommandReturn { get { return m_CommandReturn ?? COMMAND_RETURN; } set { m_CommandReturn = value; } }
		public string CommandInfo { get { return m_CommandInfo ?? COMMAND_INFO; } set { m_CommandInfo = value; } }
		public string CommandEject { get { return m_CommandEject ?? COMMAND_EJECT; } set { m_CommandEject = value; } }
		public string CommandPower { get { return m_CommandPower ?? COMMAND_POWER; } set { m_CommandPower = value; } }
		public string CommandRed { get { return m_CommandRed ?? COMMAND_RED; } set { m_CommandRed = value; } }
		public string CommandGreen { get { return m_CommandGreen ?? COMMAND_GREEN; } set { m_CommandGreen = value; } }
		public string CommandYellow { get { return m_CommandYellow ?? COMMAND_YELLOW; } set { m_CommandYellow = value; } }
		public string CommandBlue { get { return m_CommandBlue ?? COMMAND_BLUE; } set { m_CommandBlue = value; } }
		public string CommandUp { get { return m_CommandUp ?? COMMAND_UP; } set { m_CommandUp = value; } }
		public string CommandDown { get { return m_CommandDown ?? COMMAND_DOWN; } set { m_CommandDown = value; } }
		public string CommandLeft { get { return m_CommandLeft ?? COMMAND_LEFT; } set { m_CommandLeft = value; } }
		public string CommandRight { get { return m_CommandRight ?? COMMAND_RIGHT; } set { m_CommandRight = value; } }
		public string CommandSelect { get { return m_CommandSelect ?? COMMAND_SELECT; } set { m_CommandSelect = value; } }

		#endregion

		#region Methods

		/// <summary>
		/// Copies the commands from the other commands instance.
		/// </summary>
		/// <param name="other"></param>
		public void Update(IrTvTunerCommands other)
		{
			if (other == null)
				throw new ArgumentNullException("other");

			CommandClear = other.CommandClear;
			CommandEnter = other.CommandEnter;
			CommandChannelDown = other.CommandChannelDown;
			CommandChannelUp = other.CommandChannelUp;

			CommandRepeat = other.CommandRepeat;
			CommandRewind = other.CommandRewind;
			CommandFastForward = other.CommandFastForward;
			CommandStop = other.CommandStop;
			CommandPlay = other.CommandPlay;
			CommandPause = other.CommandPause;
			CommandRecord = other.CommandRecord;

			CommandPageUp = other.CommandPageUp;
			CommandPageDown = other.CommandPageDown;
			CommandTopMenu = other.CommandTopMenu;
			CommandPopupMenu = other.CommandPopupMenu;
			CommandReturn = other.CommandReturn;
			CommandInfo = other.CommandInfo;
			CommandEject = other.CommandEject;
			CommandPower = other.CommandPower;
			CommandRed = other.CommandRed;
			CommandGreen = other.CommandGreen;
			CommandYellow = other.CommandYellow;
			CommandBlue = other.CommandBlue;
			CommandUp = other.CommandUp;
			CommandDown = other.CommandDown;
			CommandLeft = other.CommandLeft;
			CommandRight = other.CommandRight;
			CommandSelect = other.CommandSelect;
		}

		/// <summary>
		/// Clears the configured commands.
		/// </summary>
		public void Clear()
		{
			CommandClear = null;
			CommandEnter = null;
			CommandChannelDown = null;
			CommandChannelUp = null;

			CommandRepeat = null;
			CommandRewind = null;
			CommandFastForward = null;
			CommandStop = null;
			CommandPlay = null;
			CommandPause = null;
			CommandRecord = null;

			CommandPageUp = null;
			CommandPageDown = null;
			CommandTopMenu = null;
			CommandPopupMenu = null;
			CommandReturn = null;
			CommandInfo = null;
			CommandEject = null;
			CommandPower = null;
			CommandRed = null;
			CommandGreen = null;
			CommandYellow = null;
			CommandBlue = null;
			CommandUp = null;
			CommandDown = null;
			CommandLeft = null;
			CommandRight = null;
			CommandSelect = null;
		}

		#endregion
	}
}
