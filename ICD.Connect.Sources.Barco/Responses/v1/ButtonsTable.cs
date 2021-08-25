#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Json;
using ICD.Connect.Sources.Barco.Responses.Common;

namespace ICD.Connect.Sources.Barco.Responses.v1
{
	[JsonConverter(typeof(ButtonsTableConverter))]
	public sealed class ButtonsTable : IButtonsCollection
	{
		private readonly IcdHashSet<Button> m_Buttons;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ButtonsTable()
		{
			m_Buttons = new IcdHashSet<Button>();
		}

		public IEnumerable<Button> GetButtons()
		{
			return m_Buttons.ToArray();
		}

		public void AddButton(Button value)
		{
			m_Buttons.Add(value);
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
			int id = int.Parse(property);
			Button value = serializer.Deserialize<Button>(reader);
			value.Id = id;

			instance.AddButton(value);
		}
	}
}
