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
using Functions.Models;
using System.Net;

namespace Functions
{
    public static class FilesList
    {
        /// <summary>
        /// Receives JSON from a GitHub Webhook Push event and extacts a list of changed, added or deleted *.md files. Then works out the GitHub repo API endpoint for each one and adds to an array which is returned
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
                // deserialise payload to dynamic object
                dynamic payload = JsonConvert.DeserializeObject(requestBody);

                // get commit details required to call API
                var commitId = payload.head_commit.id;
                var owner = payload.head_commit.author.username;
                var repo = payload.repository.name;

                // get commit files urls
                var addedFiles = new List<string>();
                var removedFiles = new List<string>();
                var modifiedFiles = new List<string>();
                using (var client = new HttpClient())
                {
                    //setup HttpClient and get Commit
                    var getCommitUrl = $"https://api.github.com/repos/{owner}/{repo}/commits/{commitId}";
                    client.BaseAddress = new Uri(getCommitUrl);
                    client.DefaultRequestHeaders.Add("User-Agent", "MartinK.me FilesList Function");
                    var getCommitResponse = await client.GetAsync(getCommitUrl);

                    //return BadRequest if not sucessfull
                    if (getCommitResponse.IsSuccessStatusCode)
                    {
                        // deserialise the commit JSON
                        var getCommitResponseString = await getCommitResponse.Content.ReadAsStringAsync();
                        //dynamic commit = JsonConvert.DeserializeObject(getCommitResponseString);
                        var commit = JsonConvert.DeserializeObject<GitHubCommitDto>(getCommitResponseString);

                        // get the files in the commit
                        foreach (var file in commit.files)
                        {
                            var fileApiUrl = $"https://api.github.com/repos/{owner}/{repo}/contents/{file.filename}";
                            if (fileApiUrl.ToLower().EndsWith(".md"))
                            {
                                //we have a markdown file
                                switch (file.status)
                                {
                                    case "added":
                                        addedFiles.Add(fileApiUrl);
                                        break;
                                    case "modified":
                                        modifiedFiles.Add(fileApiUrl);
                                        break;
                                    case "renamed":
                                        modifiedFiles.Add(fileApiUrl);
                                        break;
                                    case "removed":
                                        removedFiles.Add(fileApiUrl);
                                        break;
                                    default: break;
                                }
                            }
                        }
                    }
                    else
                    {
                        // repsond with bad request
                        return (ActionResult)new BadRequestObjectResult($"GitHub API {getCommitUrl} returned a non-sucess status code.");
                    }
                }

                //construct response
                var markdownFilesInCommit = new Dictionary<string, List<string>>();
                markdownFilesInCommit.Add("added", addedFiles);
                markdownFilesInCommit.Add("removed", removedFiles);
                markdownFilesInCommit.Add("modified", modifiedFiles);

                // respond with OK and response body
                return (ActionResult)new OkObjectResult(markdownFilesInCommit);
            }
            else
            {
                // repsond with ok and message about empty body. Bad request cause sthe logic app to fail
                return (ActionResult)new BadRequestObjectResult("Empty body posted");
            }

        }
    }
}
