using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses
{
	[JsonConverter(typeof(SerialNumberResponseConverter))]
	public sealed class SerialNumberResponse : AbstractClickshareResponse<string>
	{
	}

	public sealed class SerialNumberResponseConverter : AbstractClickshareResponseConverter<SerialNumberResponse, string>
	{
	}
}