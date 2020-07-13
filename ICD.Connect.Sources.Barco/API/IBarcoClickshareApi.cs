using System;
using ICD.Connect.Protocol.Network.Ports.Web;
using ICD.Connect.Sources.Barco.Responses.Common;

namespace ICD.Connect.Sources.Barco.API
{
	public interface IBarcoClickshareApi
	{
		bool GetSharingState(IWebPort port);

		Version GetVersion(IWebPort port);

		Version GetSoftwareVersion(IWebPort port);

		IButtonsCollection GetButtonsTable(IWebPort port);

		string GetModel(IWebPort port);

		string GetSerialNumber(IWebPort port);

		LanInfo GetLan(IWebPort port);

		WlanInfo GetWlan(IWebPort port);
	}
}
