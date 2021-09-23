namespace MartinKMe.Domain.Interfaces
{
    public interface IMarkdownService
    {
        /// <summary>
        /// Converts Markdown to a html string
        /// </summary>
        /// <param name="markdown">Markdown to convert</param>
        /// <returns>Html string</returns>
        string MarkdownToHtml(string markdown);
    }
}
