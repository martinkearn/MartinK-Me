using Newtonsoft.Json;

namespace MartinKMe.Domain.Models
{
    /// <summary>
    /// Class used to deserialise the output of the Github Content API for a given file.
    /// </summary>
    public class GithubContent
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("sha")]
        public string Sha { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }

        [JsonProperty("git_url")]
        public string GitUrl { get; set; }

        [JsonProperty("download_url")]
        public string DownloadUrl { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("encoding")]
        public string Encoding { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }
    }

    /// <summary>
    /// Used for the Links property of GithubContent
    /// </summary>
    public class Links
    {
        [JsonProperty("self")]
        public string Self { get; set; }

        [JsonProperty("git")]
        public string Git { get; set; }

        [JsonProperty("html")]
        public string Html { get; set; }
    }
}
