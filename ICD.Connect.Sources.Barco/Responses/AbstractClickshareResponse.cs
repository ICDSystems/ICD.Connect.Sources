using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using Newtonsoft.Json;

namespace ICD.Connect.Sources.Barco.Responses
{
	public abstract class AbstractClickshareResponse<T> : IBarcoClickshareResponse<T>
	{
		public int Status { get; set; }
		public string Message { get; set; }
		public KeyValuePair<string, T> Data { get; set; }

		KeyValuePair<string, object> IBarcoClickshareResponse.Data
		{
			get { return new KeyValuePair<string, object>(Data.Key, Data.Value); }
			set { Data = new KeyValuePair<string, T>(value.Key, (T)value.Value); }
		}
	}

	public abstract class AbstractClickshareResponseConverter<TResponse, TData> : AbstractGenericJsonConverter<TResponse>
		where TResponse : IBarcoClickshareResponse<TData>
	{
		/*
		{
			"status": 200,
			"message": "GET successful",
			"data": {
				"key": "/CurrentVersion",
				"value": "v1.12"
			}
		}
		*/

		private const string PROP_STATUS = "status";
		private const string PROP_MESSAGE = "message";
		private const string PROP_DATA = "data";
		private const string PROP_KEY = "key";
		private const string PROP_VALUE = "value";

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter"/> can write JSON.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter"/> can write JSON; otherwise, <c>false</c>.
		/// </value>
		public override bool CanWrite { get { return false; } }

		/// <summary>
		/// Override to handle the current property value with the given name.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="reader"></param>
		/// <param name="instance"></param>
		/// <param name="serializer"></param>
		protected override void ReadProperty(string property, JsonReader reader, TResponse instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case PROP_STATUS:
					instance.Status = reader.GetValueAsInt();
					break;

				case PROP_MESSAGE:
					instance.Message = reader.GetValueAsString();
					break;

				case PROP_DATA:
					reader.ReadObject(serializer, (p, r, s) => ReadData(p, r, instance, s));
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}

		/// <summary>
		/// Reads the data key-value pair.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="reader"></param>
		/// <param name="instance"></param>
		/// <param name="serializer"></param>
		private void ReadData(string property, JsonReader reader, TResponse instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case PROP_KEY:
					string key = reader.GetValueAsString();
					instance.Data = new KeyValuePair<string, TData>(key, instance.Data.Value);
					break;

				case PROP_VALUE:
					TData value = serializer.Deserialize<TData>(reader);
					instance.Data = new KeyValuePair<string, TData>(instance.Data.Key, value);
					break;

				default:
					reader.Skip();
					break;
			}
		}
	}
}
