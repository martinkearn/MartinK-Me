using System.Diagnostics.CodeAnalysis;

namespace MartinKMe.IntegrationTests.Models
{
    /// <summary>
    /// Model use to cast app settings to a stringly typed class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Settings
    {
        public string GithubPat { get; set; }

        public string StorageConnectionString { get; set; }
    }
}
