using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.EventArguments;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Routing;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Routing.EventArguments;

namespace ICD.Connect.Sources.Barco
{
	public sealed class BarcoClickshareRouteSourceControl<TClickshare, TClickshareSettings> :
		AbstractRouteSourceControl<TClickshare>
		where TClickshare : AbstractBarcoClickshareDevice<TClickshareSettings>
		where TClickshareSettings : AbstractBarcoClickshareDeviceSettings, new()
	{
		public override event EventHandler<TransmissionStateEventArgs> OnActiveTransmissionStateChanged;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		public BarcoClickshareRouteSourceControl(TClickshare parent, int id)
			: base(parent, id)
		{
			Subscribe(parent);
		}

		/// <summary>
		/// Override to release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			OnActiveTransmissionStateChanged = null;

			base.DisposeFinal(disposing);

			Unsubscribe(Parent);
		}

		#region Methods

		/// <summary>
		/// Returns true if the device is actively transmitting on the given output.
		/// This is NOT the same as sending video, since some devices may send an
		/// idle signal by default.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public override bool GetActiveTransmissionState(int output, eConnectionType type)
		{
			if (EnumUtils.HasMultipleFlags((int)type))
			{
				return EnumUtils.GetFlagsExceptNone(type)
				                .Select(f => GetActiveTransmissionState(output, f))
				                .Unanimous(false);
			}

			if (output != 1)
			{
				string message = string.Format("{0} has no {1} output at address {2}", this, type, output);
				throw new KeyNotFoundException(message);
			}

			switch (type)
			{
				case eConnectionType.Audio:
				case eConnectionType.Video:
					return Parent.Sharing;

				default:
					throw new ArgumentOutOfRangeException("type");
			}
		}

		/// <summary>
		/// Returns the outputs.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<ConnectorInfo> GetOutputs()
		{
			yield return new ConnectorInfo(1, eConnectionType.Audio | eConnectionType.Video);
		}

		#endregion

		#region Parent Callbacks

		/// <summary>
		/// Subscribe to parent events.
		/// </summary>
		/// <param name="parent"></param>
		private void Subscribe(TClickshare parent)
		{
			parent.OnSharingStatusChanged += ParentOnSharingStatusChanged;
		}

		/// <summary>
		/// Unsubscribe from parent events.
		/// </summary>
		/// <param name="parent"></param>
		private void Unsubscribe(TClickshare parent)
		{
			parent.OnSharingStatusChanged -= ParentOnSharingStatusChanged;
		}

		/// <summary>
		/// Called when the clickshare starts or stops sharing.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void ParentOnSharingStatusChanged(object sender, BoolEventArgs boolEventArgs)
		{
			TransmissionStateEventArgs args =
				new TransmissionStateEventArgs(1, eConnectionType.Audio | eConnectionType.Video, boolEventArgs.Data);
			OnActiveTransmissionStateChanged.Raise(this, args);
		}

		#endregion
	}
}
