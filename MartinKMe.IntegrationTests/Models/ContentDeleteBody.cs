using System.Text.Json.Serialization;

namespace MartinKMe.IntegrationTests.Models
{
    public class ContentDeleteBody
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("sha")]
        public string? Sha { get; set; }
    }
}
