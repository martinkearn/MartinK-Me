using System.Text.Json.Serialization;

namespace MartinKMe.IntegrationTests.Models
{
    /// <summary>
    /// Model used for the body for Put request to Github repo api
    /// </summary>
    public class ContentPutBody
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
