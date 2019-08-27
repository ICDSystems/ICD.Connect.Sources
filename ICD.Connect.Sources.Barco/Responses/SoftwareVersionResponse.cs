using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses
{
	[JsonConverter(typeof(SoftwareVersionResponseConverter))]
	public sealed class SoftwareVersionResponse : AbstractClickshareResponse<string>
	{
	}

	public sealed class SoftwareVersionResponseConverter : AbstractClickshareResponseConverter<SoftwareVersionResponse, string>
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
