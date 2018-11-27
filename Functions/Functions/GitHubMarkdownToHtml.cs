using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Markdig;

namespace Functions
{
    public static class GitHubMarkdownToHtml
    {
        [FunctionName("GitHubMarkdownToHtml")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = null)] HttpRequest req,ILogger log)
        {
            log.LogInformation("GitHubMarkdownToHtml C# HTTP trigger function processed a request.");

            // get payload which is a URL to a Git Hub Repo API File object
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            if (!string.IsNullOrEmpty(requestBody))
            {
                // get raw file and extract YAML
                using (var client = new HttpClient())
                {
                    // setup HttpClient and get GitHub API response JSON and decode the content from base 64
                    client.BaseAddress = new Uri(requestBody);
                    client.DefaultRequestHeaders.Add("User-Agent", "MartinK.me GitHubMarkdownToHtml Function");
                    var gitHubFileString = await client.GetStringAsync(requestBody);

                    // deserialise JSON and decode Base64 content
                    dynamic gitHubFile = JsonConvert.DeserializeObject(gitHubFileString);
                    var fileContent = Helpers.Base64Decode((string)gitHubFile.content);

                    // parse markdown to html with MarkDig
                    var mdPipeline = new MarkdownPipelineBuilder()
                        .UseYamlFrontMatter()
                        .UseAdvancedExtensions()
                        .Build();
                    var html = Markdown.ToHtml(fileContent, mdPipeline);

                    // trim leading <H1> ... Bit hacky as it assumes that the H1 is the first line of html
                    var htmlNoH1 = html.Substring(html.IndexOf("</h1>") + 5);

                    // base64 encode
                    var htmlbase64 = Helpers.Base64Encode(htmlNoH1);

                    // respond with OK and the object
                    return (ActionResult)new OkObjectResult(htmlbase64);
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
