using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MartinKMe.Functions.Activities
{
    public class MarkdownToHtmlActivity
    {
        [FunctionName(nameof(MarkdownToHtmlActivity))]
        public async Task<string> MarkdownToHtml([ActivityTrigger] string markdown)
        {
            // Return
            return "contents";
        }
    }
}
