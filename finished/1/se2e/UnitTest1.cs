using System;
using Xunit;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace se2e
{
    public class UnitTest1
    {
        string _baseUrl = "https://localhost:5001";
        HttpClient _client;

        public UnitTest1(){
            var httpClientHandler = new HttpClientHandler();

            httpClientHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

            httpClientHandler.AllowAutoRedirect = false;

            _client = new HttpClient(httpClientHandler) { BaseAddress = new Uri(_baseUrl) };
        }

        [Fact]
        public async Task Test1()
        {
            var response = await _client.GetAsync("");
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal( (int)HttpStatusCode.OK, (int)response.StatusCode);
            Assert.Equal( "Dingus", body );
        }
    }
}
