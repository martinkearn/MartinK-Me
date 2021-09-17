﻿using MartinKMe.Domain.Interfaces;
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
        private readonly IUtilityService _utilityService;

        public GithubService(IHttpClientFactory httpClientFactory, IUtilityService utilityService)
        {
            _clientFactory = httpClientFactory;
            _utilityService = utilityService;
        }

        public async Task<FileNameContents> GetFileContents(string fileApiUrl)
        {
            // Get github file api response
            var filePayload = await this.GetGithubFilePayload(fileApiUrl);

            // Prepare response
            var decodedContents = _utilityService.Base64Decode(filePayload.Content);
            var fileNameContents = new FileNameContents()
            {
                FileName = filePayload.Name,
                FileContents = decodedContents,
            };

            // Return
            return fileNameContents;
        }

        public async Task<Uri> GetFileUri(string fileApiUrl)
        {
            // Get github file api response
            var filePayload = await this.GetGithubFilePayload(fileApiUrl);

            // Return
            return new Uri(filePayload.HtmlUrl);
        }

        private async Task<GithubFilePayload> GetGithubFilePayload(string fileApiUrl)
        {
            // Make request to Github
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(fileApiUrl);
            client.DefaultRequestHeaders.Add("User-Agent", "Martink.me - GetFileContentsActivity");
            var response = await client.GetAsync(fileApiUrl);
            response.EnsureSuccessStatusCode();

            // Read and decode contents
            var rawContents = await response.Content.ReadAsStringAsync();
            var objContents = JsonConvert.DeserializeObject<GithubFilePayload>(rawContents);

            // Return
            return objContents;
        }
    }
}
