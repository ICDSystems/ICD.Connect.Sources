#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using ICD.Connect.Sources.Barco.Responses.v1;
using NUnit.Framework;

namespace ICD.Connect.Sources.Barco.Tests.Responses
{
	[TestFixture]
	public sealed class SharingStateResponseConverterTest
	{
		[Test]
		public void DeserializeTest()
		{
			const string data = @"		{
			""status"": 200,
			""message"": ""GET successful"",
			""data"": {
				""key"": ""/v1.12/DeviceInfo/Sharing"",
				""value"": true
			}
		}";

			SharingStateResponse response = JsonConvert.DeserializeObject<SharingStateResponse>(data);

			Assert.AreEqual(true, response.Data.Value);
		}
	}
}
