#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Protocol.Network.Ports.Web;
using ICD.Connect.Sources.Barco.Responses.Common;
using ICD.Connect.Sources.Barco.Responses.v2;

namespace ICD.Connect.Sources.Barco.API
{
	public sealed class BarcoClickshareApiV2 : IBarcoClickshareApi
	{
		private const string REQUEST_VERSION = "CurrentVersion";
		private const string KEY_BUTTONS = "/configuration/buttons";
		private const string KEY_SHARING = "/configuration/system/status";
		private const string KEY_SOFTWARE_VERSION = "/configuration/system/updates/current";
		private const string KEY_DEVICE_IDENTITY = "/configuration/system/device-identity";
		private const string KEY_NETWORK = "/configuration/system/network";
		private const string KEY_PERIPHERALS = "/configuration/peripherals";

		private const string VERSION = "v2";

		public string ApiVersion{get { return "2.0"; }}


		public bool GetSharingState(IWebPort port)
		{
			return Poll<SystemStatusResponse, bool>(port, VERSION + KEY_SHARING, r => r.Sharing);
		}

		public Version GetVersion(IWebPort port)
		{
			return Poll<VersionResponse, Version>(port, REQUEST_VERSION, r => r.Data.Value);
		}

		public Version GetSoftwareVersion(IWebPort port)
		{
			return Poll<SoftwareVersionResponse, Version>(port, VERSION + KEY_SOFTWARE_VERSION, r => r.Version);
		}

		public IEnumerable<Button> GetButtonsTable(IWebPort port)
		{
			return Poll<Button[], Button[]>(port, VERSION + KEY_BUTTONS, r => r);
		}

		public string GetModel(IWebPort port)
		{
			return Poll<DeviceIdentityResponse, string>(port, VERSION + KEY_DEVICE_IDENTITY, r => r.ModelName);
		}

		public string GetSerialNumber(IWebPort port)
		{
			return Poll<DeviceIdentityResponse, string>(port, VERSION + KEY_DEVICE_IDENTITY, r => r.SerialNumber);
		}

		public LanInfo GetLan(IWebPort port)
		{
			return Poll<NetworkSettingsResponse, LanInfo>(port, VERSION + KEY_NETWORK, r => r.Wired.FirstOrDefault());
		}

		public WlanInfo GetWlan(IWebPort port)
		{
			return Poll<NetworkSettingsResponse, WlanInfo>(port, VERSION + KEY_NETWORK, r => r.Wireless.FirstOrDefault());
		}

		public IEnumerable<UsbPeripheral> GetPeripherals(IWebPort port)
		{
			return Poll<UsbPeripheral[], UsbPeripheral[]>(port, VERSION + KEY_PERIPHERALS, r => r);
		}

		/// <summary>
		/// Called when data is received from the physical device.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="relativeOrAbsoluteUri"></param>
		/// <param name="responseCallback"></param>
		private static TOutput Poll<TResponse, TOutput>(IWebPort port, string relativeOrAbsoluteUri, Func<TResponse, TOutput> responseCallback)
		{
			if (responseCallback == null)
				throw new ArgumentNullException("responseCallback");

			WebPortResponse portResponse;

			try
			{
				portResponse = port.Get(relativeOrAbsoluteUri);
			}
			catch (Exception e)
			{
				throw new Exception("Failed to make request - " + e.Message, e);
			}

			if (!portResponse.Success)
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

			return responseCallback(response);
		}
	}
}