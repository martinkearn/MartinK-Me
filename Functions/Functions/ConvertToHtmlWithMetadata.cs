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

            // get payload
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            if (!string.IsNullOrEmpty(requestBody))
            {
                // get url from request body
                var url = requestBody;

                // get raw file and extract YAML
                using (var client = new HttpClient())
                {
                    // setup HttpClient
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Add("User-Agent", "MartinK.me ExtractYAML Function");

                    // get GitHub API response JSON and decode the content from base 64
                    var gitHubFileString = await client.GetStringAsync(url);
                    dynamic gitHubFile = JsonConvert.DeserializeObject(gitHubFileString);
                    var content = Helpers.Base64Decode((string)gitHubFile.content);

                    // chop off the markdown, leaving just the YAML header
                    var yaml = content.Substring(0, content.LastIndexOf("---"));

                    // deserliase the YAML
                    var deserializer = new DeserializerBuilder()
                        .WithNamingConvention(new CamelCaseNamingConvention())
                        .Build();
                    var yamlHeader = deserializer.Deserialize<YamlHeader>(yaml);

                    // parse markdown to html with MarkDig
                    var pipeline = new MarkdownPipelineBuilder()
                        .UseYamlFrontMatter()
                        .UseAdvancedExtensions()
                        .Build();
                    var html = Markdown.ToHtml(content, pipeline);

                    //the line endings are getting lsot when it is commited to storage

                    // build dto
                    var dto = new Dto()
                    {
                        RowKey = yamlHeader.Title.Replace(" ", "-").ToLowerInvariant(),
                        Title = yamlHeader.Title,
                        Author = yamlHeader.Author,
                        Description = yamlHeader.Description,
                        Image = yamlHeader.Image,
                        Published = yamlHeader.Published,
                        Categories = string.Join(",", yamlHeader.Categories),
                        HtmlBase64 = Helpers.Base64Encode(html)
                    };

                    // respond
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
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public DateTime Published { get; set; }
        public List<string> Categories { get; set; }
    }

    public class Dto
    {
        public string RowKey { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public DateTime Published { get; set; }
        public string Categories { get; set; }
        public string HtmlBase64 { get; set; }
    }
}
