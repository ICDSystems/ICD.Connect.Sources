using System;
using System.Collections.Generic;
using ICD.Common.Logging.LoggingContexts;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Routing.Devices.Streaming;
using ICD.Connect.Settings;

namespace ICD.Connect.Sources.TvTuner.Streaming
{
	public sealed class StreamTvTunerDevice : AbstractDevice<StreamTvTunerDeviceSettings>, IStreamTvTunerDevice
	{
		#region Events

		/// <summary>
		/// Raised when the stream URI changes
		/// </summary>
		public event EventHandler<GenericEventArgs<Uri>> OnStreamUriChanged;

		#endregion

		[CanBeNull]
		private Uri m_StreamUri;

		#region Properties

		/// <summary>
		/// URI of the stream
		/// </summary>
		public Uri StreamUri
		{
			get { return m_StreamUri; }
			private set
			{
				if (m_StreamUri == value)
					return;

				m_StreamUri = value;
				Logger.LogSetTo(eSeverity.Informational, "Stream Uri", m_StreamUri);
				OnStreamUriChanged.Raise(this, new GenericEventArgs<Uri>(m_StreamUri));
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Override to add controls to the device.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		/// <param name="addControl"></param>
		protected override void AddControls(StreamTvTunerDeviceSettings settings, IDeviceFactory factory, Action<IDeviceControl> addControl)
		{
			base.AddControls(settings, factory, addControl);

			addControl(new StreamSourceDeviceRoutingControl(this, 0));
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return true;
		}

		/// <summary>
		/// Goes to the given  channel stream
		/// </summary>
		/// <param name="uri"></param>
		[PublicAPI]
		public void SetStreamUri(Uri uri)
		{
			StreamUri = uri;
		}

		#endregion

		#region Console

		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("StreamUri", StreamUri);
		}

		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand baseConsoleCommand in GetBaseConsoleCommands())
				yield return baseConsoleCommand;

			yield return new GenericConsoleCommand<string>("SetStreamUri", "Changes the stream uri",
														   s => SetStreamUri(new Uri(s)));
		}

		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}
