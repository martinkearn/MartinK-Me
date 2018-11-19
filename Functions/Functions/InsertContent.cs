using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Functions.Models;

namespace Functions
{
    public static class InsertContent
    {
        [FunctionName("InsertContent")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,ILogger log)
        {
            log.LogInformation("InsertContent function processed a request.");

            // get payload which is a ContentEntity JSON document
            string requestBody = new StreamReader(req.Body).ReadToEnd();

            //deserliase json
            var contentEntity = JsonConvert.DeserializeObject<ContentEntity>(requestBody);

            return (ActionResult)new OkObjectResult(contentEntity);
        }
    }
}
