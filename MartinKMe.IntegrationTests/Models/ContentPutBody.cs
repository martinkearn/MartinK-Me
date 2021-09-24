using System.Text.Json.Serialization;

namespace MartinKMe.IntegrationTests.Models
{
    public class ContentPutBody
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
