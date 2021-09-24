using MartinKMe.IntegrationTests.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json.Nodes;
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
        public async Task GithubApi_GetContentRepo_SmokeTest()
        {
            // Act
            var response = await _httpClient.GetAsync($"https://api.github.com/repos/martinkearn/content");
            string resultJson = await response.Content.ReadAsStringAsync();
            JsonObject result = JsonNode.Parse(resultJson)?.AsObject();
            var name = (string)result?["name"];

            // Assert
            Assert.Equal("Content", name);
        }
    }
}