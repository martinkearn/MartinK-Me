using System.Text.Json.Serialization;

namespace Domain.Models;

public class GithubFile
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("path")]
    public string Path { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("download_url")]
    public string DownloadUrl { get; set; }
}