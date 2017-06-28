using System;
using ICD.Common.Properties;
using ICD.Connect.Settings.Attributes.Factories;
using ICD.Connect.Sources.Barco;

namespace ICD.Connect.Sources.BarcoPro
{
	public sealed class BarcoClickshareProDeviceSettings : AbstractBarcoClickshareDeviceSettings
	{
		private const string FACTORY_NAME = "BarcoClickshare";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(BarcoClickshareProDevice); } }

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlDeviceSettingsFactoryMethod(FACTORY_NAME)]
		public static BarcoClickshareProDeviceSettings FromXml(string xml)
		{
			BarcoClickshareProDeviceSettings output = new BarcoClickshareProDeviceSettings();
			ParseXml(output, xml);
			return output;
		}
	}
}
