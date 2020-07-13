using Newtonsoft.Json;

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
