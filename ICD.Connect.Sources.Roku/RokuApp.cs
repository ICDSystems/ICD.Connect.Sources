using System.Collections.Generic;
using ICD.Common.Properties;
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

		#region Properties

		public int AppId { get; set; }
		public string SubType { get; set; }
		public string Type { get; set; }
		public string Version { get; set; }
		public string Name { get; set; }

		#endregion

		#region Factory Methods

		public static IEnumerable<RokuApp> ReadAppsFromXml(string xml)
		{
			return XmlUtils.ReadListFromXml<RokuApp>(xml, NAME_ELEMENT, ReadAppFromXml);
		}

		[CanBeNull]
		public static RokuApp ReadActiveAppFromXml(string xml)
		{
			foreach (string child in XmlUtils.GetChildElementsAsString(xml))
			{
				string unused;
				if (XmlUtils.TryGetAttribute(child, ID_ATTRIBUTE, out unused))
					return ReadAppFromXml(child);
			}

			return null;
		}

		[NotNull]
		public static RokuApp ReadAppFromXml(string xml)
		{
			return new RokuApp
			{
				AppId = XmlUtils.GetAttributeAsInt(xml, ID_ATTRIBUTE),
				Type = XmlUtils.GetAttribute(xml, TYPE_ATTRIBUTE),
				SubType = XmlUtils.GetAttribute(xml, SUBTYPE_ATTRIBUTE),
				Version = XmlUtils.GetAttributeAsString(xml, VERSION_ATTRIBUTE),
				Name = XmlUtils.ReadElementContent(xml)
			};
		}

		#endregion
	}
}
