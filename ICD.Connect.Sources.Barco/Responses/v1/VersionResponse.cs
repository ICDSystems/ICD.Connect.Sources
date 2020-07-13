using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses.v1
{
	[JsonConverter(typeof(VersionResponseConverter))]
	public sealed class VersionResponse : AbstractClickshareApiV1Response<string>
	{
	}

	public sealed class VersionResponseConverter : AbstractClickshareApiV1ResponseConverter<VersionResponse, string>
	{
		/*
		{
			"status": 200,
			"message": "GET successful",
			"data": {
				"key": "/CurrentVersion",
				"value": "v1.12"
			}
		}
		*/
	}
}
