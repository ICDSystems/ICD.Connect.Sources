using System;

namespace ICD.Connect.Sources.TvTuner.AppleTv
{
	public sealed class AppleTvIrCommands
	{
		#region Defaults

		private const string COMMAND_UP = "UP";
		private const string COMMAND_DOWN = "DOWN";
		private const string COMMAND_LEFT = "LEFT";
		private const string COMMAND_RIGHT = "RIGHT";
		private const string COMMAND_SELECT = "SELECT";
		private const string COMMAND_MENU = "MENU";
		private const string COMMAND_PLAY_PAUSE_TOGGLE = "PLAY/PAUSE";

		#endregion

		#region Backing Fields

		private string m_CommandUp;
		private string m_CommandDown;
		private string m_CommandLeft;
		private string m_CommandRight;
		private string m_CommandSelect;
		private string m_CommandMenu;
		private string m_CommandPlayPauseToggle;

		#endregion

		#region Properties

		public string CommandUp { get { return m_CommandUp ?? COMMAND_UP; } set { m_CommandUp = value; } }
		public string CommandDown { get { return m_CommandDown ?? COMMAND_DOWN; } set { m_CommandDown = value; } }
		public string CommandLeft { get { return m_CommandLeft ?? COMMAND_LEFT; } set { m_CommandLeft = value; } }
		public string CommandRight { get { return m_CommandRight ?? COMMAND_RIGHT; } set { m_CommandRight = value; } }
		public string CommandSelect { get { return m_CommandSelect ?? COMMAND_SELECT; } set { m_CommandSelect = value; } }
		public string CommandMenu { get { return m_CommandMenu ?? COMMAND_MENU; } set { m_CommandMenu = value; } }
		public string CommandPlayPauseToggle { get { return m_CommandPlayPauseToggle ?? COMMAND_PLAY_PAUSE_TOGGLE; } set { m_CommandPlayPauseToggle = value; } }

		#endregion

		#region Methods

		/// <summary>
		/// Copies the commands from the other commands instance.
		/// </summary>
		/// <param name="other"></param>
		public void Update(AppleTvIrCommands other)
		{
			if (other == null)
				throw new ArgumentNullException("other");

			CommandUp = other.CommandUp;
			CommandDown = other.CommandDown;
			CommandLeft = other.CommandLeft;
			CommandRight = other.CommandRight;
			CommandSelect = other.CommandSelect;
			CommandMenu = other.CommandMenu;
			CommandPlayPauseToggle = other.CommandPlayPauseToggle;
		}

		/// <summary>
		/// Clears the configured commands.
		/// </summary>
		public void Clear()
		{
			CommandUp = null;
			CommandDown = null;
			CommandLeft = null;
			CommandRight = null;
			CommandSelect = null;
			CommandMenu = null;
			CommandPlayPauseToggle = null;
		}

		#endregion
	}
}