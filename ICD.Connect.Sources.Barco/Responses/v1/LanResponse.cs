using ICD.Connect.Sources.Barco.Responses.Common;
using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses.v1
{
	[JsonConverter(typeof(LanResponseConverter))]
	public sealed class LanResponse : AbstractClickshareApiV1Response<LanInfo>
	{
	}

	public sealed class LanResponseConverter : AbstractClickshareApiV1ResponseConverter<LanResponse, LanInfo>
	{
	}
}
