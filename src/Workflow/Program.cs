global using Services;
global using Workflow.Models;
global using Workflow.Orchestrations;
global using Workflow.Activities;
global using Workflow.Interfaces;
global using Workflow.Services;
global using Domain.Interfaces;
global using Domain.Models;
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
        services.AddOptions<GithubConfiguration>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(nameof(GithubConfiguration)).Bind(settings);
            });
    })
    .Build();

host.Run();
