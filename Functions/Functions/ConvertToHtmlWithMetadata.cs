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

                    // get raw Markdown document
                    var response = await client.GetStringAsync(url);

                    // chop off the markdown, leaving just the YAML header
                    var yaml = response.Substring(0, response.LastIndexOf("---"));

                    // deserliase the YAML
                    var deserializer = new DeserializerBuilder()
                        .WithNamingConvention(new CamelCaseNamingConvention())
                        .Build();
                    var yamlHeader = deserializer.Deserialize<YamlHeader>(yaml);

                    // parse markdown to html with MarkDig
                    var pipeline = new MarkdownPipelineBuilder()
                        .UseYamlFrontMatter()
                        .UseAdvancedExtensions()
                        .ConfigureNewLine("\r\n")
                        .Build();
                    var html = Markdown.ToHtml(response, pipeline);

                    //// parse markdown to html with CommonMark.net
                    //var html = string.Empty;
                    //using (var writer = new StringWriter())
                    //{
                    //    CommonMark.CommonMarkConverter.ProcessStage3(CommonMark.CommonMarkConverter.Parse(response), writer);
                    //    html += writer.ToString();
                    //}

                    //// parse markdown to html with MarkdownSharp
                    //var html = new MarkdownSharp.Markdown().Transform(response);

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
                        Html = html
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
        public string Html { get; set; }
    }

    public class GitHubFile
    {
            public string name { get; set; }
            public string path { get; set; }
            public string sha { get; set; }
            public int size { get; set; }
            public string url { get; set; }
            public string html_url { get; set; }
            public string git_url { get; set; }
            public string download_url { get; set; }
            public string type { get; set; }
            public string content { get; set; }
            public string encoding { get; set; }
            public GitHubFileLinks _links { get; set; }
    }

    public class GitHubFileLinks
    {
        public string self { get; set; }
        public string git { get; set; }
        public string html { get; set; }
    }
}
