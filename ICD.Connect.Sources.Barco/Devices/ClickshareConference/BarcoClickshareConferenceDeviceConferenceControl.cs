﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.DialContexts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.IncomingCalls;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.Sources.Barco.Responses.v2;

namespace ICD.Connect.Sources.Barco.Devices.ClickshareConference
{
	public sealed class BarcoClickshareConferenceDeviceConferenceControl : AbstractConferenceDeviceControl<BarcoClickshareConferenceDevice, TraditionalConference>
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

		/// <summary>
		/// Raised when a conference is added to the dialing control.
		/// </summary>
		public override event EventHandler<ConferenceEventArgs> OnConferenceAdded;

		/// <summary>
		/// Raised when a conference is removed from the dialing control.
		/// </summary>
		public override event EventHandler<ConferenceEventArgs> OnConferenceRemoved;

		#endregion

		private readonly IcdHashSet<UsbPeripheral> m_ConferencePeripherals;
		private readonly SafeCriticalSection m_ConferencePeripheralsSection;

		[CanBeNull]
		private TraditionalConference m_ActiveConference;

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

			SupportedConferenceFeatures = eConferenceFeatures.None;
		}

		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			Unsubscribe(Parent);
		}

		#endregion

		#region Methods

		public override IEnumerable<TraditionalConference> GetConferences()
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

		#endregion

		#region Private Methods

		private void UpdateConferenceState()
		{
			if (m_ActiveConference == null && AnyUsbPeripheralsInUse())
				StartConference();

			else if (m_ActiveConference != null && m_ActiveConference.IsActive() && !AnyUsbPeripheralsInUse())
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

			ThinTraditionalParticipant participant = new ThinTraditionalParticipant
			{
				HangupCallback = HangupParticipant
			};
			participant.SetName("Clickshare Conference");
			participant.SetAnswerState(eCallAnswerState.Answered);
			participant.SetCallType(DetermineCallTypeFromPeripherals());
			participant.SetDialTime(now);
			participant.SetStart(now);
			participant.SetStatus(eParticipantStatus.Connected);

			m_ActiveConference = new TraditionalConference();
			m_ActiveConference.AddParticipant(participant);

			OnConferenceAdded.Raise(this, new ConferenceEventArgs(m_ActiveConference));
		}

		private void EndConference()
		{
			if (m_ActiveConference == null)
				return;

			m_ActiveConference.Hangup();

			var endedConference = m_ActiveConference;
			m_ActiveConference = null;

			OnConferenceRemoved.Raise(this, new ConferenceEventArgs(endedConference));
		}

		private void TransitionCallTypeToAudio()
		{
			if (m_ActiveConference != null)
				m_ActiveConference.GetParticipants()
				                  .Cast<ThinTraditionalParticipant>()
				                  .ForEach(p => p.SetCallType(eCallType.Audio));
		}

		private void TransitionCallTypeToVideo()
		{
			if (m_ActiveConference != null)
				m_ActiveConference.GetParticipants()
								  .Cast<ThinTraditionalParticipant>()
				                  .ForEach(p => p.SetCallType(eCallType.Video));
		}

		/// <summary>
		/// Helper method to check if any USB peripherals are in use.
		/// </summary>
		/// <returns></returns>
		private bool AnyUsbPeripheralsInUse()
		{
			return AnyCamerasInUse() || AnyMicrophonesInUse() || AnySpeakersInUse();
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
		/// Flattens all speaker peripherals and checks if any are being used.
		/// </summary>
		/// <returns></returns>
		private bool AnySpeakersInUse()
		{
			return m_ConferencePeripheralsSection.Execute(() => m_ConferencePeripherals
			                                                    .SelectMany(p => p.Speakers)
			                                                    .Any(s => s.InUse));
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

		private void HangupParticipant(ThinTraditionalParticipant participant)
		{
			participant.SetStatus(eParticipantStatus.Disconnected);
			participant.SetEnd(IcdEnvironment.GetUtcTime());

			if (m_ActiveConference != null)
				m_ActiveConference.RemoveParticipant(participant);
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
	}
}