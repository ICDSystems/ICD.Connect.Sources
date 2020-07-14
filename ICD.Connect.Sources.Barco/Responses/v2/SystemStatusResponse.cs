using System;
using ICD.Common.Utils.Extensions;
using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses.v2
{
	[JsonConverter(typeof(SystemStatusRespnseConverter))]
	public sealed class SystemStatusResponse : AbstractBarcoClickshareApiV2Response
	{
		public string ErrorCode { get; set; }

		public string ErrorMessage { get; set; }

		public int CurrentUptime { get; set; }

		public int TotalUptime { get; set; }

		public DateTime FirstUsed { get; set; }

		public bool InUse { get; set; }

		public bool Sharing { get; set; }
	}

	public sealed class SystemStatusRespnseConverter : AbstractBarcoClickshareApiV2ResponseConverter<SystemStatusResponse>
	{
		/*
		 {
			"errorCode": 0,
			"errorMessage": " ",
			"currentUptime": 1280329,
			"totalUptime": 1283038,
			"firstUsed": "2020-06-18T15:24:18",
			"inUse": false,
			"sharing": false
		 }
		 */

		private const string PROP_ERROR_CODE = "errorCode";
		private const string PROP_ERROR_MESSAGE = "errorMessage";
		private const string PROP_CURRENT_UPTIME = "currentUptime";
		private const string PROP_TOTAL_UPTIME = "totalUptime";
		private const string PROP_FIRST_USED = "firstUsed";
		private const string PROP_IN_USE = "inUse";
		private const string PROP_SHARING = "sharing";

		protected override void ReadProperty(string property, JsonReader reader, SystemStatusResponse instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case PROP_ERROR_CODE:
					instance.ErrorCode = reader.GetValueAsString();
					break;
				case PROP_ERROR_MESSAGE:
					instance.ErrorMessage = reader.GetValueAsString();
					break;
				case PROP_CURRENT_UPTIME:
					instance.CurrentUptime = reader.GetValueAsInt();
					break;
				case PROP_TOTAL_UPTIME:
					instance.TotalUptime = reader.GetValueAsInt();
					break;
				case PROP_FIRST_USED:
					instance.FirstUsed = reader.GetValueAsDateTime();
					break;
				case PROP_IN_USE:
					instance.InUse = reader.GetValueAsBool();
					break;
				case PROP_SHARING:
					instance.Sharing = reader.GetValueAsBool();
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
