using System.Linq;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Sources.Barco.Responses.Common;
using ICD.Connect.Sources.Barco.Responses.v1;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ICD.Connect.Sources.Barco.Tests.Responses
{
	[TestFixture]
	public sealed class ButtonsTableConverterTest
	{
		[Test]
		public void DeserializeTest()
		{
			const string data = @"{
	""1"": {
			""Connected"": false,
			""ConnectionCount"": 5,
			""FirmwareVersion"": ""02.09.05.0022"",
			""IpAddress"": """",
			""LastConnected"": ""2019-06-18T18:34:47"",
			""LastPaired"": ""2019-06-18T17:51:16"",
			""MacAddress"": ""00:23:A7:63:CC:10"",
			""SerialNumber"": ""1871012116"",
			""Status"": ""OK""
		}
	}";

			ButtonsTable collection = JsonConvert.DeserializeObject<ButtonsTable>(data);

			IcdHashSet<Button> set = collection.GetButtons().ToIcdHashSet();

			Assert.AreEqual(1, set.Count);
			Assert.AreEqual(1, set.First().Id);
		}
	}
}
