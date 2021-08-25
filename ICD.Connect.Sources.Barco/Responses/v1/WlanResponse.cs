#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using ICD.Connect.Sources.Barco.Responses.Common;

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
