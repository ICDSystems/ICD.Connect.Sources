using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.API.Nodes;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.DialContexts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.IncomingCalls;
using ICD.Connect.Conferencing.Participants.Enums;
using ICD.Connect.Sources.Barco.Responses.v2;

namespace ICD.Connect.Sources.Barco.Devices.ClickshareConference
{
	public sealed class BarcoClickshareConferenceDeviceConferenceControl : AbstractConferenceDeviceControl<BarcoClickshareConferenceDevice, ThinConference>
	{
		#region Events

		/// <summary>
		/// Raised when an incoming call is added to the dialing control.
		/// </summary>
		public override event EventHandler<GenericEventArgs<IIncomingCall>> OnIncomingCallAdded;

		/// <summary>
		/// Raised when an incoming call is removed from the dialing control.
		/// </summary>
		public override event EventHandler<GenericEventArgs<IIncomingCall>> OnIncomingCallRemoved;

		public override event EventHandler<ConferenceEventArgs> OnConferenceAdded;
		public override event EventHandler<ConferenceEventArgs> OnConferenceRemoved;

		#endregion

		private readonly IcdHashSet<UsbPeripheral> m_ConferencePeripherals;
		private readonly SafeCriticalSection m_ConferencePeripheralsSection;

		[CanBeNull]
		private ThinConference m_ActiveConference;

		#region Properties

		/// <summary>
		/// Gets the type of conference this dialer supports.
		/// </summary>
		public override eCallType Supports { get { return eCallType.Audio | eCallType.Video; } }

		#endregion

		#region Constructors

		public BarcoClickshareConferenceDeviceConferenceControl(BarcoClickshareConferenceDevice parent, int id) 
			: base(parent, id)
		{
			m_ConferencePeripheralsSection = new SafeCriticalSection();
			m_ConferencePeripherals = new IcdHashSet<UsbPeripheral>();

			Subscribe(parent);

			SupportedConferenceControlFeatures = eConferenceControlFeatures.None;
		}

		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			Unsubscribe(Parent);
		}

		#endregion

		#region Methods

		public override IEnumerable<ThinConference> GetConferences()
		{
			if (m_ActiveConference != null)
				yield return m_ActiveConference;
		}

		public override eDialContextSupport CanDial(IDialContext dialContext)
		{
			return eDialContextSupport.Unsupported;
		}

		public override void Dial(IDialContext dialContext)
		{
			throw new NotSupportedException();
		}

		public override void SetDoNotDisturb(bool enabled)
		{
			throw new NotSupportedException();
		}

		public override void SetAutoAnswer(bool enabled)
		{
			throw new NotSupportedException();
		}

		public override void SetPrivacyMute(bool enabled)
		{
			throw new NotSupportedException();
		}

		public override void SetCameraMute(bool mute)
		{
			throw new NotSupportedException();
		}

		public override void StartPersonalMeeting()
		{
			throw new NotSupportedException();
		}

		public override void EnableCallLock(bool enabled)
		{
			throw new NotSupportedException();
		}

		#endregion

		#region Private Methods

		private void UpdateConferenceState()
		{
			if (m_ActiveConference == null && AnyCamerasOrMicrophonesInUse())
				StartConference();

			else if (m_ActiveConference != null && m_ActiveConference.IsActive() && !AnyCamerasOrMicrophonesInUse())
				EndConference();

			else if (m_ActiveConference != null && m_ActiveConference.CallType.HasFlag(eCallType.Video) && !AnyCamerasInUse())
				TransitionCallTypeToAudio();

			else if (m_ActiveConference != null && !m_ActiveConference.CallType.HasFlag(eCallType.Video) && AnyCamerasInUse())
				TransitionCallTypeToVideo();
		}

		private void StartConference()
		{
			EndConference();

			DateTime now = IcdEnvironment.GetUtcTime();

			ThinConference conference = new ThinConference
			{
				Name = "Clickshare Conference",
				AnswerState = eCallAnswerState.Answered,
				CallType = DetermineCallTypeFromPeripherals(),
				DialTime = now,
				StartTime = now,
				Status = eConferenceStatus.Connected
			};

			m_ActiveConference = conference;

			OnConferenceAdded.Raise(this, conference);
		}

		private void EndConference()
		{
			if (m_ActiveConference == null)
				return;

			m_ActiveConference.EndTime = IcdEnvironment.GetUtcTime();
			m_ActiveConference.Status = eConferenceStatus.Disconnected;

			ThinConference endedConference = m_ActiveConference;
			m_ActiveConference = null;

			OnConferenceRemoved.Raise(this, endedConference);
		}

		private void TransitionCallTypeToAudio()
		{
			if (m_ActiveConference != null)
				m_ActiveConference.CallType = eCallType.Audio;
		}

		private void TransitionCallTypeToVideo()
		{
			if (m_ActiveConference != null)
				m_ActiveConference.CallType = eCallType.Video;
		}

		private bool AnyCamerasOrMicrophonesInUse()
		{
			return AnyCamerasInUse() || AnyMicrophonesInUse();
		}

		/// <summary>
		/// Flattens all camera peripherals and checks if any are being used.
		/// </summary>
		/// <returns></returns>
		private bool AnyCamerasInUse()
		{
			return m_ConferencePeripheralsSection.Execute(() => m_ConferencePeripherals
			                                                    .SelectMany(p => p.Cameras)
			                                                    .Any(c => c.InUse));
		}

		/// <summary>
		/// Flattens all microphone peripherals and checks if any are being used.
		/// </summary>
		/// <returns></returns>
		private bool AnyMicrophonesInUse()
		{
			return m_ConferencePeripheralsSection.Execute(() => m_ConferencePeripherals
			                                                    .SelectMany(p => p.Microphones)
			                                                    .Any(m => m.InUse));
		}

		/// <summary>
		/// Determines the call type based on peripherals status.
		/// Camera in use = video.
		/// Microphone in use = audio.
		/// Both in use = video.
		/// Neither in use returns unknown.
		/// </summary>
		/// <returns></returns>
		private eCallType DetermineCallTypeFromPeripherals()
		{
			return AnyCamerasInUse() ? eCallType.Video : AnyMicrophonesInUse() ? eCallType.Audio : eCallType.Unknown;
		}

		#endregion

		#region Parent Callbacks

		protected override void Subscribe(BarcoClickshareConferenceDevice parent)
		{
			base.Subscribe(parent);

			if (parent == null)
				return;

			parent.OnPeripheralsChanged += ParentOnPeripheralsChanged;
		}

		protected override void Unsubscribe(BarcoClickshareConferenceDevice parent)
		{
			base.Unsubscribe(parent);

			if (parent == null)
				return;

			parent.OnPeripheralsChanged -= ParentOnPeripheralsChanged;
		}

		private void ParentOnPeripheralsChanged(object sender, GenericEventArgs<UsbPeripheral[]> e)
		{
			m_ConferencePeripheralsSection.Enter();

			try
			{
				m_ConferencePeripherals.Clear();
				m_ConferencePeripherals.AddRange(e.Data);
			}
			finally
			{
				m_ConferencePeripheralsSection.Leave();
			}

			UpdateConferenceState();
		}

		#endregion

		#region Console

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);
			addRow("Any Cameras In Use", AnyCamerasInUse());
			addRow("Any Microphones In Use", AnyMicrophonesInUse());
		}

		#endregion
	}
}