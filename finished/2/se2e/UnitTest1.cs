using System;
using Xunit;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using Bogus;

namespace se2e
{
    public class Customer : IEquatable<Customer> {
        public string Name { get; set; }
        public int Age { get; set; }

        public bool Equals(Customer other){
            return (
                this.Name == other.Name
                && this.Age == other.Age
            );
        }
    }

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

        [Fact]
        public async Task TestController_PostValidObject_Success()
        {
            var customer = new Customer{
                Name = "testName",
                Age = 101
            };

            string customerAsString = JsonConvert.SerializeObject( customer );

            var context = new StringContent( customerAsString, Encoding.UTF8, "application/json" );

            var response = await _client.PostAsync("", context);
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal( (int)HttpStatusCode.Created, (int)response.StatusCode);
            Assert.True( Int32.TryParse(body, out int responseId ), "Body should be an int, representing the id");
            Assert.True( responseId > 0, "Returned ID should be greater than 0");
        }

        [Fact]
        public async Task TestController_PostAndRetrieveValidObject_Success()
        {
            var faker = new Faker<Customer>()
                            .RuleFor(c => c.Name, f => f.Name.FirstName())
                            .RuleFor(c => c.Age, f => f.Random.Number(1, 100));

            var customer = faker.Generate();

            string customerAsString = JsonConvert.SerializeObject( customer );

            var context = new StringContent( customerAsString, Encoding.UTF8, "application/json" );

            var response = await _client.PostAsync("", context);
            var body = await response.Content.ReadAsStringAsync();

            var id = Int32.Parse(body);

            var retrievedCustomerResponse = await _client.GetAsync($"/{id}");
            Assert.Equal( (int)HttpStatusCode.OK, (int)retrievedCustomerResponse.StatusCode );

            string responseBodyString = await retrievedCustomerResponse.Content.ReadAsStringAsync();
            var responseObj = JsonConvert.DeserializeObject<Customer>( responseBodyString );

            Assert.Equal( customer, responseObj );
        }
    }
}
