using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses
{
	[JsonConverter(typeof(ModelResponseConverter))]
	public sealed class ModelResponse : AbstractClickshareResponse<string>
	{
	}

	public sealed class ModelResponseConverter : AbstractClickshareResponseConverter<ModelResponse, string>
	{
	}
}
