using System;
using System.Globalization;
using ICD.Common.Properties;
using ICD.Common.Utils;
using Newtonsoft.Json.Linq;

namespace ICD.Connect.Sources.Barco
{
	/// <summary>
	/// Represents a button registered with the Barco Clickshare.
	/// </summary>
	public struct BarcoClickshareButton
	{
		// 2016-02-26T19:24:59
		private const string DATE_FORMAT = @"yyyy-MM-dd\THH:mm:ss";

		public enum eStatus
		{
			[UsedImplicitly] Ok,
			[UsedImplicitly] Warning,
			[UsedImplicitly] Error
		}

		private readonly bool m_Connected;
		private readonly int m_ConnectionCount;
		private readonly string m_Version;
		private readonly string m_Ip;
		private readonly DateTime? m_LastConnected;
		private readonly DateTime? m_LastPaired;
		private readonly string m_MacAddress;
		private readonly string m_SerialNumber;
		private readonly eStatus m_Status;

		#region Properties

		/// <summary>
		/// Gets the connected state.
		/// </summary>
		public bool Connected { get { return m_Connected; } }

		/// <summary>
		/// Gets the number of times the button has connected to the Clickshare.
		/// </summary>
		public int ConnectionCount { get { return m_ConnectionCount; } }

		/// <summary>
		/// Gets the firmware version of the button.
		/// </summary>
		public string Version { get { return m_Version; } }

		/// <summary>
		/// Gets the IP address of the button.
		/// </summary>
		public string Ip { get { return m_Ip; } }

		/// <summary>
		/// Gets the time the button last connected to the Clickshare.
		/// </summary>
		[PublicAPI]
		public DateTime? LastConnected { get { return m_LastConnected; } }

		/// <summary>
		/// Gets the time the button was last paired with the Clickshare.
		/// </summary>
		[PublicAPI]
		public DateTime? LastPaired { get { return m_LastPaired; } }

		/// <summary>
		/// Gets the MAC address of the button.
		/// </summary>
		public string MacAddress { get { return m_MacAddress; } }

		/// <summary>
		/// Gets the serial number of the button.
		/// </summary>
		public string SerialNumber { get { return m_SerialNumber; } }

		/// <summary>
		/// Gets the status of the button.
		/// </summary>
		public eStatus Status { get { return m_Status; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="connected"></param>
		/// <param name="connectionCount"></param>
		/// <param name="version"></param>
		/// <param name="ip"></param>
		/// <param name="lastConnected"></param>
		/// <param name="lastPaired"></param>
		/// <param name="macAddress"></param>
		/// <param name="serialNumber"></param>
		/// <param name="status"></param>
		private BarcoClickshareButton(bool connected, int connectionCount, string version, string ip, DateTime? lastConnected,
		                              DateTime? lastPaired, string macAddress, string serialNumber, eStatus status)
		{
			m_Connected = connected;
			m_ConnectionCount = connectionCount;
			m_Version = version;
			m_Ip = ip;
			m_LastConnected = lastConnected;
			m_LastPaired = lastPaired;
			m_MacAddress = macAddress;
			m_SerialNumber = serialNumber;
			m_Status = status;
		}

		/// <summary>
		/// Instantiates the button from JSON.
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		public static BarcoClickshareButton FromJson(string json)
		{
			JObject item = JObject.Parse(json);
			return FromJson(item);
		}

		/// <summary>
		/// Instantiates the button from JSON.
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		public static BarcoClickshareButton FromJson(JToken json)
		{
			bool connected = (bool)json.SelectToken("Connected");
			int connectionCount = (int)json.SelectToken("ConnectionCount");
			string version = (string)json.SelectToken("FirmwareVersion");
			string ip = (string)json.SelectToken("IpAddress");
			string lastConnectedString = (string)json.SelectToken("LastConnected");
			string lastPairedString = (string)json.SelectToken("LastPaired");
			string macAddress = (string)json.SelectToken("MacAddress");
			string serialNumber = (string)json.SelectToken("SerialNumber");
			string statusString = (string)json.SelectToken("Status");

			CultureInfo provider = CultureInfo.CurrentCulture;
			DateTime? lastConnected = lastConnectedString == null
				                          ? (DateTime?)null
				                          : DateTime.ParseExact(lastConnectedString, DATE_FORMAT, provider);
			DateTime? lastPaired = lastPairedString == null
				                       ? (DateTime?)null
				                       : DateTime.ParseExact(lastPairedString, DATE_FORMAT, provider);

			eStatus status = EnumUtils.Parse<eStatus>(statusString, true);

			return new BarcoClickshareButton(connected, connectionCount, version, ip, lastConnected, lastPaired, macAddress,
			                                 serialNumber, status);
		}

		#endregion
	}
}
