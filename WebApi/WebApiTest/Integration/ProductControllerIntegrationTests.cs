using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using Test.Helpers;
using WebApi;
using WebApi.Models.DataModels;

namespace WebApiTest.Integration
{
    public class ProductControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {

        private readonly HttpClient _httpClient;
        private readonly WebApplicationFactory<Program> _factory;
        public ProductControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
        }


        [Fact]
        public async Task GetAllProducts_WithoutBearerHeader_ReturnsUnauthorized()
        {

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "wrongToken");

            var response = await _httpClient.GetAsync("api/products");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllProducts_WithBearerHeader_ReturnsOk()
        {
            var token = await GetToken("johndoe", "johndoe2410");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/Products");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAllProducts_WhenExecuted_ReturnsListOfProducts()
        {
            var token = await GetToken("johndoe", "johndoe2410");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/products");
            var content = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(content);

            Assert.IsAssignableFrom<IEnumerable<Product>>(products);
        }


        [Theory, MemberData(nameof(Good))]
        public async Task GetAProduct_WithExistingId_ReturnsNotFound(Guid id)
        {
            var response = await _httpClient.GetAsync($"/api/products/{id}");

            var content = await response.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<Product>(content);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.IsType<Product>(product);
        }

        [Theory, MemberData(nameof(Bad))]
        public async Task GetAProduct_WithNonExistingId_ReturnsNotFound(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/products/{id}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private async Task<string?> GetToken(string userName, string password)
        {
            var request = new
            {
                Url = "/api/auth",
                Body = new
                {
                    UserName = userName,
                    Password = password
                }
            };
            var res = await _httpClient.PostAsync(request.Url, HelperClass.ContentHelper.GetStringContent(request.Body));

            if (!res.IsSuccessStatusCode) return null;

            var userModel = await res.Content.ReadAsStringAsync();
            if (userModel is null) return null;

            return JsonConvert.DeserializeObject<Tokeen>(userModel)?.Token ?? "";
        }

        static readonly Guid a = Guid.NewGuid();
        static readonly Guid b = Guid.NewGuid();
        static readonly Guid c = Guid.NewGuid();
        static readonly Guid d = Guid.NewGuid();

        public static IEnumerable<object[]> Good
        {
            get
            {
                yield return new object[] { a };
                yield return new object[] { b };
            }
        }
        public static IEnumerable<object[]> Bad
        {
            get
            {
                yield return new object[] { c };
                yield return new object[] { d };
            }
        }
    }
}
