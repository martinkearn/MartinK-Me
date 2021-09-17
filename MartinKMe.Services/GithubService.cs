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
        private readonly HttpClient _client;
        private readonly IUtilityService _utilityService;

        public GithubService(IHttpClientFactory httpClientFactory, IUtilityService utilityService)
        {
            _client = httpClientFactory.CreateClient();
            _utilityService = utilityService;
        }

        public async Task<FileNameContents> GetFileContents(string fileApiUrl)
        {
            // Make request to Github
            _client.BaseAddress = new Uri(fileApiUrl);
            _client.DefaultRequestHeaders.Add("User-Agent", "Martink.me - GetFileContentsActivity");
            var response = await _client.GetAsync(fileApiUrl);
            response.EnsureSuccessStatusCode();

            // Read and decode contents
            string rawContents = await response.Content.ReadAsStringAsync();
            GithubFilePayload objContents = JsonConvert.DeserializeObject<GithubFilePayload>(rawContents);

            // Prepare response Tuple
            var decodedContents = _utilityService.Base64Decode(objContents.Content);
            var fileNameContents = new FileNameContents()
            {
                FileName = objContents.Name,
                FileContents = decodedContents,
            };

            // Return
            return fileNameContents;
        }
    }
}
