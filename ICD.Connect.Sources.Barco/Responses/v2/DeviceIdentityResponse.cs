using ICD.Common.Utils.Extensions;
using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses.v2
{
	[JsonConverter(typeof(DeviceIdentityResponseConverter))]
	public sealed class DeviceIdentityResponse : AbstractBarcoClickshareApiV2Response
	{
		public string ArticleNumber { get; set; }

		public string ModelName { get; set; }
		
		public string ProductName { get; set; }
		
		public string SerialNumber { get; set; }
	}

	public sealed class DeviceIdentityResponseConverter : AbstractBarcoClickshareApiV2ResponseConverter<DeviceIdentityResponse>
	{
		/*
		 {
			"serialNumber": "1862334926",
			"articleNumber": "R9861513NA",
			"modelName": "C3010S",
			"productName": "CX-30"
		 }
		 */

		private const string PROP_SERIAL_NUMBER = "serialNumber";
		private const string PROP_ARTICLE_NUMBER = "articleNumber";
		private const string PROP_MODEL_NAME = "modelName";
		private const string PROP_PRODUCT_NAME = "productName";

		protected override void ReadProperty(string property, JsonReader reader, DeviceIdentityResponse instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case PROP_SERIAL_NUMBER:
					instance.SerialNumber = reader.GetValueAsString();
					break;
				case PROP_ARTICLE_NUMBER:
					instance.ArticleNumber = reader.GetValueAsString();
					break;
				case PROP_MODEL_NAME:
					instance.ModelName = reader.GetValueAsString();
					break;
				case PROP_PRODUCT_NAME:
					instance.ProductName = reader.GetValueAsString();
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
