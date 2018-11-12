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
                    var article = deserializer.Deserialize<Article>(yaml);

                    // respond
                    return (ActionResult)new OkObjectResult(article);
                    
                }
            }
            else
            {
                // repsond with bad request
                return (ActionResult)new BadRequestObjectResult("Null body passed");
            }
        }
    }

public class Article
{
    public string Title { get; set; }
    public string Author { get; set; }
    public List<string> Categories { get; set; }
}
}
