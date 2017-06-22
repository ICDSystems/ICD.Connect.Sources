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

		protected override void PollDevice()
		{
			// Don't create multiple threads to poll at the same time.
			if (m_Thread != null)
				return;

#if SIMPLSHARP
			ThreadCallbackFunction callback =
				o =>
				{
					ThreadCallback();
					return null;
				};
			
			m_Thread = new Thread(callback, null, Thread.eThreadStartOptions.Running)
			{
				Priority = Thread.eThreadPriority.LowestPriority
			};
#else
			m_Thread = new Thread(ThreadCallback)
			{
				Priority = ThreadPriority.Lowest
			};
			m_Thread.Start();
#endif
		}

		private void ThreadCallback()
		{
			base.PollDevice();
			m_Thread = null;
		}
	}
}
