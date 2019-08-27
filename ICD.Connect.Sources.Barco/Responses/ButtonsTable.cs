using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Json;
using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses
{
	[JsonConverter(typeof(ButtonsTableConverter))]
	public sealed class ButtonsTable
	{
		private readonly Dictionary<int, Button> m_Buttons;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ButtonsTable()
		{
			m_Buttons = new Dictionary<int, Button>();
		}

		public IEnumerable<KeyValuePair<int, Button>> GetButtons()
		{
			return m_Buttons.ToArray();
		}

		public void AddButton(int key, Button value)
		{
			m_Buttons.Add(key, value);
		}
	}

	public sealed class ButtonsTableConverter : AbstractGenericJsonConverter<ButtonsTable>
	{
		/*
		{
			"1": {
				"Connected": false,
				"ConnectionCount": 5,
				"FirmwareVersion": "02.09.05.0022",
				"IpAddress": "",
				"LastConnected": "2019-06-18T18:34:47",
				"LastPaired": "2019-06-18T17:51:16",
				"MacAddress": "00:23:A7:63:CC:10",
				"SerialNumber": "1871012116",
				"Status": "OK"
			}
		}
		*/

		/// <summary>
		/// Override to handle the current property value with the given name.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="reader"></param>
		/// <param name="instance"></param>
		/// <param name="serializer"></param>
		protected override void ReadProperty(string property, JsonReader reader, ButtonsTable instance, JsonSerializer serializer)
		{
			int key = int.Parse(property);
			Button value = serializer.Deserialize<Button>(reader);

			instance.AddButton(key, value);
		}
	}
}
