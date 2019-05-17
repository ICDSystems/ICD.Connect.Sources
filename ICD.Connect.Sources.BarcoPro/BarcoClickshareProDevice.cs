using System;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Sources.Barco;
#if SIMPLSHARP
using Crestron.SimplSharpPro.CrestronThread;
#else
using System.Threading;
#endif

namespace ICD.Connect.Sources.BarcoPro
{
	/// <summary>
	/// The BarcoClickshareProDevice handles device polling in a low priority thread to
	/// avoid stealing all CPU cycles.
	/// </summary>
	public sealed class BarcoClickshareProDevice : AbstractBarcoClickshareDevice<BarcoClickshareProDeviceSettings>
	{
		private Thread m_Thread;

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
#if SIMPLSHARP
			if (m_Thread != null)
				m_Thread.Abort();
#endif

			base.DisposeFinal(disposing);
		}

		protected override void PollDevice()
		{
			// Don't create multiple threads to poll at the same time.
			if (m_Thread != null)
				return;

#if SIMPLSHARP
			try
			{
				m_Thread = new Thread(ThreadCallback, null) { Priority = Thread.eThreadPriority.LowestPriority };
			}
			catch (Exception e)
			{
				Log(eSeverity.Error, "Failed to poll, Crestron Thread constructor threw a {0} - {1}", e.GetType(), e.Message);
			}
#else
			m_Thread = new Thread(() => ThreadCallback());
			m_Thread.Start();
#endif
		}

		private object ThreadCallback(object unused)
		{
			ThreadCallback();
			return null;
		}

		private void ThreadCallback()
		{
			base.PollDevice();
			m_Thread = null;
		}
	}
}
