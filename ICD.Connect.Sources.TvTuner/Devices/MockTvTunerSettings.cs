using ICD.Common.Utils.Xml;
using ICD.Connect.Devices.Mock;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Sources.TvTuner.Devices
{
	[KrangSettings("MockTvTuner", typeof(MockTvTunerDevice))]
	public sealed class MockTvTunerSettings : AbstractTvTunerSettings, IMockDeviceSettings
	{
		public bool DefaultOffline { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			MockDeviceSettingsHelper.WriteElements(this, writer);
		}

		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			MockDeviceSettingsHelper.ParseXml(this, xml);
		}
	}
}
