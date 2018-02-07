using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;
using ICD.Connect.Protocol.Ports.IrPort;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Sources.TvTuner.Devices
{
	public sealed class IrTvTunerSettings : AbstractTvTunerSettings
	{
		private const string FACTORY_NAME = "IrTvTuner";
		private const string PORT_ELEMENT = "Port";

		[OriginatorIdSettingsProperty(typeof(IIrPort))]
		public int? Port { get; set; }

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(IrTvTunerDevice); } }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PORT_ELEMENT, IcdXmlConvert.ToString(Port));
		}

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlFactoryMethod(FACTORY_NAME)]
		public static IrTvTunerSettings FromXml(string xml)
		{
			int? port = XmlUtils.TryReadChildElementContentAsInt(xml, PORT_ELEMENT);

			IrTvTunerSettings output = new IrTvTunerSettings
			{
				Port = port
			};

			output.ParseXml(xml);
			return output;
		}
	}
}
