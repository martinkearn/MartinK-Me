using MartinKMe.Domain.Interfaces;
using MartinKMe.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MartinKMe.Services
{
    public class GithubService : IGithubService
    {
        private readonly IHttpClientFactory _clientFactory;

        public GithubService(IHttpClientFactory httpClientFactory)
        {
            _clientFactory = httpClientFactory;
        }

        public async Task<GithubContent> GetGithubContent(string fileApiUrl)
        {
            // Make request to Github
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(fileApiUrl);
            client.DefaultRequestHeaders.Add("User-Agent", "Martink.me - GetFileContentsActivity");
            var response = await client.GetAsync(fileApiUrl);
            response.EnsureSuccessStatusCode();

            // Read and decode contents
            var rawContents = await response.Content.ReadAsStringAsync();
            var objContents = JsonConvert.DeserializeObject<GithubContent>(rawContents);

            // Return
            return objContents;
        }
    }
}
