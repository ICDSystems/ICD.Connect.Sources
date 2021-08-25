#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;

namespace ICD.Connect.Sources.Barco.Responses.Common
{
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
		private const string KEY_NETWORK_ADDRESSING = "addressing";
		private const string KEY_NETWORK_IP_ADDRESS = "ipaddress";
		private const string KEY_NETWORK_SUBNET_MASK = "subnetmask";
		private const string KEY_NETWORK_DEFAULT_GATEWAY = "defaultgateway";
		private const string KEY_NETWORK_HOSTNAME = "hostname";

		/// <summary>
		/// Override to handle the current property value with the given name.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="reader"></param>
		/// <param name="instance"></param>
		/// <param name="serializer"></param>
		protected override void ReadProperty(string property, JsonReader reader, LanInfo instance, JsonSerializer serializer)
		{
			// Supports API v1 & v2 which have different capitalization rules 
			switch (property.ToLower())
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
