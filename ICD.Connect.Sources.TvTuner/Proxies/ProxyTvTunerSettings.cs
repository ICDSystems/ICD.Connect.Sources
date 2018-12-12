using ICD.Connect.Devices.Proxies.Devices;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Sources.TvTuner.Proxies
{
	[KrangSettings("ProxyTvTuner", typeof(ProxyTvTuner))]
	public sealed class ProxyTvTunerSettings : AbstractProxyDeviceSettings
	{
	}
}