using ICD.Common.Utils;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Protocol.Network.Ports.Web;
using ICD.Connect.Sources.Barco.Devices;
using ICD.Connect.Sources.Barco.Responses.Common;
#if SIMPLSHARP
using System;
using Crestron.SimplSharp;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Properties;

namespace ICD.Connect.Sources.Barco
{
	public delegate void DelBoolStatus(ushort value);

	public delegate void DelStringStatus(SimplSharpString value);

	public delegate void DelButtonStatus(
		ushort index, ushort connected, ushort connectionCount, SimplSharpString ip, SimplSharpString lastConnected,
		SimplSharpString lastPaired, SimplSharpString macAddress, SimplSharpString serialNumber, SimplSharpString status,
		SimplSharpString version);

	[PublicAPI("S+")]
	public sealed class SPlusBarcoClickshareInterface
	{
		#region Fields

		private BarcoClickshareDevice m_Clickshare;

		private HttpPort m_Port;

		#endregion

		#region Properties

		[PublicAPI("S+")]
		public SimplSharpString Host { get; set; }

		[PublicAPI("S+")]
		public SimplSharpString User { get; set; }

		[PublicAPI("S+")]
		public SimplSharpString Pass { get; set; }

		#endregion

		#region SPlus Delegates

		[PublicAPI("S+")]
		public DelBoolStatus SPlusSharingStatusChanged { get; set; }

		[PublicAPI("S+")]
		public DelBoolStatus SPlusOnlineStatusChanged { get; set; }

		[PublicAPI("S+")]
		public DelStringStatus SPlusVersionChanged { get; set; }

		[PublicAPI("S+")]
		public DelButtonStatus SPlusButtonStatusChanged { get; set; }

		[PublicAPI("S+")]
		public DelBoolStatus SPlusButtonsClearStatus { get; set; }

		#endregion

		#region SPlus Methods

		[PublicAPI("S+")]
		public void SPlusApplySettings()
		{
			if (Host == null || Host == new SimplSharpString())
				return;

			Unregister();

			m_Port = new HttpPort
			{
				Uri = new IcdUriBuilder(Host.ToString())
				{
					UserName = User.ToString(),
					Password = Pass.ToString()
				}.Uri
			};

			m_Clickshare = new BarcoClickshareDevice();

			m_Clickshare.OnSharingStatusChanged += ClickshareOnSharingStatusChanged;
			m_Clickshare.OnIsOnlineStateChanged += ClickshareOnIsOnlineStateChanged;
			m_Clickshare.OnButtonsChanged += ClickshareOnButtonsChanged;
			m_Clickshare.OnVersionChanged += ClickshareOnVersionChanged;

			m_Clickshare.SetPort(m_Port);
		}

		[PublicAPI("S+")]
		public void SPlusDestroyDevice()
		{
			Unregister();
		}

		[PublicAPI("S+")]
		public void SPlusSetHost(SimplSharpString host)
		{
			Host = host;
		}

		[PublicAPI("S+")]
		public void SPlusSetUser(SimplSharpString user)
		{
			User = user;
		}

		[PublicAPI("S+")]
		public void SPlusSetPass(SimplSharpString pass)
		{
			Pass = pass;
		}

		#endregion

		private void ClickshareOnVersionChanged(object sender, StringEventArgs stringEventArgs)
		{
			var delegateToFire = SPlusVersionChanged;
			if (delegateToFire != null)
				delegateToFire(new SimplSharpString(stringEventArgs.Data));
		}

		private void ClickshareOnButtonsChanged(object sender, EventArgs eventArgs)
		{
			var buttons = m_Clickshare.GetButtons();

			foreach (Button button in buttons)
			{
				var delegateToFire = SPlusButtonStatusChanged;
				if (delegateToFire == null)
					continue;
				string lastConnected = button.LastConnected.HasValue ? button.LastConnected.Value.ToString("s") : "";
				string lastPared = button.LastPaired.HasValue ? button.LastPaired.Value.ToString("s") : "";

				delegateToFire((ushort)button.Id, button.Connected ? (ushort)1 : (ushort)0,(ushort)button.ConnectionCount,
				               new SimplSharpString(button.Ip),
				               new SimplSharpString(lastConnected),
							   new SimplSharpString(lastPared),
				               new SimplSharpString(button.MacAddress),
				               new SimplSharpString(button.SerialNumber),
							   new SimplSharpString(button.Status.ToString()),
				               new SimplSharpString(button.Version));
			}
		}

		private void ClickshareOnIsOnlineStateChanged(object sender, DeviceBaseOnlineStateApiEventArgs args)
		{
			var delegateToFire = SPlusOnlineStatusChanged;

			if (delegateToFire != null)
				delegateToFire(args.Data ? (ushort)1 : (ushort)0);
		}

		private void ClickshareOnSharingStatusChanged(object sender, BoolEventArgs boolEventArgs)
		{
			var delegateToFire = SPlusSharingStatusChanged;

			if (delegateToFire != null)
				delegateToFire(boolEventArgs.Data ? (ushort)1 : (ushort)0);
		}

		private void ClearButtonFeedback()
		{
			var delegateToFire = SPlusButtonsClearStatus;

			if (delegateToFire != null)
				delegateToFire(1);
		}

		private void Unregister()
		{
			if (m_Clickshare != null)
			{
				m_Clickshare.OnSharingStatusChanged -= ClickshareOnSharingStatusChanged;
				m_Clickshare.OnIsOnlineStateChanged -= ClickshareOnIsOnlineStateChanged;
				m_Clickshare.OnButtonsChanged -= ClickshareOnButtonsChanged;
				m_Clickshare.OnVersionChanged -= ClickshareOnVersionChanged;
				m_Clickshare.Dispose();

				ClickshareOnSharingStatusChanged(this, new BoolEventArgs(false));
				ClickshareOnIsOnlineStateChanged(this, new DeviceBaseOnlineStateApiEventArgs(false));
				ClickshareOnVersionChanged(this, new StringEventArgs(""));
				ClearButtonFeedback();

			}
			m_Clickshare = null;

			if (m_Port != null)
			{
				m_Port.Dispose();
			}
			m_Port = null;
		}
	}
}
#endif
