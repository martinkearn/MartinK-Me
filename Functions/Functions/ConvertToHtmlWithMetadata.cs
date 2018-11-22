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
using Functions.Models;

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

                    // trim leading <H1> ... Bit hacky as it assumes that the H1 is the first line of html after the Yaml whihc is convention
                    var htmlNoH1 = html.Substring(html.IndexOf("</h1>") + 5);

                    // check we have required props
                    if (string.IsNullOrEmpty(yamlHeader.Title)) return (ActionResult)new BadRequestObjectResult("Title required in Yaml header");

                    // build dto
                    var path = string.Join("-", yamlHeader.Title.Split(Path.GetInvalidFileNameChars()));
                    path = path.Replace(" ", "-");
                    path = path.ToLowerInvariant();
                    var contentEntity = new ContentEntity()
                    {
                        Key = Helpers.Base64Encode((string)gitHubFile.path), // Base64 required because the path may contain a backslash
                        Title = yamlHeader.Title,
                        Author = yamlHeader.Author ?? string.Empty,
                        Description = yamlHeader.Description ?? string.Empty,
                        Image = yamlHeader.Image.ToLowerInvariant() ?? string.Empty,
                        Thumbnail = yamlHeader.Thumbnail.ToLowerInvariant() ?? string.Empty,
                        Type = yamlHeader.Type.ToLowerInvariant() ?? string.Empty,
                        Published = yamlHeader.Published,
                        Categories = (string.Join(",", yamlHeader.Categories)) ?? string.Empty,
                        HtmlBase64 = (Helpers.Base64Encode(htmlNoH1)) ?? string.Empty, // Base64 required to make sure things like line endings are properly included
                        Path = path,
                        Status = yamlHeader.Status,
                        GitHubPath = (string)gitHubFile.path
                    };

                    // respond with OK and the object
                    return (ActionResult)new OkObjectResult(contentEntity);         
                }
            }
            else
            {
                // repsond with bad request
                return (ActionResult)new BadRequestObjectResult("Null body passed");
            }
        }
    }

}
