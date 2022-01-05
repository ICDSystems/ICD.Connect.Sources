#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using System;
using System.Collections.Generic;
using ICD.Connect.Protocol.Network.Ports.Web;
using ICD.Connect.Sources.Barco.Responses.Common;
using ICD.Connect.Sources.Barco.Responses.v1;

namespace ICD.Connect.Sources.Barco.API
{
	public sealed class BarcoClickshareApiV1 : IBarcoClickshareApi
	{
		#region Constants

		private const int STATUS_SUCCESS = 200;
		//private const int STATUS_BAD_FORMAT = 400;
		//private const int STATUS_NOT_WRITABLE = 403;
		//private const int STATUS_BAD_PATH = 404;
		//private const int STATUS_ERROR = 500;

		private const string REQUEST_VERSION = "CurrentVersion";
		private const string KEY_BUTTONS_TABLE = "/Buttons/ButtonTable";
		private const string KEY_DEVICE_SHARING = "/DeviceInfo/Sharing";
		private const string KEY_SOFTWARE_VERSION = "/Software/FirmwareVersion";
		private const string KEY_DEVICE_MODEL = "/DeviceInfo/ModelName";
		private const string KEY_DEVICE_SERIAL = "/DeviceInfo/SerialNumber";
		private const string KEY_LAN = "/Network/Lan";
		private const string KEY_WLAN = "/Network/Wlan";

		private const string VERSION = "v1.0";

		#endregion

		#region Properties

		public string ApiVersion { get { return "1.0"; } }

		#endregion

		#region Polling

		public bool GetSharingState(IWebPort port)
		{
			return Poll<SharingStateResponse, bool>(port, VERSION + KEY_DEVICE_SHARING, r => r.Data.Value);
		}

		public Version GetVersion(IWebPort port)
		{
			return Poll<VersionResponse, Version>(port, REQUEST_VERSION, r => new Version(r.Data.Value.Replace("v", string.Empty)));
		}

		public Version GetSoftwareVersion(IWebPort port)
		{
			return Poll<SoftwareVersionResponse, Version>(port, VERSION + KEY_SOFTWARE_VERSION, r => new Version(r.Data.Value));
		}

		public IEnumerable<Button> GetButtonsTable(IWebPort port)
		{
			return Poll<ButtonsTableResponse, IEnumerable<Button>>(port, VERSION + KEY_BUTTONS_TABLE, r => r.Data.Value.GetButtons());
		}

		public string GetModel(IWebPort port)
		{
			return Poll<ModelResponse, string>(port, VERSION + KEY_DEVICE_MODEL, r => r.Data.Value);
		}

		public string GetSerialNumber(IWebPort port)
		{
			return Poll<SerialNumberResponse, string>(port, VERSION + KEY_DEVICE_SERIAL, r => r.Data.Value);
		}

		public LanInfo GetLan(IWebPort port)
		{
			return Poll<LanResponse, LanInfo>(port, VERSION + KEY_LAN, r => r.Data.Value);
		}

		public WlanInfo GetWlan(IWebPort port)
		{
			return Poll<WlanResponse, WlanInfo>(port, VERSION + KEY_WLAN, r => r.Data.Value);
		}

		/// <summary>
		/// Called when data is received from the physical device.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="relativeOrAbsoluteUri"></param>
		/// <param name="responseCallback"></param>
		private static TOutput Poll<TResponse, TOutput>(IWebPort port, string relativeOrAbsoluteUri, Func<TResponse, TOutput> responseCallback)
			where TResponse : IBarcoClickshareApiV1Response
		{
			if (responseCallback == null)
				throw new ArgumentNullException("responseCallback");

			WebPortResponse portResponse = port.Get(relativeOrAbsoluteUri, null);
			if (!portResponse.GotResponse)
				throw new Exception("Failed to make request");

			if (!portResponse.IsSuccessCode)
				throw new Exception("Request failed with code " + portResponse.StatusCode);

			string data = (portResponse.DataAsString ?? string.Empty).Trim();
			if (string.IsNullOrEmpty(data))
				throw new FormatException("Response is null or empty");

			TResponse response;

			try
			{
				response = JsonConvert.DeserializeObject<TResponse>(data);
			}
			catch (Exception e)
			{
				throw new FormatException("Failed to parse json - " + e.Message, e);
			}

			if (response.Status != STATUS_SUCCESS)
				throw new Exception(string.Format("Request failed - {0} - {1}", response.Status, response.Message));

			return responseCallback(response);
		}

		#endregion
	}
}