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
        var functionUrl = configuration["FunctionUrl"];
        Console.WriteLine($"FunctionUrl:{functionUrl}");
        
        
        // Create Fixture
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var ghWh = fixture.Create<GithubPushWebhookPayload>();
        ghWh.Repository.Name = "Content";
        ghWh.HeadCommit.Message = "Update AI Services at Future Decoded 2017.md";
        ghWh.HeadCommit.Url = "https://github.com/martinkearn/Content/commit/56d556b7daf138231ee8dbd5c4489a095ccfabea";
        ghWh.HeadCommit.Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz");
        ghWh.HeadCommit.Author = new Author()
        {
            Name = "Martin Kearn",
            Email = "martin.kearn@microsoft.com",
            Username = "martinkearn"
        };
        ghWh.HeadCommit.Added = [];
        ghWh.HeadCommit.Removed = [];
        ghWh.HeadCommit.Modified = ["Blogs/AI%20Services%20at%20Future%20Decoded%202017.md"];
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

        // Send to Function
        using var client = new HttpClient();
        var jsonData = JsonSerializer.Serialize(ghWh);
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