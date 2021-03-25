using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Nodes;
using ICD.Connect.Conferencing.BYOD;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Settings;
using ICD.Connect.Sources.Barco.API;
using ICD.Connect.Sources.Barco.Responses.v2;

namespace ICD.Connect.Sources.Barco.Devices.ClickshareConference
{
	public sealed class BarcoClickshareConferenceDevice : AbstractBarcoClickshareDevice<BarcoClickshareConferenceDeviceSettings>, IByodHubDevice
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

		/// <summary>
		/// Destination device for cameras routed through a BYOD Hub Device.
		/// </summary>
		public IDevice CameraDestinationDevice { get; set; }

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

		#region Settings

		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			CameraDestinationDevice = null;
		}

		protected override void CopySettingsFinal(BarcoClickshareConferenceDeviceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.CameraDestinationDevice = CameraDestinationDevice == null ? (int?)null : CameraDestinationDevice.Id;
		}

		protected override void ApplySettingsFinal(BarcoClickshareConferenceDeviceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			if (settings.CameraDestinationDevice == null)
				return;

			try
			{
				CameraDestinationDevice = factory.GetDeviceById(settings.CameraDestinationDevice.Value);
			}
			catch (KeyNotFoundException)
			{
				Logger.Log(eSeverity.Error, "No device with id {0}", settings.CameraDestinationDevice);
			}
		}

		#endregion

		#region Console

		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("CameraDestinationDevice", CameraDestinationDevice);
		}

		#endregion
	}
}