using System;
using ICD.Common.Properties;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Sources.TvTuner
{
	public sealed class MockTvTunerSettings : AbstractTvTunerSettings
	{
		private const string FACTORY_NAME = "MockTvTuner";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(MockTvTunerDevice); } }

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlFactoryMethod(FACTORY_NAME)]
		public static MockTvTunerSettings FromXml(string xml)
		{
			MockTvTunerSettings output = new MockTvTunerSettings();
			ParseXml(output, xml);
			return output;
		}
	}
}