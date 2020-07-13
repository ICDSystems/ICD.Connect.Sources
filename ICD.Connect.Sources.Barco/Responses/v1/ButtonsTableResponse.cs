using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses.v1
{
	[JsonConverter(typeof(ButtonsTableResponseConverter))]
	public sealed class ButtonsTableResponse : AbstractClickshareApiV1Response<ButtonsTable>
	{
	}

	public sealed class ButtonsTableResponseConverter : AbstractClickshareApiV1ResponseConverter<ButtonsTableResponse, ButtonsTable>
	{
	}
}
