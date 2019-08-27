using System;
using ICD.Common.Utils.Json;
using ICD.Connect.Sources.Barco.Responses;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ICD.Connect.Sources.Barco.Tests.Responses
{
	[TestFixture]
	public sealed class ButtonConverterTest
	{
		[Test]
		public void DeserializeTest()
		{
			const string data = @"{
	""Connected"": false,
			""ConnectionCount"": 5,
			""FirmwareVersion"": ""02.09.05.0022"",
			""IpAddress"": """",
			""LastConnected"": ""2019-06-18T18:34:47"",
			""LastPaired"": ""2019-06-18T17:51:16"",
			""MacAddress"": ""00:23:A7:63:CC:10"",
			""SerialNumber"": ""1871012116"",
			""Status"": ""OK""
		}";

			Button button = JsonConvert.DeserializeObject<Button>(data);

			Assert.AreEqual(button.Connected, false);
			Assert.AreEqual(button.ConnectionCount, 5);
			Assert.AreEqual(button.Version, "02.09.05.0022");
			Assert.AreEqual(button.Ip, string.Empty);
			Assert.AreEqual(button.LastConnected, new DateTime(2019, 06, 18, 18, 34, 47));
			Assert.AreEqual(button.LastPaired, new DateTime(2019, 06, 18, 17, 51, 16));
			Assert.AreEqual(button.MacAddress, "00:23:A7:63:CC:10");
			Assert.AreEqual(button.SerialNumber, "1871012116");
			Assert.AreEqual(button.Status, Button.eStatus.Ok);
		}
	}
}
