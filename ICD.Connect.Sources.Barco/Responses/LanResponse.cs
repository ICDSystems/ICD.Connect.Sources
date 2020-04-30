using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses
{
	[JsonConverter(typeof(LanResponseConverter))]
	public sealed class LanResponse : AbstractClickshareResponse<LanInfo>
	{
	}

	public sealed class LanResponseConverter : AbstractClickshareResponseConverter<LanResponse, LanInfo>
	{
	}

	[JsonConverter(typeof(LanInfoConverter))]
	public sealed class LanInfo
	{
		public string Addressing { get; set; }
		public string IpAddress { get; set; }
		public string SubnetMask { get; set; }
		public string DefaultGateway { get; set; }
		public string Hostname { get; set; }
	}

	public sealed class LanInfoConverter : AbstractGenericJsonConverter<LanInfo>
	{
		private const string KEY_NETWORK_ADDRESSING = "Addressing";
		private const string KEY_NETWORK_IP_ADDRESS = "IpAddress";
		private const string KEY_NETWORK_SUBNET_MASK = "SubnetMask";
		private const string KEY_NETWORK_DEFAULT_GATEWAY = "DefaultGateway";
		private const string KEY_NETWORK_HOSTNAME = "Hostname";

		/// <summary>
		/// Override to handle the current property value with the given name.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="reader"></param>
		/// <param name="instance"></param>
		/// <param name="serializer"></param>
		protected override void ReadProperty(string property, JsonReader reader, LanInfo instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case KEY_NETWORK_ADDRESSING:
					instance.Addressing = reader.GetValueAsString();
					break;

				case KEY_NETWORK_IP_ADDRESS:
					instance.IpAddress = reader.GetValueAsString();
					break;

				case KEY_NETWORK_SUBNET_MASK:
					instance.SubnetMask = reader.GetValueAsString();
					break;

				case KEY_NETWORK_DEFAULT_GATEWAY:
					instance.DefaultGateway = reader.GetValueAsString();
					break;

				case KEY_NETWORK_HOSTNAME:
					instance.Hostname = reader.GetValueAsString();
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
