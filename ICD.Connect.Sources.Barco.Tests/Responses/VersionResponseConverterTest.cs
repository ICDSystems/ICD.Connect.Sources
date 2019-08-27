using ICD.Connect.Sources.Barco.Responses;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ICD.Connect.Sources.Barco.Tests.Responses
{
	[TestFixture]
	public sealed class VersionResponseConverterTest
	{
		[Test]
		public void DeserializeTest()
		{
			const string data = @"		{
			""status"": 200,
			""message"": ""GET successful"",
			""data"": {
				""key"": ""/CurrentVersion"",
				""value"": ""v1.12""
			}
		}";

			VersionResponse response = JsonConvert.DeserializeObject<VersionResponse>(data);

			Assert.AreEqual("v1.12", response.Data.Value);
		}
	}
}
