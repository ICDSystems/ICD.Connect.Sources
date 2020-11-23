using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Settings;
using ICD.Connect.Sources.Barco.API;
using ICD.Connect.Sources.Barco.Responses.v2;

namespace ICD.Connect.Sources.Barco.Devices.ClickshareConference
{
	public sealed class BarcoClickshareConferenceDevice : AbstractBarcoClickshareDevice<BarcoClickshareConferenceDeviceSettings>
	{
		#region Events

		public event EventHandler<GenericEventArgs<UsbPeripheral[]>> OnPeripheralsChanged;

		#endregion

		#region Fields

		private readonly IcdHashSet<UsbPeripheral> m_Peripherals;
		private readonly SafeCriticalSection m_PeripheralsSection;

		#endregion

		#region Properties

		public new BarcoClickshareApiV2 Api
		{
			get
			{
				BarcoClickshareApiV2 apiV2 = base.Api as BarcoClickshareApiV2;
				if (apiV2 == null)
					throw new InvalidOperationException("Misconfigured Device - Check Device API version");

				return apiV2;
			}
		}

		#endregion

		#region Constructor

		public BarcoClickshareConferenceDevice()
		{
			m_PeripheralsSection = new SafeCriticalSection();
			m_Peripherals = new IcdHashSet<UsbPeripheral>();
		}

		#endregion

		#region Private Methods

		protected override void PollDevice()
		{
			// No port, or someone else is using it?
			if (Port == null || Port.Busy)
				return;

			try
			{
				bool oldSharing = Sharing;
				Sharing = Api.GetSharingState(Port);
				UpdatePeripherals(Api.GetPeripherals(Port));

				// If the sharing state changed or we've updated enough times, check the version and buttons table.
				if (Sharing != oldSharing || UpdateCount == 0)
				{
					Version = Api.GetVersion(Port);
					MonitoredDeviceInfo.FirmwareVersion = Api.GetSoftwareVersion(Port).ToString();
					UpdateButtons(Api.GetButtonsTable(Port));
					MonitoredDeviceInfo.Model = Api.GetModel(Port);
					MonitoredDeviceInfo.SerialNumber = Api.GetSerialNumber(Port);
					UpdateLanInfo(Api.GetLan(Port));
					UpdateWlanInfo(Api.GetWlan(Port));
				}

				UpdateCount = (UpdateCount + 1) % INFO_UPDATE_OCCURRENCE;
			}
			catch (Exception e)
			{
				Logger.Log(eSeverity.Error, "Error communicating with {0} - {1}", Port.Uri, e.Message);
				IncrementUpdateInterval();
				return;
			}

			ResetUpdateInterval();
		}

		private void UpdatePeripherals(IEnumerable<UsbPeripheral> peripherals)
		{
			bool changed;

			m_PeripheralsSection.Enter();

			try
			{
				IcdHashSet<UsbPeripheral> newPeripherals = peripherals.ToIcdHashSet();

				changed = !newPeripherals.SetEquals(m_Peripherals);

				if (changed)
				{
					m_Peripherals.Clear();
					m_Peripherals.AddRange(newPeripherals);
				}
			}
			finally
			{
				m_PeripheralsSection.Leave();
			}

			if (changed)
				OnPeripheralsChanged.Raise(this, new GenericEventArgs<UsbPeripheral[]>(m_Peripherals.ToArray()));
		}

		protected override void AddControls(BarcoClickshareConferenceDeviceSettings settings, IDeviceFactory factory, Action<IDeviceControl> addControl)
		{
			base.AddControls(settings, factory, addControl);

			addControl(new BarcoClickshareConferenceDeviceConferenceControl(this, 1));
		}

		#endregion
	}
}