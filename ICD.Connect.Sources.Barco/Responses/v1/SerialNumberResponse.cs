#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif

namespace ICD.Connect.Sources.Barco.Responses.v1
{
	[JsonConverter(typeof(SerialNumberResponseConverter))]
	public sealed class SerialNumberResponse : AbstractClickshareApiV1Response<string>
	{
	}

	public sealed class SerialNumberResponseConverter : AbstractClickshareApiV1ResponseConverter<SerialNumberResponse, string>
	{
	}
}