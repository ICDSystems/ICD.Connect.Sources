using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Sources.Barco.Devices.ClickshareConference
{
	[KrangSettings("BarcoClickshareConferenceDevice", typeof(BarcoClickshareConferenceDevice))]
	public sealed class BarcoClickshareConferenceDeviceSettings : AbstractBarcoClickshareDeviceSettings
	{
		/// <summary>
		/// Default API version to use for the device if not in config
		/// </summary>
		protected override string DefaultApiVersion { get { return "2.0"; } }

		/// <summary>
		/// Default port number to use for the device if not in the config
		/// </summary>
		protected override ushort? DefaultPortNumber { get { return 4003; } }
	}
}