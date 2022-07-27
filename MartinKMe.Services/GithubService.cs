using MartinKMe.Domain.Interfaces;
using MartinKMe.Domain.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace MartinKMe.Services
{
    /// <inheritdoc/>
    public class GithubService : IGithubService
    {
        private readonly IHttpClientFactory _clientFactory;

        public GithubService(IHttpClientFactory httpClientFactory)
        {
            _clientFactory = httpClientFactory;
        }

        public async Task<GithubContent> GetGithubContent(Uri fileApiUrl)
        {
            // Make request to Github
            var client = _clientFactory.CreateClient();
            client.BaseAddress = fileApiUrl;
            client.DefaultRequestHeaders.Add("User-Agent", "Martink.me - GetFileContentsActivity");
            var response = await client.GetAsync(fileApiUrl);
            response.EnsureSuccessStatusCode();

            // Read and decode contents
            var rawContents = await response.Content.ReadAsStringAsync();
            var objContents = JsonSerializer.Deserialize<GithubContent>(rawContents);

            // Return
            return objContents;
        }
    }
}
