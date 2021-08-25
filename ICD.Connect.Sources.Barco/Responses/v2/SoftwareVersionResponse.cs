#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using System;
using ICD.Common.Utils.Extensions;

namespace ICD.Connect.Sources.Barco.Responses.v2
{
	[JsonConverter(typeof(SoftwareVersionResponseConverter))]
	public sealed class SoftwareVersionResponse : AbstractBarcoClickshareApiV2Response
	{
		public DateTime Date { get; set; }

		public Version Version { get; set; }
	}

	public sealed class SoftwareVersionResponseConverter : AbstractBarcoClickshareApiV2ResponseConverter<SoftwareVersionResponse>
	{
		/*
		 {
			"date": "2020-06-25T19:25:01",
			"version": "02.05.00.0011"
		 }
		*/

		private const string PROP_DATE = "date";
		private const string PROP_VERSION = "version";

		protected override void ReadProperty(string property, JsonReader reader, SoftwareVersionResponse instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case PROP_DATE:
					instance.Date = reader.GetValueAsDateTime();
					break;
				case PROP_VERSION:
					instance.Version = new Version(reader.GetValueAsString());
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
