using ICD.Common.Utils.Xml;
using ICD.Connect.Conferencing.BYOD;
using ICD.Connect.Devices;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Sources.Barco.Devices.ClickshareConference
{
	[KrangSettings("BarcoClickshareConferenceDevice", typeof(BarcoClickshareConferenceDevice))]
	public sealed class BarcoClickshareConferenceDeviceSettings : AbstractBarcoClickshareDeviceSettings, IByodHubDeviceSettings
	{
		private const string CAMERA_DESTINATION_DEVICE_ELEMENT = "CameraDestinationDevice";

		/// <summary>
		/// Default API version to use for the device if not in config
		/// </summary>
		protected override string DefaultApiVersion { get { return "2.0"; } }

		/// <summary>
		/// Default port number to use for the device if not in the config
		/// </summary>
		protected override ushort? DefaultPortNumber { get { return 4003; } }

		/// <summary>
		/// Originator ID of a destination device for cameras to be routed to through the BYOD Hub Device.
		/// </summary>
		[OriginatorIdSettingsProperty(typeof(IDevice))]
		public int? CameraDestinationDevice { get; set; }

		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(CAMERA_DESTINATION_DEVICE_ELEMENT, IcdXmlConvert.ToString(CameraDestinationDevice));
		}

		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			CameraDestinationDevice = XmlUtils.TryReadChildElementContentAsInt(xml, CAMERA_DESTINATION_DEVICE_ELEMENT);
		}
	}
}