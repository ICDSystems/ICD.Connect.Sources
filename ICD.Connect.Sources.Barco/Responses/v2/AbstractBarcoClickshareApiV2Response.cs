using ICD.Common.Utils.Json;

namespace ICD.Connect.Sources.Barco.Responses.v2
{
	public abstract class AbstractBarcoClickshareApiV2Response : IBarcoClickshareApiV2Response
	{
	}

	public abstract class AbstractBarcoClickshareApiV2ResponseConverter<TResponse> : AbstractGenericJsonConverter<TResponse>
		where TResponse : IBarcoClickshareApiV2Response
	{
		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON.
		/// </summary>
		/// <value><c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON; otherwise, <c>false</c>.</value>
		public override bool CanWrite { get { return false; } }
	}
}
