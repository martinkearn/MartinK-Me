global using Services;
global using Workflow.Models;
global using Workflow.Orchestrations;
global using Workflow.Activities;
global using Domain.Models;
global using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddHttpClient();
        services.AddLogging();
        services.AddSingleton<IGithubService, GithubService>();
        services.AddSingleton<IMarkdownService, MarkdownService>();
        services.AddSingleton<IStorageService, StorageService>();
        services.AddSingleton<IYamlService, YamlService>();
        services.AddOptions<StorageConfiguration>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(nameof(StorageConfiguration)).Bind(settings);
            });
    })
    .Build();

host.Run();
