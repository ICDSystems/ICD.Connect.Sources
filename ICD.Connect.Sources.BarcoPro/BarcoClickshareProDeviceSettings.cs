using ICD.Common.Properties;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Attributes.Factories;
using ICD.Connect.Settings.Core;
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
		/// Creates a new originator instance from the settings.
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public override IOriginator ToOriginator(IDeviceFactory factory)
		{
			BarcoClickshareProDevice output = new BarcoClickshareProDevice();
			output.ApplySettings(this, factory);
			return output;
		}

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
