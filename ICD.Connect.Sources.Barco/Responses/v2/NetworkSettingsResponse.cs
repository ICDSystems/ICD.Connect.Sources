using ICD.Common.Utils.Extensions;
using ICD.Connect.Sources.Barco.Responses.Common;
using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses.v2
{
	[JsonConverter(typeof(NetworkSettingsResponseConverter))]
	public sealed class NetworkSettingsResponse : AbstractBarcoClickshareApiV2Response
	{
		public string HostName { get; set; }

		public LanInfo Wired { get; set; }

		public WlanInfo Wireless { get; set; }
	}

	public sealed class NetworkSettingsResponseConverter : AbstractBarcoClickshareApiV2ResponseConverter<NetworkSettingsResponse>
	{
		private const string PROP_HOSTNAME = "hostname";
		private const string PROP_WIRED = "wired";
		private const string PROP_WIRELESS = "wireless";

		protected override void ReadProperty(string property, JsonReader reader, NetworkSettingsResponse instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case PROP_HOSTNAME:
					instance.HostName = reader.GetValueAsString();
					break;
				case PROP_WIRED:
					instance.Wired = serializer.Deserialize<LanInfo>(reader);
					break;
				case PROP_WIRELESS:
					instance.Wireless = serializer.Deserialize<WlanInfo>(reader);
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
