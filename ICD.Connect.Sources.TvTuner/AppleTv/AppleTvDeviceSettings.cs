using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;
using ICD.Connect.Protocol.Settings;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Sources.TvTuner.AppleTv
{
	[KrangSettings("AppleTv", typeof(AppleTvDevice))]
	public sealed class AppleTvDeviceSettings : AbstractDeviceSettings, IAppleTvDeviceSettings, IIrDriverSettings
	{
		private const string PORT_ELEMENT = "Port";

		private const string ELEMENT_IR_COMMANDS = "IrCommands";
		private const string ELEMENT_UP = "Up";
		private const string ELEMENT_DOWN = "Down";
		private const string ELEMENT_LEFT = "Left";
		private const string ELEMENT_RIGHT = "Right";
		private const string ELEMENT_SELECT = "Select";
		private const string ELEMENT_MENU = "Menu";
		private const string ELEMENT_PLAY_PAUSE_TOGGLE = "PlayPauseToggle";

		private readonly AppleTvIrCommands m_Commands;
		private readonly IrDriverProperties m_IrDriverProperties;

		#region Properties

		/// <summary>
		/// The IR Port ID used to communicate with the device.
		/// </summary>
		public int? Port { get; set; }

		/// <summary>
		/// Represents the configured Apple TV IR command strings
		/// </summary>
		public AppleTvIrCommands Commands { get { return m_Commands; } }

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

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public AppleTvDeviceSettings()
		{
			m_Commands = new AppleTvIrCommands();
			m_IrDriverProperties = new IrDriverProperties();
		}

		#endregion

		#region Methods

		public void ClearIrProperties()
		{
			m_IrDriverProperties.ClearIrProperties();
		}

		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PORT_ELEMENT, IcdXmlConvert.ToString(Port));

			m_IrDriverProperties.WriteElements(writer);

			writer.WriteStartElement(ELEMENT_IR_COMMANDS);
			{
				writer.WriteElementString(ELEMENT_UP, Commands.CommandUp);
				writer.WriteElementString(ELEMENT_DOWN, Commands.CommandDown);
				writer.WriteElementString(ELEMENT_LEFT, Commands.CommandLeft);
				writer.WriteElementString(ELEMENT_RIGHT, Commands.CommandRight);
				writer.WriteElementString(ELEMENT_SELECT, Commands.CommandSelect);
				writer.WriteElementString(ELEMENT_MENU, Commands.CommandMenu);
				writer.WriteElementString(ELEMENT_PLAY_PAUSE_TOGGLE, Commands.CommandPlayPauseToggle);
			}
			writer.WriteEndElement();
		}

		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			Port = XmlUtils.TryReadChildElementContentAsInt(xml, PORT_ELEMENT);

			m_IrDriverProperties.ParseXml(xml);

			string irCommands;
			XmlUtils.TryGetChildElementAsString(xml, ELEMENT_IR_COMMANDS, out irCommands);

			Commands.CommandUp = irCommands == null ? null : XmlUtils.TryReadChildElementContentAsString(irCommands, ELEMENT_UP);
			Commands.CommandDown = irCommands == null ? null : XmlUtils.TryReadChildElementContentAsString(irCommands, ELEMENT_DOWN);
			Commands.CommandLeft = irCommands == null ? null : XmlUtils.TryReadChildElementContentAsString(irCommands, ELEMENT_LEFT);
			Commands.CommandRight = irCommands == null ? null : XmlUtils.TryReadChildElementContentAsString(irCommands, ELEMENT_RIGHT);
			Commands.CommandSelect = irCommands == null ? null : XmlUtils.TryReadChildElementContentAsString(irCommands, ELEMENT_SELECT);
			Commands.CommandMenu = irCommands == null ? null : XmlUtils.TryReadChildElementContentAsString(irCommands, ELEMENT_MENU);
			Commands.CommandPlayPauseToggle = irCommands == null ? null : XmlUtils.TryReadChildElementContentAsString(irCommands, ELEMENT_PLAY_PAUSE_TOGGLE);
		}

		#endregion
	}
}