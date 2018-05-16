using ICD.Connect.Settings.Attributes;
using ICD.Connect.Sources.Barco;

namespace ICD.Connect.Sources.BarcoPro
{
	[KrangSettings("BarcoClickshare", typeof(BarcoClickshareProDevice))]
	public sealed class BarcoClickshareProDeviceSettings : AbstractBarcoClickshareDeviceSettings
	{
	}
}
