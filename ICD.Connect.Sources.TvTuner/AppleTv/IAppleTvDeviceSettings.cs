using ICD.Connect.Devices;
using ICD.Connect.Protocol.Ports.IrPort;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Sources.TvTuner.AppleTv
{
	public interface IAppleTvDeviceSettings : IDeviceSettings
	{
		/// <summary>
		/// The IR Port ID used to communicate with the device.
		/// </summary>
		[OriginatorIdSettingsProperty(typeof(IIrPort))]
		int? Port { get; set; }

		/// <summary>
		/// Represents the configured Apple TV IR command strings
		/// </summary>
		[HiddenSettingsProperty]
		AppleTvIrCommands Commands { get; }
	}
}