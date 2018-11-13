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

namespace Functions
{
    public static class ExtractYAML
    {
        [FunctionName("ExtractYAML")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("ExtractYAML function processed a request.");

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

                    // build dto
                    var dto = new MetadataDTO()
                    {
                        RowKey = yamlHeader.Title.Replace(" ", "-").ToLowerInvariant(),
                        Title = yamlHeader.Title,
                        Author = yamlHeader.Author,
                        Description = yamlHeader.Description,
                        Image = yamlHeader.Image,
                        Published = yamlHeader.Published,
                        Categories = string.Join(",", yamlHeader.Categories)
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

    public class MetadataDTO
    {
        public string RowKey { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public DateTime Published { get; set; }
        public string Categories { get; set; }
    }
}
