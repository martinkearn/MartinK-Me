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

namespace Functions
{
    public static class FilesList
    {
        /// <summary>
        /// Receives JSON from a GitHub Webhook Push event and extacts a list of changed, added or deleted *.md files
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("FilesList")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("FilesList function processed a request.");

            // get payload
            string requestBody = new StreamReader(req.Body).ReadToEnd();

            if (!string.IsNullOrEmpty(requestBody))
            {
                dynamic payload = JsonConvert.DeserializeObject(requestBody);

                // get commit details required to call API
                var commitId = payload.after;
                var owner = payload.head_commit.author.username;
                var repo = payload.repository.name;

                // get commit files urls
                var markdownFilesInCommit = new List<string>();
                using (var client = new HttpClient())
                {
                    //setup HttpClient
                    var getCommitUrl = $"https://api.github.com/repos/{owner}/{repo}/commits/{commitId}";
                    client.BaseAddress = new Uri(getCommitUrl);
                    client.DefaultRequestHeaders.Add("User-Agent", "MartinK.me FilesList Function");

                    //setup httpContent object
                    var getCommitResponse = await client.GetAsync(getCommitUrl);

                    //return null if not sucessfull
                    if (getCommitResponse.IsSuccessStatusCode)
                    {
                        var getCommitResponseString = await getCommitResponse.Content.ReadAsStringAsync();
                        dynamic commit = JsonConvert.DeserializeObject(getCommitResponseString);

                        // get the file
                        foreach (dynamic file in commit.files)
                        {
                            var fileApiUrl = $"https://api.github.com/repos/{owner}/{repo}/contents/{file.filename}";
                            if (fileApiUrl.ToLower().EndsWith(".md"))
                            {
                                //we have a markdown file
                                markdownFilesInCommit.Add(fileApiUrl);
                            }
                        }
                    }
                }

                // respond with OK and response body
                return (ActionResult)new OkObjectResult(markdownFilesInCommit);
            }
            else
            {
                // repsond with bad request
                return (ActionResult)new BadRequestResult();
            }

        }
    }
}
