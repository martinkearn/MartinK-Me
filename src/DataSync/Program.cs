using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Domain.Models;
using System.Text;
using System.Text.Json;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Configuration;

namespace DataSync;

class Program
{
    static async Task Main(string[] args)
    {
        // Setup configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())  // Set the base path
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // Load default appsettings.json
            .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true) // Load development-specific settings
            .AddEnvironmentVariables(); // Optionally add environment variables
        IConfiguration configuration = builder.Build();
        var functionUrl = configuration["FunctionUrl"]!;
        var gitHubPat = configuration["GitHubPAT"]!;
        
        // Get GH blogs
        var files = await GetGithubFiles("martinkearn", "Content", "Blogs", gitHubPat); // These values ARE case senitive
        Console.WriteLine($"Got {files.Count} files from GitHub");
        foreach (var file in files)
        {
            Console.WriteLine($"Processing File: {file.Path}");
            
            // Get Commit
            var commit = await GetGithubLastCommit("martinkearn", "Content", file.Path, gitHubPat);
            
            // Create Fixture
            var fixture = CreateFixture($"Updated {file.Path}", commit.Url, file.Path);

            // Send to Function
            if (functionUrl != null) await CallFunction(functionUrl, fixture);
            
            await Task.Delay(2000);  // Pause for 2 seconds
            
            Console.WriteLine($"Processed File: {file.Path}");
        }
        
        Console.WriteLine("COMPLETED");
    }
    

    private static GithubPushWebhookPayload CreateFixture(string message, string commitUrl, string modifiedPath)
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var ghWh = fixture.Create<GithubPushWebhookPayload>();
        ghWh.Repository.Name = "Content";
        ghWh.HeadCommit.Message = message;
        ghWh.HeadCommit.Url = commitUrl;
        ghWh.HeadCommit.Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz");
        ghWh.HeadCommit.Author = new Author()
        {
            Name = "Martin Kearn",
            Email = "martin.kearn@microsoft.com",
            Username = "martinkearn"
        };
        ghWh.HeadCommit.Added = [];
        ghWh.HeadCommit.Removed = [];
        ghWh.HeadCommit.Modified = [modifiedPath];
        var commit = new Commit()
        {
            Id = ghWh.HeadCommit.Id,
            TreeId = ghWh.HeadCommit.TreeId,
            Distinct = ghWh.HeadCommit.Distinct,
            Message = ghWh.HeadCommit.Message,
            Timestamp = Convert.ToDateTime(ghWh.HeadCommit.Timestamp),
            Url = ghWh.HeadCommit.Url,
            Author = ghWh.HeadCommit.Author,
            Committer = ghWh.HeadCommit.Committer,
            Added = [],
            Removed = [],
            Modified = ghWh.HeadCommit.Modified
        };
        ghWh.Commits =
        [
            commit
        ];

        return ghWh;
    }

    private static async Task<List<GithubFile>> GetGithubFiles(string repoOwner, string repoName, string folderPath, string pat)
    {
        var url = $"https://api.github.com/repos/{repoOwner}/{repoName}/contents/{folderPath}";
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", pat);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; dotnet)"); //(GitHub requires this)
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var files = JsonSerializer.Deserialize<GithubFile[]>(responseBody);

        return files!.ToList();
    }
    
    private static async Task<Commit> GetGithubLastCommit(string repoOwner, string repoName, string filePath, string pat)
    {
        var url = $"https://api.github.com/repos/{repoOwner}/{repoName}/commits?path={filePath}&sha=master";
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", pat);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; dotnet)"); //(GitHub requires this)
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var commits = JsonSerializer.Deserialize<Commit[]>(responseBody);

        return commits.FirstOrDefault();
    }

    private static async Task CallFunction(string functionUrl, GithubPushWebhookPayload data)
    {
        using var client = new HttpClient();
        var jsonData = JsonSerializer.Serialize(data);
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(functionUrl, content);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response received successfully:");
            Console.WriteLine(result);
        }
        else
        {
            Console.WriteLine($"Failed to send POST request. Status Code: {response.StatusCode}");
        }
    }
}