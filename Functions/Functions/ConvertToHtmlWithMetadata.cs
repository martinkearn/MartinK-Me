using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Markdig;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Functions
{
    public static class ConvertToHtmlWithMetadata
    {
        [FunctionName("ConvertToHtmlWithMetadata")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("ConvertToHtmlWithMetadata function processed a request.");

            // get payload which is a URL to a Git Hub Repo API File object
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            if (!string.IsNullOrEmpty(requestBody))
            {
                // get raw file and extract YAML
                using (var client = new HttpClient())
                {
                    // setup HttpClient and get GitHub API response JSON and decode the content from base 64
                    client.BaseAddress = new Uri(requestBody);
                    client.DefaultRequestHeaders.Add("User-Agent", "MartinK.me ExtractYAML Function");
                    var gitHubFileString = await client.GetStringAsync(requestBody);

                    // deserialise JSON and decode Base64 content
                    dynamic gitHubFile = JsonConvert.DeserializeObject(gitHubFileString);
                    var fileContent = Helpers.Base64Decode((string)gitHubFile.content);

                    // deserliase the YAML with YamlDotNet
                    var yamlString = fileContent.Substring(0, fileContent.LastIndexOf("---")); // chop off the markdown, leaving just the YAML header as YamlDotNet only deals with YAML documents
                    var yamlDeserializer = new DeserializerBuilder()
                        .WithNamingConvention(new CamelCaseNamingConvention())
                        .Build();
                    var yamlHeader = yamlDeserializer.Deserialize<YamlHeader>(yamlString);

                    // parse markdown to html with MarkDig
                    var mdPipeline = new MarkdownPipelineBuilder()
                        .UseYamlFrontMatter()
                        .UseAdvancedExtensions()
                        .Build();
                    var html = Markdown.ToHtml(fileContent, mdPipeline);

                    // build dto
                    var title = yamlHeader.Title ?? $"{DateTime.UtcNow.ToShortDateString()}-{DateTime.UtcNow.ToShortTimeString()}";
                    var path = string.Join("-", title.Split(Path.GetInvalidFileNameChars()));
                    path = path.Replace(" ", "-");
                    path = path.ToLowerInvariant();
                    var dto = new Dto()
                    {
                        Key = yamlHeader.Key ?? $"AutoGen-{Guid.NewGuid().ToString()}",
                        Title = title,
                        Author = yamlHeader.Author ?? string.Empty,
                        Description = yamlHeader.Description ?? string.Empty,
                        Image = yamlHeader.Image.ToLowerInvariant() ?? string.Empty,
                        Thumbnail = yamlHeader.Thumbnail.ToLowerInvariant() ?? string.Empty,
                        Type = yamlHeader.Type.ToLowerInvariant() ?? string.Empty,
                        Published = yamlHeader.Published,
                        Categories = (string.Join(",", yamlHeader.Categories)) ?? string.Empty,
                        HtmlBase64 = (Helpers.Base64Encode(html)) ?? string.Empty, // Base64 required to make sure things like line endings are properly included
                        Path = path
                    };

                    // respond with OK and the DTO object
                    return (ActionResult)new OkObjectResult(dto);         
                }
            }
            else
            {
                // repsond with bad request
                return (ActionResult)new BadRequestObjectResult("Null body passed");
            }
        }
    }

    public class YamlHeader
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Thumbnail { get; set; }
        public string Type { get; set; }
        public DateTime Published { get; set; }
        public List<string> Categories { get; set; }
    }

    public class Dto
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Thumbnail { get; set; }
        public string Type { get; set; }
        public DateTime Published { get; set; }
        public string Categories { get; set; }
        public string HtmlBase64 { get; set; }
        public string Path { get; set; }
    }
}
