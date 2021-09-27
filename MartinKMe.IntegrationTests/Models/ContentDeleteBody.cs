using System.Text.Json.Serialization;

namespace MartinKMe.IntegrationTests.Models
{
    /// <summary>
    /// Model used for the body for Delete request to Github repo api
    /// </summary>
    public class ContentDeleteBody
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("sha")]
        public string? Sha { get; set; }
    }
}
