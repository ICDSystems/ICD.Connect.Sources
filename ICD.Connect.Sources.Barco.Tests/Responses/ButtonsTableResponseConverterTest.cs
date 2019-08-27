using ICD.Connect.Sources.Barco.Responses;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ICD.Connect.Sources.Barco.Tests.Responses
{
	[TestFixture]
	public sealed class ButtonsTableResponseConverterTest
	{
		[Test]
		public void DeserializeTest()
		{
			const string data = @"{
  ""status"": 200,
			""message"": ""GET successful"",
			""data"": {
				""key"": ""/v1.12/Buttons/ButtonTable"",
				""value"": {
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
				}
			}
		}";

			ButtonsTableResponse response = JsonConvert.DeserializeObject<ButtonsTableResponse>(data);

			Assert.NotNull(response.Data.Value);
		}
	}
}
