#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif

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
