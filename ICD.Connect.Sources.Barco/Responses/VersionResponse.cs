using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses
{
	[JsonConverter(typeof(VersionResponseConverter))]
	public sealed class VersionResponse : AbstractClickshareResponse<string>
	{
	}

	public sealed class VersionResponseConverter : AbstractClickshareResponseConverter<VersionResponse, string>
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
