#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif

namespace ICD.Connect.Sources.Barco.Responses.v1
{
	[JsonConverter(typeof(ModelResponseConverter))]
	public sealed class ModelResponse : AbstractClickshareApiV1Response<string>
	{
	}

	public sealed class ModelResponseConverter : AbstractClickshareApiV1ResponseConverter<ModelResponse, string>
	{
	}
}
