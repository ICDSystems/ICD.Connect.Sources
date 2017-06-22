using ICD.Common.Attributes.Properties;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings;

namespace ICD.Connect.Sources.Barco
{
	/// <summary>
	/// Settings for the BarcoClickshareDevice.
	/// </summary>
	public abstract class AbstractBarcoClickshareDeviceSettings : AbstractDeviceSettings
	{
		private const string PORT_ELEMENT = "Port";

		[SettingsProperty(SettingsProperty.ePropertyType.PortId)]
		public int? Port { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			if (Port != null)
				writer.WriteElementString(PORT_ELEMENT, IcdXmlConvert.ToString((int)Port));
		}

		/// <summary>
		/// Parses the xml and applies the properties to the instance.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="xml"></param>
		protected static void ParseXml(AbstractBarcoClickshareDeviceSettings instance, string xml)
		{
			instance.Port = XmlUtils.TryReadChildElementContentAsInt(xml, PORT_ELEMENT);

			AbstractDeviceSettings.ParseXml(instance, xml);
		}
	}
}
