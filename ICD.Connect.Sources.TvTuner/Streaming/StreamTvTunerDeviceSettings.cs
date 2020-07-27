using ICD.Connect.Devices;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Sources.TvTuner.Streaming
{
	[KrangSettings("StreamTvTuner", typeof(StreamTvTunerDevice))]
	public sealed class StreamTvTunerDeviceSettings : AbstractDeviceSettings
	{
	}
}
