#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using System.Linq;
using ICD.Common.Utils.Extensions;

namespace ICD.Connect.Sources.Barco.Responses.v2
{
	/// <summary>
	/// Represents a specific Usb Peripheral.
	/// </summary>
	[JsonConverter(typeof(PeripheralResponseConverter))]
	public sealed class UsbPeripheral : AbstractBarcoClickshareApiV2Response
	{
		public UsbPeripheralSubDevice[] Cameras { get; set; }

		public int Id { get; set; }

		public  UsbPeripheralSubDevice[] Microphones { get; set; }

		public string Name { get; set; }

		public bool PluggedIn { get; set; }

		public string SerialNumber { get; set; }

		public UsbPeripheralSubDevice[] Speakers { get; set; }

		public string Vendor { get; set; }
	}

	/// <summary>
	/// Represents a specific Usb device status (mic or camera or speaker).
	/// </summary>
	[JsonConverter(typeof(UsbPeripheralSubDeviceConverter))]
	public sealed class UsbPeripheralSubDevice : AbstractBarcoClickshareApiV2Response
	{
		public bool Capable { get; set; }

		public bool InUse { get; set; }

		public bool Muted { get; set; }
	}

	public sealed class PeripheralResponseConverter : AbstractBarcoClickshareApiV2ResponseConverter<UsbPeripheral>
	{
		private const string PROP_CAMERAS = "cameras";
		private const string PROP_ID = "id";
		private const string PROP_MICROPHONES = "microphones";
		private const string PROP_NAME = "name";
		private const string PROP_PLUGGED_IN = "pluggedIn";
		private const string PROP_SERIAL_NUMBER = "serialNumber";
		private const string PROP_SPEAKERS = "speakers";
		private const string PROP_VENDOR = "vendor";

		protected override void WriteProperties(JsonWriter writer, UsbPeripheral value, JsonSerializer serializer)
		{
			base.WriteProperties(writer, value, serializer);

			if (value.Cameras != null)
			{
				writer.WritePropertyName(PROP_CAMERAS);
				serializer.Serialize(writer, value.Cameras);
			}

			if (value.Id != 0)
			{
				writer.WritePropertyName(PROP_ID);
				serializer.Serialize(writer, value.Id);
			}

			if (value.Microphones != null)
			{
				writer.WritePropertyName(PROP_MICROPHONES);
				serializer.Serialize(writer, value.Microphones);
			}

			if (value.Name != null)
			{
				writer.WritePropertyName(PROP_NAME);
				serializer.Serialize(writer, value.Name);
			}

			if (value.PluggedIn)
			{
				writer.WritePropertyName(PROP_PLUGGED_IN);
				serializer.Serialize(writer, value.PluggedIn);
			}

			if (value.SerialNumber != null)
			{
				writer.WritePropertyName(PROP_SERIAL_NUMBER);
				serializer.Serialize(writer, value.SerialNumber);
			}

			if (value.Speakers != null)
			{
				writer.WritePropertyName(PROP_SPEAKERS);
				serializer.Serialize(writer, value.Speakers);
			}

			if (value.Vendor != null)
			{
				writer.WritePropertyName(PROP_VENDOR);
				serializer.Serialize(writer, value.Vendor);
			}
		}

		protected override void ReadProperty(string property, JsonReader reader, UsbPeripheral instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case PROP_CAMERAS:
					instance.Cameras = serializer.DeserializeArray<UsbPeripheralSubDevice>(reader).ToArray();
					break;
				case PROP_ID:
					instance.Id = reader.GetValueAsInt();
					break;
				case PROP_MICROPHONES:
					instance.Microphones = serializer.DeserializeArray<UsbPeripheralSubDevice>(reader).ToArray();
					break;
				case PROP_NAME:
					instance.Name = reader.GetValueAsString();
					break;
				case PROP_PLUGGED_IN:
					instance.PluggedIn = reader.GetValueAsBool();
					break;
				case PROP_SERIAL_NUMBER:
					instance.SerialNumber = reader.GetValueAsString();
					break;
				case PROP_SPEAKERS:
					instance.Speakers = serializer.DeserializeArray<UsbPeripheralSubDevice>(reader).ToArray();
					break;
				case PROP_VENDOR:
					instance.Vendor = reader.GetValueAsString();
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}

	public sealed class UsbPeripheralSubDeviceConverter : AbstractBarcoClickshareApiV2ResponseConverter<UsbPeripheralSubDevice>
	{
		private const string PROP_CAPABLE = "capable";
		private const string PROP_IN_USE = "inUse";
		private const string PROP_MUTED = "muted";

		protected override void WriteProperties(JsonWriter writer, UsbPeripheralSubDevice value, JsonSerializer serializer)
		{
			base.WriteProperties(writer, value, serializer);

			if (value.Capable)
			{
				writer.WritePropertyName(PROP_CAPABLE);
				serializer.Serialize(writer, value.Capable);
			}

			if (value.InUse)
			{
				writer.WritePropertyName(PROP_IN_USE);
				serializer.Serialize(writer, value.InUse);
			}

			if (value.Muted)
			{
				writer.WritePropertyName(PROP_MUTED);
				serializer.Serialize(writer, value.Muted);
			}
		}

		protected override void ReadProperty(string property, JsonReader reader, UsbPeripheralSubDevice instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case PROP_CAPABLE:
					instance.Capable = reader.GetValueAsBool();
					break;
				case PROP_IN_USE:
					instance.InUse = reader.GetValueAsBool();
					break;
				case PROP_MUTED:
					instance.Muted = reader.GetValueAsBool();
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}