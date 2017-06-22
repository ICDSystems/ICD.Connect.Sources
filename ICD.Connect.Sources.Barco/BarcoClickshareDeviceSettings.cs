using ICD.Connect.Settings;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Sources.Barco
{
	public sealed class BarcoClickshareDeviceSettings : AbstractBarcoClickshareDeviceSettings
	{
		private const string FACTORY_NAME = "BarcoClickshareSimpl";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Creates a new originator instance from the settings.
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public override IOriginator ToOriginator(IDeviceFactory factory)
		{
			BarcoClickshareDevice output = new BarcoClickshareDevice();
			output.ApplySettings(this, factory);
			return output;
		}
	}
}
