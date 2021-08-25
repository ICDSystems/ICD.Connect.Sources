#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Sources.Barco.Responses.Common;

namespace ICD.Connect.Sources.Barco.Responses.v2
{
	[JsonConverter(typeof(NetworkSettingsResponseConverter))]
	public sealed class NetworkSettingsResponse : AbstractBarcoClickshareApiV2Response
	{
		public string HostName { get; set; }

		public IEnumerable<LanInfo> Wired { get; set; }

		public IEnumerable<WlanInfo> Wireless { get; set; }
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
					instance.Wired = serializer.DeserializeArray<LanInfo>(reader).ToArray();
					break;
				case PROP_WIRELESS:
					instance.Wireless = serializer.DeserializeArray<WlanInfo>(reader).ToArray();
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
