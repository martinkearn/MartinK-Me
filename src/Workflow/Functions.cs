using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Domain.Interfaces;

class HelloSequence
{
    private readonly IStorageService _storageService;

    public HelloSequence(IStorageService storageService)
    {
        _storageService = storageService;
    }

    [Function(nameof(HelloCities))]
    public async Task<string> HelloCities([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        string result = "";
        result += await context.CallActivityAsync<string>(nameof(SayHello), "Tokyo") + " ";
        result += await context.CallActivityAsync<string>(nameof(SayHello), "London") + " ";
        result += await context.CallActivityAsync<string>(nameof(SayHello), "Seattle");
        return result;
    }

    [Function(nameof(SayHello))]
    public string SayHello([ActivityTrigger] string cityName, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger(nameof(SayHello));
        logger.LogInformation("Saying hello to {name}", cityName);
        return $"Hello, {cityName}!";
    }

    [Function(nameof(StartHelloCities))]
    public async Task<HttpResponseData> StartHelloCities(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
/*         var article = new Domain.Models.Article()
        {
            Key = "YmxvZ3MvdGVzdC5tZA==",
            Title = "Test article",
            Author = "Martin Kearn",
            Description = "Test description",
            Image = "https://martink.me/images/speaking.jpg",
            Thumbnail = "https://martink.me/images/speaking.jpg",
            Published = DateTime.UtcNow,
            Categories = "test",
            Status = "draft",
            WebPath = "test",
            HtmlBlobPath = new Uri("https://martink.me/images/speaking.jpg")

        };
        await _storageService.UpsertArticle(article); */

        ILogger logger = executionContext.GetLogger(nameof(StartHelloCities));

        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(HelloCities));
        logger.LogInformation("Created new orchestration with instance ID = {instanceId}", instanceId);

        return client.CreateCheckStatusResponse(req, instanceId);
    }
}