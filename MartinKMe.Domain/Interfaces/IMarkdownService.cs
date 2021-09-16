namespace MartinKMe.Domain.Interfaces
{
    public interface IMarkdownService
    {
        /// <summary>
        /// Converts Markdown to a Base64 encoded html string
        /// </summary>
        /// <param name="markdown">Markdown to convert</param>
        /// <returns>Base64 encoded html string</returns>
        string MarkdownToBase64Html(string markdown);
    }
}
