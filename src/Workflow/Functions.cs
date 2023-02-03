using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

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