using System;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Sources.TvTuner.Devices
{
	[KrangSettings(FACTORY_NAME)]
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
	}
}