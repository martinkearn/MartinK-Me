using Azure;
using Azure.Data.Tables;
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

            // Get settings from environment variables if they have not loaded from
            if (_settings == null)
            {
                _settings = new Settings()
                {
                    GithubPat = Environment.GetEnvironmentVariable("Settings:GithubPat"),
                    StorageConnectionString = Environment.GetEnvironmentVariable("Settings:StorageConnectionString"),
                };
            }

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

        [Fact(Timeout = 300000)] // 5 minute timeout
        public async Task Functions_AddArticle_CreatesBlobAndArticleEntity()
        {
            // Upload markdown file to Github
            var testRun = Guid.NewGuid();
            var blobFileName = $"integrationtest-{testRun}";
            var gitHubPath = $"Blogs/Tests/{blobFileName}.md";
            var gitHubPathBytes = Encoding.UTF8.GetBytes(gitHubPath.ToLowerInvariant());
            var uniqueTestMessage = $"Integration test content for test run {testRun}";
            var uniqueTestMessageBytes = Encoding.UTF8.GetBytes(uniqueTestMessage);
            var testArticleContentsBase64 = $"LS0tCnRpdGxlOiBJbnRlZ3JhdGlvbiBUZXN0aW5nCmF1dGhvcjogTWFydGluIEtlYXJuCmRlc2NyaXB0aW9uOiBNeSB0ZXN0IGFydGljbGUKaW1hZ2U6IGh0dHBzOi8vZHVtbXlpbWFnZS5jb20vODAweDYwMC8wMDAvZmZmJnRleHQ9cGxhY2Vob2xkZXIKdGh1bWJuYWlsOiBodHRwczovL2R1bW15aW1hZ2UuY29tLzIwMHgyMDAvMDAwL2ZmZiZ0ZXh0PXBsYWNlaG9sZGVyCnR5cGU6IGFydGljbGUKc3RhdHVzOiBkcmFmdApwdWJsaXNoZWQ6IDIwMjEvMDkvMjIgMDk6MzA6MDAKY2F0ZWdvcmllczogCiAgLSBUZXN0aW5nCi0tLQoKLlRoaXMgYXJ0aWNsZSBpcyBqdXN0IGZvciB0ZXN0aW5nIHB1cnBvc2VzCgojIyBIZXJlIGlzIGEgSDIgdG8gbWFrZSBzdXJlIEhUTUwgcGFyc2luZyBpcyB3b3JraW5nCi0gQW5kCi0gSGVyZQotIEFyZQotIFNvbWUKLSBCdWxsZXRzCgojIyMgSDMKU29tZSBleHRyYSB0ZXh0{Convert.ToBase64String(uniqueTestMessageBytes)}";
            var putBody = new ContentPutBody()
            {
                 Content = testArticleContentsBase64,
                 Message = uniqueTestMessage
            };
            var content = new StringContent(JsonSerializer.Serialize(putBody));
            var response = await _httpClient.PutAsync($"https://api.github.com/repos/martinkearn/content/contents/{gitHubPath}", content);
            response.EnsureSuccessStatusCode();

            // Setup Azure Storage clients
            var blobContainerClient = new BlobContainerClient(_settings.StorageConnectionString, "articleblobs");
            var blobClient = blobContainerClient.GetBlobClient($"tests/{blobFileName}.html");
            var tableClient = new TableClient(_settings.StorageConnectionString, "articles");

            // Assert in a loop with 30 second delays until test times out
            do
            {
                if (blobClient.Exists())
                {
                    Assert.True(blobClient.Exists());

                    TableEntity entity;
                    try
                    {
                        entity = tableClient.GetEntity<TableEntity>("article", Convert.ToBase64String(gitHubPathBytes), default);
                        Assert.Equal("Integration Testing", entity["title"]);
                        Assert.Equal(blobClient.Uri.AbsoluteUri, entity["htmlblobpath"]);
                        break;
                    }
                    catch (RequestFailedException ex)
                    { 
                        // We dont do anything with the exception, let the loop carry on
                    }
                }

                // Wait 30 seconds
                await Task.Delay(new TimeSpan(0, 0, 30));
            }
            while (true);
        }
    }
}