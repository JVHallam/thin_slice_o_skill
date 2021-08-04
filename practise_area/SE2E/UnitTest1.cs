using System;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;

namespace SE2E
{
    public class UnitTest1
    {
        public class Details{
            public string Name;
        }
        
        HttpClient _client;
        string _baseUrl;
        string controllerName = "My";

        public UnitTest1(){
            _baseUrl = "https://localhost:5001";

            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
            {
                return true;
            };

            httpClientHandler.AllowAutoRedirect = false;

            _client = new HttpClient(httpClientHandler) { BaseAddress = new Uri(_baseUrl) };
        }
        
        [Fact]
        public async Task MyController_Get_Success()
        {
            var response = await _client.GetAsync($"/{controllerName}");

            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine( body );

            Assert.Equal( (int)response.StatusCode, (int)HttpStatusCode.OK );
        }

        [Fact]
        public async Task MyController_PostModel_Success()
        {
            //Lets go ahead and post a model
            var model = new Details{
                Name = "jake"
            };

            var detailsAsJObject = JObject.FromObject(model);
            var postBody = detailsAsJObject.ToString();

            Console.WriteLine( postBody );

            HttpContent content = new StringContent( postBody, Encoding.UTF8, "application/json" );

            var response = await _client.PostAsync( $"{controllerName}", content );

            Console.WriteLine( await response.Content.ReadAsStringAsync() );

            Assert.Equal( (int)HttpStatusCode.OK, (int)response.StatusCode );
        }

        [Fact]
        public async Task MyController_PostNullBody_Fails()
        {
            var content = new StringContent( "", Encoding.UTF8, "application/json" );

            var response = await _client.PostAsync( $"{controllerName}", content );

            Console.WriteLine( await response.Content.ReadAsStringAsync() );

            Assert.Equal( (int)HttpStatusCode.BadRequest, (int)response.StatusCode );
        }
    }
}
