namespace Domain.Models
{
    /// <summary>
    /// Used to strongly type the "GithubConfiguration" appsettings section
    /// </summary>
    public class GithubConfiguration
    {
        /// <summary>
        /// PAT for acessing API.
        /// </summary>
        public string Pat { get; set; }
    }
}