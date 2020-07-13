using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses.v1
{
	[JsonConverter(typeof(SoftwareVersionResponseConverter))]
	public sealed class SoftwareVersionResponse : AbstractClickshareApiV1Response<string>
	{
	}

	public sealed class SoftwareVersionResponseConverter : AbstractClickshareApiV1ResponseConverter<SoftwareVersionResponse, string>
	{
		/*
		{
			"status": 200,
			"message": "GET successful",
			"data": {
				"key": "/v1.12/Software/FirmwareVersion",
				"value": "01.07.00.0022"
			}
		}
		*/
	}
}
