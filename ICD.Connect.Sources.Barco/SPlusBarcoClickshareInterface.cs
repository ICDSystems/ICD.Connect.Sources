using System;
using Crestron.SimplSharp;
using ICD.Common.EventArguments;
using ICD.Connect.Protocol.Network.WebPorts.Https;

#if SIMPLSHARP

namespace ICD.Connect.Sources.Barco
{

	public delegate void DelBoolStatus(ushort value);

	public delegate void DelStringStatus(SimplSharpString value);

	public delegate void DelButtonStatus(
		ushort index, ushort connected, ushort connectionCount, SimplSharpString ip, SimplSharpString lastConnected,
		SimplSharpString lastPaired, SimplSharpString macAddress, SimplSharpString serialNumber, SimplSharpString status,
		SimplSharpString version);

	public class SPlusBarcoClickshareInterface
	{

		#region Fields
		private BarcoClickshareDevice m_Clickshare;

		private HttpsPort m_Port;

		#endregion


		#region Properties
		public SimplSharpString Host { get; set; }

		public SimplSharpString User { get; set; }

		public SimplSharpString Pass { get; set; }
		#endregion



		#region SPlus Delegates

		public DelBoolStatus SPlusSharingStatusChanged { get; set; }

		public DelBoolStatus SPlusOnlineStatusChanged { get; set; }

		public DelStringStatus SPlusVersionChanged { get; set; }

		public DelButtonStatus SPlusButtonStatusChanged { get; set; }

		public DelBoolStatus SPlusButtonsClearStatus { get; set; }

		#endregion


		#region SPlus Methods

		public void SPlusApplySettings()
		{
			if (Host == null || Host == new SimplSharpString())
				return;

			Unregister();

			m_Port = new HttpsPort
			{
				Address = Host.ToString(),
				Username = User.ToString(),
				Password = Pass.ToString()
			};

			m_Clickshare = new BarcoClickshareDevice();

			m_Clickshare.OnSharingStatusChanged += ClickshareOnSharingStatusChanged;
			m_Clickshare.OnIsOnlineStateChanged += ClickshareOnIsOnlineStateChanged;
			m_Clickshare.OnButtonsChanged += ClickshareOnButtonsChanged;
			m_Clickshare.OnVersionChanged += ClickshareOnVersionChanged;

			m_Clickshare.SetPort(m_Port);
		}

		public void SPlusDestroyDevice()
		{
			Unregister();
		}

		public void SPlusSetHost(SimplSharpString host)
		{
			Host = host;
		}

		public void SPlusSetUser(SimplSharpString user)
		{
			User = user;
		}

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

			foreach (var kvp in buttons)
			{
				var delegateToFire = SPlusButtonStatusChanged;
				if (delegateToFire == null)
					continue;
				string lastConnected = kvp.Value.LastConnected.HasValue ? kvp.Value.LastConnected.Value.ToString("s") : "";
				string lastPared = kvp.Value.LastPaired.HasValue ? kvp.Value.LastPaired.Value.ToString("s") : "";

				delegateToFire((ushort)kvp.Key, kvp.Value.Connected ? (ushort)1 : (ushort)0,(ushort)kvp.Value.ConnectionCount,
				               new SimplSharpString(kvp.Value.Ip),
				               new SimplSharpString(lastConnected),
							   new SimplSharpString(lastPared),
				               new SimplSharpString(kvp.Value.MacAddress),
				               new SimplSharpString(kvp.Value.SerialNumber),
							   new SimplSharpString(kvp.Value.Status.ToString()),
				               new SimplSharpString(kvp.Value.Version));
			}
		}

		private void ClickshareOnIsOnlineStateChanged(object sender, BoolEventArgs boolEventArgs)
		{
			var delegateToFire = SPlusOnlineStatusChanged;

			if (delegateToFire != null)
				delegateToFire(boolEventArgs.Data ? (ushort)1 : (ushort)0);
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
				ClickshareOnIsOnlineStateChanged(this, new BoolEventArgs(false));
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
