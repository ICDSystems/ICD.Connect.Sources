using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses.v2
{
	[JsonConverter(typeof(VersionResponseConverter))]
	public sealed class VersionResponse : AbstractBarcoClickshareApiV2Response
	{
		public int Status { get; set; }

		public string Message { get; set; }

		public KeyValuePair<string, Version> Data { get; set; }
	}

	public sealed class VersionResponseConverter : AbstractBarcoClickshareApiV2ResponseConverter<VersionResponse>
	{
		/*
		 {
			"status": 200,
			"message": "GET successful",
			"data": {
				"key": "/CurrentVersion",
				"value": "v2.0"
			}
		 }
		 */

		private const string PROP_STATUS = "status";
		private const string PROP_MESSAGE = "message";
		private const string PROP_DATA = "data";
		private const string PROP_KEY = "key";
		private const string PROP_VALUE = "value";

		protected override void ReadProperty(string property, JsonReader reader, VersionResponse instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case PROP_STATUS:
					instance.Status = reader.GetValueAsInt();
					break;
				case PROP_MESSAGE:
					instance.Message = reader.GetValueAsString();
					break;
				case PROP_DATA:
					reader.ReadObject(serializer, (p, r, s) => ReadData(p, r, instance, s));
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}

		private void ReadData(string property, JsonReader reader, VersionResponse instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case PROP_KEY:
					string key = reader.GetValueAsString();
					instance.Data = new KeyValuePair<string, Version>(key, instance.Data.Value);
					break;
				case PROP_VALUE:
					Version value = new Version(reader.GetValueAsString());
					instance.Data = new KeyValuePair<string, Version>(instance.Data.Key, value);
					break;
			}
		}
	}
}
