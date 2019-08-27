using ICD.Connect.Sources.Barco.Responses;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ICD.Connect.Sources.Barco.Tests.Responses
{
	[TestFixture]
	public sealed class SoftwareVersionResponseConverterTest
	{
		[Test]
		public void DeserializeTest()
		{
			const string data = @"		{
			""status"": 200,
			""message"": ""GET successful"",
			""data"": {
				""key"": ""/v1.12/Software/FirmwareVersion"",
				""value"": ""01.07.00.0022""
			}
		}";

			SoftwareVersionResponse response = JsonConvert.DeserializeObject<SoftwareVersionResponse>(data);

			Assert.AreEqual("01.07.00.0022", response.Data.Value);
		}
	}
}
