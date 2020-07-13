using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Collections;
using ICD.Connect.Sources.Barco.Responses.Common;
using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses.v2
{
	[JsonConverter(typeof(ButtonsConverter))]
	public sealed class Buttons : AbstractBarcoClickshareApiV2Response, IButtonsCollection
	{
		private readonly IcdHashSet<Button> m_Buttons;

		/// <summary>
		/// Constructor.
		/// </summary>
		public Buttons()
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

	public sealed class ButtonsConverter : AbstractBarcoClickshareApiV2ResponseConverter<Buttons>
	{
		/*
		[
			{
				"id": 2,
				"connected": false,
				"connectionCount": 8,
				"firmwareVersion": "04.05.00.0002",
				"ipAddress": "",
				"lastConnected": "2020-06-26T10:28:17",
				"lastPaired": "2020-06-26T10:28:48",
				"macAddress": "3C:E1:A1:0D:40:74",
				"serialNumber": "1860013460",
				"status": "Ok"
			},
			{
				"id": 4,
				"connected": false,
				"connectionCount": 2,
				"firmwareVersion": "04.05.00.0002",
				"ipAddress": "",
				"lastConnected": "2020-06-25T16:39:26",
				"lastPaired": "2020-06-25T16:39:26",
				"macAddress": "3C:E1:A1:0D:2F:54",
				"serialNumber": "1860010642",
				"status": "Ok"
			}
		]
		*/

		/// <summary>
		/// Override to handle the current property value with the given name.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="reader"></param>
		/// <param name="instance"></param>
		/// <param name="serializer"></param>
		protected override void ReadProperty(string property, JsonReader reader, Buttons instance, JsonSerializer serializer)
		{
			Button value = serializer.Deserialize<Button>(reader);
			instance.AddButton(value);
		}
	}
}