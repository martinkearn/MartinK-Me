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
using YamlDotNet.Serialization;
using Functions.Models;
using YamlDotNet.Serialization.NamingConventions;

namespace Functions
{
    public static class YamlToMetadata
    {
        [FunctionName("YamlToMetadata")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,ILogger log)
        {
            log.LogInformation("YamlToMetadata C# HTTP trigger function processed a request.");

            // get payload which is a URL to a Git Hub Repo API File object
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            if (!string.IsNullOrEmpty(requestBody))
            {
                // get raw file and extract YAML
                using (var client = new HttpClient())
                {
                    // setup HttpClient and get GitHub API response JSON and decode the content from base 64
                    client.BaseAddress = new Uri(requestBody);
                    client.DefaultRequestHeaders.Add("User-Agent", "MartinK.me YamlToMetadata Function");
                    var gitHubFileString = await client.GetStringAsync(requestBody);

                    // deserialise JSON and decode Base64 content
                    dynamic gitHubFile = JsonConvert.DeserializeObject(gitHubFileString);
                    var fileContent = Helpers.Base64Decode((string)gitHubFile.content);

                    // deserliase the YAML with YamlDotNet
                    var yamlString = fileContent.Substring(0, fileContent.LastIndexOf("---\n")); // chop off the markdown, leaving just the YAML header as YamlDotNet only deals with YAML documents. Assumes there is a space after the end of the YAML header
                    var yamlDeserializer = new DeserializerBuilder()
                        .WithNamingConvention(new CamelCaseNamingConvention())
                        .Build();
                    var yamlHeader = yamlDeserializer.Deserialize<YamlHeader>(yamlString);

                    // check we have required props
                    if (string.IsNullOrEmpty(yamlHeader.Title)) return (ActionResult)new BadRequestObjectResult("Title required in Yaml header");


                    string blobPath = req.Query["BlobPath"];
                    if (string.IsNullOrEmpty(blobPath)) return (ActionResult)new BadRequestObjectResult("BlobPath query param required");

                    // build dto
                    var path = string.Join("-", yamlHeader.Title.Split(Path.GetInvalidFileNameChars()));
                    path = path.Replace(" ", "-");
                    path = path.ToLowerInvariant();
                    var contentEntity = new ContentEntity()
                    {
                        Key = Helpers.Base64Encode((string)gitHubFile.path), // Base64 required because the path may contain a back slash
                        Title = yamlHeader.Title,
                        Author = yamlHeader.Author ?? string.Empty,
                        Description = yamlHeader.Description ?? string.Empty,
                        Image = yamlHeader.Image ?? string.Empty,
                        Thumbnail = yamlHeader.Thumbnail ?? string.Empty,
                        Type = yamlHeader.Type.ToLowerInvariant() ?? string.Empty,
                        Published = yamlHeader.Published,
                        Categories = (string.Join(",", yamlHeader.Categories)) ?? string.Empty,
                        HtmlBlobPath = blobPath,
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
