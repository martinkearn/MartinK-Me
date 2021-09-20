using MartinKMe.Domain.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace MartinKMe.Functions.Activities
{
    public class DecodeBase64Activity
    {
        private readonly IUtilityService _utilityService;

        public DecodeBase64Activity(IUtilityService utilityService)
        {
            _utilityService = utilityService;
        }

        [FunctionName(nameof(DecodeBase64Activity))]
        public string DecodeBase64([ActivityTrigger] string base64EncodedString)
        {
            return _utilityService.Base64Decode(base64EncodedString);
        }
    }
}
