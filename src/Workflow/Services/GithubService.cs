using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Workflow.Services
{
    /// <inheritdoc/>
    public class GithubService(
        IHttpClientFactory httpClientFactory,
        IOptions<GithubConfiguration> githubConfigurationOptions)
        : IGithubService
    {
        private readonly GithubConfiguration _options = githubConfigurationOptions.Value;

        public async Task<GithubContent> GetGithubContent(string fileApiUrl)
        {
            // Make request to Github
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(fileApiUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.Pat);
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
