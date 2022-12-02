using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Test.Helpers;
using WebApi;

namespace WebApiTest.Integration
{
    public class AuthenticationControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;
        private readonly WebApplicationFactory<Program> _factory;

        public AuthenticationControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOk()
        {
            var request = new
            {
                Url = "/api/auth",
                Body = new
                {
                    UserName = "johndoe",
                    Password = "johndoe2410"
                }
            };

            // Act
            var response = await _httpClient.PostAsync(request.Url, HelperClass.ContentHelper.GetStringContent(request.Body));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithInValidCredentials_ReturnsUnauthorized()
        {
            //Assert
            var request = new
            {
                Url = "/api/auth",
                Body = new
                {
                    UserName = "anythingelse",
                    Password = "anythingelse2410"
                }
            };
            // Act
            var response = await _httpClient.PostAsync(request.Url, HelperClass.ContentHelper.GetStringContent(request.Body));

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
