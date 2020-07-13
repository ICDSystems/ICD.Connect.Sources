using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses.v1
{
	[JsonConverter(typeof(SharingStateResponseConverter))]
	public sealed class SharingStateResponse : AbstractClickshareApiV1Response<bool>
	{
	}

	public sealed class SharingStateResponseConverter : AbstractClickshareApiV1ResponseConverter<SharingStateResponse, bool>
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
