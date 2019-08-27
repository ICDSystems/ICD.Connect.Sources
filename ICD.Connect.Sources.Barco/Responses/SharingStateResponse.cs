using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses
{
	[JsonConverter(typeof(SharingStateResponseConverter))]
	public sealed class SharingStateResponse : AbstractClickshareResponse<bool>
	{
	}

	public sealed class SharingStateResponseConverter : AbstractClickshareResponseConverter<SharingStateResponse, bool>
	{
		/*
		{
			"status": 200,
			"message": "GET successful",
			"data": {
				"key": "/v1.12/DeviceInfo/Sharing",
				"value": false
			}
		}
		*/
	}
}
