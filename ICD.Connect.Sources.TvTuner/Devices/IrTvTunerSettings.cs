using ICD.Common.Utils.Xml;
using ICD.Connect.Protocol.Ports.IrPort;
using ICD.Connect.Protocol.Settings;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Sources.TvTuner.Devices
{
	[KrangSettings("IrTvTuner", typeof(IrTvTunerDevice))]
	public sealed class IrTvTunerSettings : AbstractTvTunerSettings, IIrDriverSettings
	{
		private const string PORT_ELEMENT = "Port";

		#region IR Command Elements

		private const string ELEMENT_IR_COMMANDS = "IrCommands";
		private const string ELEMENT_CHANNELS = "Channels";
		private const string ELEMENT_PLAYBACK = "Playback";
		private const string ELEMENT_MENUS = "Menus";

		private const string ELEMENT_CLEAR = "Clear";
		private const string ELEMENT_ENTER = "Enter";
		private const string ELEMENT_CHANNEL_UP = "ChannelUp";
		private const string ELEMENT_CHANNEL_DOWN = "ChannelDown";

		private const string ELEMENT_REPEAT = "Repeat";
		private const string ELEMENT_REWIND = "Rewind";
		private const string ELEMENT_FAST_FORWARD = "FastForward";
		private const string ELEMENT_STOP = "Stop";
		private const string ELEMENT_PLAY = "Play";
		private const string ELEMENT_PAUSE = "Pause";
		private const string ELEMENT_RECORD = "Record";

		private const string ELEMENT_PAGE_UP = "PageUp";
		private const string ELEMENT_PAGE_DOWN = "PageDown";
		private const string ELEMENT_TOP_MENU = "TopMenu";
		private const string ELEMENT_POPUP_MENU = "PopupMenu";
		private const string ELEMENT_RETURN = "Return";
		private const string ELEMENT_INFO = "Info";
		private const string ELEMENT_EJECT = "Eject";
		private const string ELEMENT_POWER = "Power";
		private const string ELEMENT_RED = "Red";
		private const string ELEMENT_GREEN = "Green";
		private const string ELEMENT_YELLOW = "Yellow";
		private const string ELEMENT_BLUE = "Blue";
		private const string ELEMENT_UP = "Up";
		private const string ELEMENT_DOWN = "Down";
		private const string ELEMENT_LEFT = "Left";
		private const string ELEMENT_RIGHT = "Right";
		private const string ELEMENT_SELECT = "Select";

		#endregion

		private readonly IrTvTunerCommands m_Commands;
		private readonly IrDriverProperties m_IrDriverProperties;

		#region Properties

		[OriginatorIdSettingsProperty(typeof(IIrPort))]
		public int? Port { get; set; }

		[HiddenSettingsProperty]
		public IrTvTunerCommands Commands { get { return m_Commands; } }

		public string CommandClear { get { return m_Commands.CommandClear; } set { m_Commands.CommandClear = value; } }
		public string CommandEnter { get { return m_Commands.CommandEnter; } set { m_Commands.CommandEnter = value; } }
		public string CommandChannelUp { get { return m_Commands.CommandChannelUp; } set { m_Commands.CommandChannelUp = value; } }
		public string CommandChannelDown { get { return m_Commands.CommandChannelDown; } set { m_Commands.CommandChannelDown = value; } }

		public string CommandRepeat { get { return m_Commands.CommandRepeat; } set { m_Commands.CommandRepeat = value; } }
		public string CommandRewind { get { return m_Commands.CommandRewind; } set { m_Commands.CommandRewind = value; } }
		public string CommandFastForward { get { return m_Commands.CommandFastForward; } set { m_Commands.CommandFastForward = value; } }
		public string CommandStop { get { return m_Commands.CommandStop; } set { m_Commands.CommandStop = value; } }
		public string CommandPlay { get { return m_Commands.CommandPlay; } set { m_Commands.CommandPlay = value; } }
		public string CommandPause { get { return m_Commands.CommandPause; } set { m_Commands.CommandPause = value; } }
		public string CommandRecord { get { return m_Commands.CommandRecord; } set { m_Commands.CommandRecord = value; } }

		public string CommandPageUp { get { return m_Commands.CommandPageUp; } set { m_Commands.CommandPageUp = value; } }
		public string CommandPageDown { get { return m_Commands.CommandPageDown; } set { m_Commands.CommandPageDown = value; } }
		public string CommandTopMenu { get { return m_Commands.CommandTopMenu; } set { m_Commands.CommandTopMenu = value; } }
		public string CommandPopupMenu { get { return m_Commands.CommandPopupMenu; } set { m_Commands.CommandPopupMenu = value; } }
		public string CommandReturn { get { return m_Commands.CommandReturn; } set { m_Commands.CommandReturn = value; } }
		public string CommandInfo { get { return m_Commands.CommandInfo; } set { m_Commands.CommandInfo = value; } }
		public string CommandEject { get { return m_Commands.CommandEject; } set { m_Commands.CommandEject = value; } }
		public string CommandPower { get { return m_Commands.CommandPower; } set { m_Commands.CommandPower = value; } }
		public string CommandRed { get { return m_Commands.CommandRed; } set { m_Commands.CommandRed = value; } }
		public string CommandGreen { get { return m_Commands.CommandGreen; } set { m_Commands.CommandGreen = value; } }
		public string CommandYellow { get { return m_Commands.CommandYellow; } set { m_Commands.CommandYellow = value; } }
		public string CommandBlue { get { return m_Commands.CommandBlue; } set { m_Commands.CommandBlue = value; } }
		public string CommandUp { get { return m_Commands.CommandUp; } set { m_Commands.CommandUp = value; } }
		public string CommandDown { get { return m_Commands.CommandDown; } set { m_Commands.CommandDown = value; } }
		public string CommandLeft { get { return m_Commands.CommandLeft; } set { m_Commands.CommandLeft = value; } }
		public string CommandRight { get { return m_Commands.CommandRight; } set { m_Commands.CommandRight = value; } }
		public string CommandSelect { get { return m_Commands.CommandSelect; } set { m_Commands.CommandSelect = value; } }

		#endregion

		#region IR Driver

		/// <summary>
		/// Gets/sets the configurable path to the IR driver.
		/// </summary>
		public string IrDriverPath
		{
			get { return m_IrDriverProperties.IrDriverPath; }
			set { m_IrDriverProperties.IrDriverPath = value; }
		}

		/// <summary>
		/// Gets/sets the configurable pulse time for the IR driver.
		/// </summary>
		public ushort? IrPulseTime
		{
			get { return m_IrDriverProperties.IrPulseTime; }
			set { m_IrDriverProperties.IrPulseTime = value; }
		}

		/// <summary>
		/// Gets/sets the configurable between time for the IR driver.
		/// </summary>
		public ushort? IrBetweenTime
		{
			get { return m_IrDriverProperties.IrBetweenTime; }
			set { m_IrDriverProperties.IrBetweenTime = value; }
		}

		/// <summary>
		/// Clears the configured values.
		/// </summary>
		void IIrDriverProperties.ClearIrProperties()
		{
			m_IrDriverProperties.ClearIrProperties();
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public IrTvTunerSettings()
		{
			m_Commands = new IrTvTunerCommands();
			m_IrDriverProperties = new IrDriverProperties();
		}

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PORT_ELEMENT, IcdXmlConvert.ToString(Port));

			writer.WriteStartElement(ELEMENT_IR_COMMANDS);
			{
				writer.WriteStartElement(ELEMENT_CHANNELS);
				{
					writer.WriteElementString(ELEMENT_CLEAR, CommandClear);
					writer.WriteElementString(ELEMENT_ENTER, CommandEnter);
					writer.WriteElementString(ELEMENT_CHANNEL_UP, CommandChannelUp);
					writer.WriteElementString(ELEMENT_CHANNEL_DOWN, CommandChannelDown);
				}
				writer.WriteEndElement();

				writer.WriteStartElement(ELEMENT_PLAYBACK);
				{
					writer.WriteElementString(ELEMENT_REPEAT, CommandRepeat);
					writer.WriteElementString(ELEMENT_REWIND, CommandRewind);
					writer.WriteElementString(ELEMENT_FAST_FORWARD, CommandFastForward);
					writer.WriteElementString(ELEMENT_STOP, CommandStop);
					writer.WriteElementString(ELEMENT_PLAY, CommandPlay);
					writer.WriteElementString(ELEMENT_PAUSE, CommandPause);
					writer.WriteElementString(ELEMENT_RECORD, CommandRecord);
				}
				writer.WriteEndElement();

				writer.WriteStartElement(ELEMENT_MENUS);
				{
					writer.WriteElementString(ELEMENT_PAGE_UP, CommandPageUp);
					writer.WriteElementString(ELEMENT_PAGE_DOWN, CommandPageDown);
					writer.WriteElementString(ELEMENT_TOP_MENU, CommandTopMenu);
					writer.WriteElementString(ELEMENT_POPUP_MENU, CommandPopupMenu);
					writer.WriteElementString(ELEMENT_RETURN, CommandReturn);
					writer.WriteElementString(ELEMENT_INFO, CommandInfo);
					writer.WriteElementString(ELEMENT_EJECT, CommandEject);
					writer.WriteElementString(ELEMENT_POWER, CommandPower);
					writer.WriteElementString(ELEMENT_RED, CommandRed);
					writer.WriteElementString(ELEMENT_GREEN, CommandGreen);
					writer.WriteElementString(ELEMENT_YELLOW, CommandYellow);
					writer.WriteElementString(ELEMENT_BLUE, CommandBlue);
					writer.WriteElementString(ELEMENT_UP, CommandUp);
					writer.WriteElementString(ELEMENT_DOWN, CommandDown);
					writer.WriteElementString(ELEMENT_LEFT, CommandLeft);
					writer.WriteElementString(ELEMENT_RIGHT, CommandRight);
					writer.WriteElementString(ELEMENT_SELECT, CommandSelect);
				}
				writer.WriteEndElement();
			}
			writer.WriteEndElement();

			m_IrDriverProperties.WriteElements(writer);
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			Port = XmlUtils.TryReadChildElementContentAsInt(xml, PORT_ELEMENT);

			string irCommands;
			string channels = null;
			string playback = null;
			string menus = null;

			XmlUtils.TryGetChildElementAsString(xml, ELEMENT_IR_COMMANDS, out irCommands);

			if (irCommands != null)
			{
				XmlUtils.TryGetChildElementAsString(irCommands, ELEMENT_CHANNELS, out channels);
				XmlUtils.TryGetChildElementAsString(irCommands, ELEMENT_PLAYBACK, out playback);
				XmlUtils.TryGetChildElementAsString(irCommands, ELEMENT_MENUS, out menus);
			}

			CommandClear = channels == null ? null : XmlUtils.TryReadChildElementContentAsString(channels, ELEMENT_CLEAR);
			CommandEnter = channels == null ? null : XmlUtils.TryReadChildElementContentAsString(channels, ELEMENT_ENTER);
			CommandChannelUp = channels == null ? null : XmlUtils.TryReadChildElementContentAsString(channels, ELEMENT_CHANNEL_UP);
			CommandChannelDown = channels == null ? null : XmlUtils.TryReadChildElementContentAsString(channels, ELEMENT_CHANNEL_DOWN);

			CommandRepeat = playback == null ? null : XmlUtils.TryReadChildElementContentAsString(playback, ELEMENT_REPEAT);
			CommandRewind = playback == null ? null : XmlUtils.TryReadChildElementContentAsString(playback, ELEMENT_REWIND);
			CommandFastForward = playback == null ? null : XmlUtils.TryReadChildElementContentAsString(playback, ELEMENT_FAST_FORWARD);
			CommandStop = playback == null ? null : XmlUtils.TryReadChildElementContentAsString(playback, ELEMENT_STOP);
			CommandPlay = playback == null ? null : XmlUtils.TryReadChildElementContentAsString(playback, ELEMENT_PLAY);
			CommandPause = playback == null ? null : XmlUtils.TryReadChildElementContentAsString(playback, ELEMENT_PAUSE);
			CommandRecord = playback == null ? null : XmlUtils.TryReadChildElementContentAsString(playback, ELEMENT_RECORD);

			CommandPageUp = menus == null ? null : XmlUtils.TryReadChildElementContentAsString(menus, ELEMENT_PAGE_UP);
			CommandPageDown = menus == null ? null : XmlUtils.TryReadChildElementContentAsString(menus, ELEMENT_PAGE_DOWN);
			CommandTopMenu = menus == null ? null : XmlUtils.TryReadChildElementContentAsString(menus, ELEMENT_TOP_MENU);
			CommandPopupMenu = menus == null ? null : XmlUtils.TryReadChildElementContentAsString(menus, ELEMENT_POPUP_MENU);
			CommandReturn = menus == null ? null : XmlUtils.TryReadChildElementContentAsString(menus, ELEMENT_RETURN);
			CommandInfo = menus == null ? null : XmlUtils.TryReadChildElementContentAsString(menus, ELEMENT_INFO);
			CommandEject = menus == null ? null : XmlUtils.TryReadChildElementContentAsString(menus, ELEMENT_EJECT);
			CommandPower = menus == null ? null : XmlUtils.TryReadChildElementContentAsString(menus, ELEMENT_POWER);
			CommandRed = menus == null ? null : XmlUtils.TryReadChildElementContentAsString(menus, ELEMENT_RED);
			CommandGreen = menus == null ? null : XmlUtils.TryReadChildElementContentAsString(menus, ELEMENT_GREEN);
			CommandYellow = menus == null ? null : XmlUtils.TryReadChildElementContentAsString(menus, ELEMENT_YELLOW);
			CommandBlue = menus == null ? null : XmlUtils.TryReadChildElementContentAsString(menus, ELEMENT_BLUE);
			CommandUp = menus == null ? null : XmlUtils.TryReadChildElementContentAsString(menus, ELEMENT_UP);
			CommandDown = menus == null ? null : XmlUtils.TryReadChildElementContentAsString(menus, ELEMENT_DOWN);
			CommandLeft = menus == null ? null : XmlUtils.TryReadChildElementContentAsString(menus, ELEMENT_LEFT);
			CommandRight = menus == null ? null : XmlUtils.TryReadChildElementContentAsString(menus, ELEMENT_RIGHT);
			CommandSelect = menus == null ? null : XmlUtils.TryReadChildElementContentAsString(menus, ELEMENT_SELECT);

			m_IrDriverProperties.ParseXml(xml);
		}
	}
}
