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

    [Function(nameof(SayHello))]
    public string SayHello([ActivityTrigger] string cityName, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger(nameof(SayHello));
        logger.LogInformation("Saying hello to {name}", cityName);
        return $"Hello, {cityName}!";
    }

}