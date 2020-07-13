using ICD.Connect.Sources.Barco.Responses.Common;
using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses.v1
{
	[JsonConverter(typeof(WlanResponseConverter))]
	public sealed class WlanResponse : AbstractClickshareApiV1Response<WlanInfo>
	{
	}

	public sealed class WlanResponseConverter : AbstractClickshareApiV1ResponseConverter<WlanResponse, WlanInfo>
	{
	}
}
