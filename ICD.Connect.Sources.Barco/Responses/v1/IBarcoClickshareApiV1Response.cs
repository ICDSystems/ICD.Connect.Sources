using System.Collections.Generic;

namespace ICD.Connect.Sources.Barco.Responses.v1
{
	public interface IBarcoClickshareApiV1Response
	{
		int Status { get; set; }

		string Message { get; set; }

		KeyValuePair<string, object> Data { get; set; }
	}

	public interface IBarcoClickshareApiV1Response<T> : IBarcoClickshareApiV1Response
	{
		new KeyValuePair<string, T> Data { get; set; }
	}
}
