using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses
{
	[JsonConverter(typeof(WlanResponseConverter))]
	public sealed class WlanResponse : AbstractClickshareResponse<WlanInfo>
	{
	}

	public sealed class WlanResponseConverter : AbstractClickshareResponseConverter<WlanResponse, WlanInfo>
	{
	}

	[JsonConverter(typeof(WlanInfoConverter))]
	public sealed class WlanInfo
	{
		public string MacAddress { get; set; }
		public string IpAddress { get; set; }
	}

	public sealed class WlanInfoConverter : AbstractGenericJsonConverter<WlanInfo>
	{	
		private const string KEY_NETWORK_IP_ADDRESS = "IpAddress";
		private const string KEY_NETWORK_MAC_ADDRESS = "MacAddress";

		/// <summary>
		/// Override to handle the current property value with the given name.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="reader"></param>
		/// <param name="instance"></param>
		/// <param name="serializer"></param>
		protected override void ReadProperty(string property, JsonReader reader, WlanInfo instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case KEY_NETWORK_IP_ADDRESS:
					instance.IpAddress = reader.GetValueAsString();
					break;

				case KEY_NETWORK_MAC_ADDRESS:
					instance.MacAddress = reader.GetValueAsString();
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
