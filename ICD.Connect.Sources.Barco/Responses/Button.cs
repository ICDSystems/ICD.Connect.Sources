using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses
{
	/// <summary>
	/// Represents a button registered with the Barco Clickshare.
	/// </summary>
	[JsonConverter(typeof(ButtonConverter))]
	public class Button
	{
		public enum eStatus
		{
			[UsedImplicitly] Ok,
			[UsedImplicitly] Warning,
			[UsedImplicitly] Error
		}

		#region Properties

		/// <summary>
		/// Gets the connected state.
		/// </summary>
		public bool Connected { get; set; }

		/// <summary>
		/// Gets the number of times the button has connected to the Clickshare.
		/// </summary>
		public int ConnectionCount { get; set; }

		/// <summary>
		/// Gets the firmware version of the button.
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		/// Gets the IP address of the button.
		/// </summary>
		public string Ip { get; set; }

		/// <summary>
		/// Gets the time the button last connected to the Clickshare.
		/// </summary>
		[PublicAPI]
		public DateTime? LastConnected { get; set; }

		/// <summary>
		/// Gets the time the button was last paired with the Clickshare.
		/// </summary>
		[PublicAPI]
		public DateTime? LastPaired { get; set; }

		/// <summary>
		/// Gets the MAC address of the button.
		/// </summary>
		public string MacAddress { get; set; }

		/// <summary>
		/// Gets the serial number of the button.
		/// </summary>
		public string SerialNumber { get; set; }

		/// <summary>
		/// Gets the status of the button.
		/// </summary>
		public eStatus Status { get; set; }

		#endregion
	}

	public sealed class ButtonConverter : AbstractGenericJsonConverter<Button>
	{
		/*
		{
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
		*/

		private const string PROP_CONNECTED = "Connected";
		private const string PROP_CONNECTION_COUNT = "ConnectionCount";
		private const string PROP_FIRMWARE_VERSION = "FirmwareVersion";
		private const string PROP_IP_ADDRESS = "IpAddress";
		private const string PROP_LAST_CONNECTED = "LastConnected";
		private const string PROP_LAST_PAIRED = "LastPaired";
		private const string PROP_MAC_ADDRESS = "MacAddress";
		private const string PROP_SERIAL_NUMBER = "SerialNumber";
		private const string PROP_STATUS = "Status";

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter"/> can write JSON.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter"/> can write JSON; otherwise, <c>false</c>.
		/// </value>
		public override bool CanWrite { get { return false; } }

		/// <summary>
		/// Override to handle the current property value with the given name.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="reader"></param>
		/// <param name="instance"></param>
		/// <param name="serializer"></param>
		protected override void ReadProperty(string property, JsonReader reader, Button instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case PROP_CONNECTED:
					instance.Connected = reader.GetValueAsBool();
					break;
				case PROP_CONNECTION_COUNT:
					instance.ConnectionCount = reader.GetValueAsInt();
					break;
				case PROP_FIRMWARE_VERSION:
					instance.Version = reader.GetValueAsString();
					break;
				case PROP_IP_ADDRESS:
					instance.Ip = reader.GetValueAsString();
					break;
				case PROP_LAST_CONNECTED:
					instance.LastConnected = reader.GetValueAsDateTime();
					break;
				case PROP_LAST_PAIRED:
					instance.LastPaired = reader.GetValueAsDateTime();
					break;
				case PROP_MAC_ADDRESS:
					instance.MacAddress = reader.GetValueAsString();
					break;
				case PROP_SERIAL_NUMBER:
					instance.SerialNumber = reader.GetValueAsString();
					break;
				case PROP_STATUS:
					instance.Status = reader.GetValueAsEnum<Button.eStatus>();
					break;
				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
