using System.Collections.Generic;

namespace ICD.Connect.Sources.Barco.Responses
{
	public interface IBarcoClickshareResponse
	{
		int Status { get; set; }

		string Message { get; set; }

		KeyValuePair<string, object> Data { get; set; }
	}

	public interface IBarcoClickshareResponse<T> : IBarcoClickshareResponse
	{
		new KeyValuePair<string, T> Data { get; set; }
	}
}
