using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using NUnit.Framework;

namespace ICD.Connect.Sources.Roku.Tests
{
	[TestFixture]
	public sealed class RokuAppTest
	{

		[TestCase(@"<app id=""31012"" Type=""menu"" Version=""1.9.28"">FandangoNOW Movies &amp; TV</app>", 31012,
			"menu", null, "1.9.28", "FandangoNOW Movies & TV")]
		[TestCase(@"<app id=""12"" subtype=""ndka"" Type=""appl"" Version=""5.0.81121025"">Netflix</app>",
			12, "appl", "ndka", "5.0.81121025", "Netflix")]
		[TestCase(@"<app id=""13"" subtype=""ndka"" Type=""appl"" Version=""10.8.2019040917"">Prime Video</app>",
			13, "appl", "ndka", "10.8.2019040917", "Prime Video")]
		[TestCase(@"<app id=""2285"" subtype=""rsga"" Type=""appl"" Version=""6.17.1"">Hulu</app>", 2285,
			"appl", "rsga", "6.17.1", "Hulu")]
		[TestCase(@"<app id=""2213"" subtype=""sdka"" Type=""appl"" Version=""4.1.1609"">Roku Media Player</app>", 2213,
		"appl", "sdka", "4.1.1609", "Roku Media Player")]
		[TestCase(@"<app id=""184661"" subtype=""sdka"" Type=""appl"" Version=""1.0.38"">Roku Streaming Player Intro</app>", 184661,
			"appl", "sdka", "1.0.38", "Roku Streaming Player Intro")]
		[TestCase(@"<app id=""13535"" subtype=""rsga"" Type=""appl"" Version=""6.3.9"">Plex</app>", 13535,
			"appl", "rsga", "6.3.9", "Plex")]
		[TestCase(@"<app id=""837"" subtype=""ndka"" Type=""appl"" Version=""1.0.80000286"">YouTube</app>", 837,
			"appl", "ndka", "1.0.80000286", "YouTube")]
		public void ReadAppFromXmlTest(string xml, int id, string type, string subtype, string version, string name)
		{
			RokuApp app = RokuApp.ReadAppFromXml(xml);

			Assert.AreEqual(id, app.AppID);
			Assert.AreEqual(type, app.Type);
			Assert.AreEqual(subtype, app.SubType);
			Assert.AreEqual(version, app.Version);
			Assert.AreEqual(name, app.Name);
		}


		[Test]
		public void ReadAppFromXmlTest()
		{
			const string xml = @"<?xml Version=""1.0"" encoding=""UTF-8"" ?>
				<apps>
					<app id=""31012"" Type=""menu"" Version=""1.9.28"">FandangoNOW Movies &amp; TV</app>
					<app id=""12"" subtype=""ndka"" Type=""appl"" Version=""5.0.81121025"">Netflix</app>
					<app id=""13"" subtype=""ndka"" Type=""appl"" Version=""10.8.2019040917"">Prime Video</app>
					<app id=""2285"" subtype=""rsga"" Type=""appl"" Version=""6.17.1"">Hulu</app>
					<app id=""2213"" subtype=""sdka"" Type=""appl"" Version=""4.1.1609"">Roku Media Player</app>
					<app id=""184661"" subtype=""sdka"" Type=""appl"" Version=""1.0.38"">Roku Streaming Player Intro</app>
					<app id=""13535"" subtype=""rsga"" Type=""appl"" Version=""6.3.9"">Plex</app>
					<app id=""837"" subtype=""ndka"" Type=""appl"" Version=""1.0.80000286"">YouTube</app>
				</apps>";
			IEnumerable<RokuApp> apps = RokuApp.ReadAppsFromXml(xml);

			Assert.AreEqual(8,apps.Count());
		}
	}
}
