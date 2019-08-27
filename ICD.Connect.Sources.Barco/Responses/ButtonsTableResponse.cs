using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses
{
	[JsonConverter(typeof(ButtonsTableResponseConverter))]
	public sealed class ButtonsTableResponse : AbstractClickshareResponse<ButtonsTable>
	{
	}

	public sealed class ButtonsTableResponseConverter : AbstractClickshareResponseConverter<ButtonsTableResponse, ButtonsTable>
	{
	}
}
