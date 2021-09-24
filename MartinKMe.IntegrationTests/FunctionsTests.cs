using MartinKMe.IntegrationTests.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MartinKMe.IntegrationTests
{
    public class FunctionsTests
    {
        private readonly HttpClient _httpClient;
        private readonly Settings _settings;

        public FunctionsTests()
        {
            // Read GithubPat form user secrets
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Settings>()
                .Build();
            _settings = configuration.GetSection(nameof(Settings)).Get<Settings>();

            // Setup http client
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.GithubPat}");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "MartinKMe.IntegrationTests");
        }

        [Fact]
        public async Task Functions_AddArticle_StoresBlobAndArticle()
        {
            // Arrange
            var response = await _httpClient.GetAsync("https://api.github.com/user/repos");
            var str = await response.Content.ReadAsStringAsync();
            
            // Assert
            Assert.NotEmpty(str);
        }

        [Fact]
        public void Hello()
        {
            var hello = "hello";
            Assert.Equal("hello", hello);
        }
    }
}