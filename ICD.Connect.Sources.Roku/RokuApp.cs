using System.Collections.Generic;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Sources.Roku
{
	public sealed class RokuApp
	{
		private const string ID_ATTRIBUTE = "id";
		private const string SUBTYPE_ATTRIBUTE = "subtype";
		private const string TYPE_ATTRIBUTE = "type";
		private const string VERSION_ATTRIBUTE = "version";
		private const string NAME_ELEMENT = "app";


		public int AppID { get; set; }
		public string SubType { get; set; }
		public string Type { get; set; }
		public string Version { get; set; }
		public string Name { get; set; }

		public static IEnumerable<RokuApp> ReadAppsFromXml(string xml)
		{
			return XmlUtils.ReadListFromXml(xml, "app", ReadAppFromXml);
		}

		public static RokuApp ReadAppFromXml(string xml)
		{
			return new RokuApp
			{
				AppID = XmlUtils.GetAttributeAsInt(xml, ID_ATTRIBUTE),
				Type = XmlUtils.GetAttribute(xml, TYPE_ATTRIBUTE),
				SubType = XmlUtils.GetAttribute(xml, SUBTYPE_ATTRIBUTE),
				Version = XmlUtils.GetAttributeAsString(xml, VERSION_ATTRIBUTE),
				Name = XmlUtils.ReadElementContent(xml)
			};
		}
	}
}
