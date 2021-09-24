using Azure.Storage.Blobs;
using MartinKMe.IntegrationTests.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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

        [Fact(Timeout = 180000)]
        public async Task Functions_AddArticle_CreatesBlobAndArticleEntity()
        {
            // Upload markdown file to Github
            var testRun = Guid.NewGuid();
            var blobFileName = $"integrationtest-{testRun}";
            var uniqueTestMessage = $"Integration test content for test run {testRun}";
            var uniqueTestMessageBytes = Encoding.UTF8.GetBytes(uniqueTestMessage);
            var testArticleContentsBase64 = $"LS0tCnRpdGxlOiBJbnRlZ3JhdGlvbiBUZXN0aW5nCmF1dGhvcjogTWFydGluIEtlYXJuCmRlc2NyaXB0aW9uOiBNeSB0ZXN0IGFydGljbGUKaW1hZ2U6IGh0dHBzOi8vZHVtbXlpbWFnZS5jb20vODAweDYwMC8wMDAvZmZmJnRleHQ9cGxhY2Vob2xkZXIKdGh1bWJuYWlsOiBodHRwczovL2R1bW15aW1hZ2UuY29tLzIwMHgyMDAvMDAwL2ZmZiZ0ZXh0PXBsYWNlaG9sZGVyCnR5cGU6IGFydGljbGUKc3RhdHVzOiBkcmFmdApwdWJsaXNoZWQ6IDIwMjEvMDkvMjIgMDk6MzA6MDAKY2F0ZWdvcmllczogCiAgLSBUZXN0aW5nCi0tLQoKLlRoaXMgYXJ0aWNsZSBpcyBqdXN0IGZvciB0ZXN0aW5nIHB1cnBvc2VzCgojIyBIZXJlIGlzIGEgSDIgdG8gbWFrZSBzdXJlIEhUTUwgcGFyc2luZyBpcyB3b3JraW5nCi0gQW5kCi0gSGVyZQotIEFyZQotIFNvbWUKLSBCdWxsZXRzCgojIyMgSDMKU29tZSBleHRyYSB0ZXh0{Convert.ToBase64String(uniqueTestMessageBytes)}";
            var putBody = new ContentPutBody()
            {
                 Content = testArticleContentsBase64,
                 Message = uniqueTestMessage
            };
            var content = new StringContent(JsonSerializer.Serialize(putBody));
            var response = await _httpClient.PutAsync($"https://api.github.com/repos/martinkearn/content/contents/Blogs/Tests/{blobFileName}.md", content);
            response.EnsureSuccessStatusCode();

            // Get blob in storage
            var blobContainerClient = new BlobContainerClient(_settings.StorageConnectionString, "articleblobs");
            var blobClient = blobContainerClient.GetBlobClient($"{blobFileName}.html");

            // Assert in a loop with 30 second delays until test times out
            do
            {
                if (blobClient.Exists())
                {
                    Assert.True(blobClient.Exists());
                    break;
                }

                // Wait 30 seconds
                await Task.Delay(new TimeSpan(0, 0, 30));
            }
            while (true);
        }
    }
}