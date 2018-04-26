using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Sources.TvTuner.Devices
{
	[KrangSettings("MockTvTuner", typeof(MockTvTunerDevice))]
	public sealed class MockTvTunerSettings : AbstractTvTunerSettings
	{
	}
}
