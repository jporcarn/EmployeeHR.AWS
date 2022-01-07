using EmployeeHR.AWSSigner;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeHR.AWS.Controllers.Tests
{
    public class ValuesControllerTests
    {
        private readonly string _region = "eu-west-1";
        private readonly string _accessKeyId = "";
        private readonly string _secretAccessKey = "";
        private readonly Uri _baseAddress;

        public ValuesControllerTests()
        {

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            _accessKeyId = config.GetValue<string>("AWS:access_key_id");
            _secretAccessKey = config["AWS:secret_access_key"];
            _region = config["AWS:region"];
            _baseAddress = new Uri(config["Request:baseAddress"]);

        }


        [Theory()]
        [InlineData("Stage")]
        [InlineData("Prod")]
        public async Task GetValuesTest(string stage)
        {
            var client = new HttpClient(new AWSSignatureV4HttpInterceptor(_accessKeyId, _secretAccessKey, _region))
            {
                BaseAddress = this._baseAddress
            };

            HttpResponseMessage? response = null;

            try
            {
                var requestUri = $"{stage}/api/values";
                response = await client.GetAsync(requestUri);

                response.EnsureSuccessStatusCode();

                Assert.NotNull(response);
            }
            catch (HttpRequestException ex)
            {

                string message = ex.Message;
                if (response != null)
                {
                    message = await response.Content.ReadAsStringAsync();
                }

                Assert.False(true, message);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
                // throw;
            }
        }

    }
}