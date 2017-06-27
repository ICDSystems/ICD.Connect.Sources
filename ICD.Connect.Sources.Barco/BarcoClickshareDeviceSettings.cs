using System;

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
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(BarcoClickshareDevice); } }
	}
}
