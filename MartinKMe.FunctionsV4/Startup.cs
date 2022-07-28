using MartinKMe.Domain.Interfaces;
using MartinKMe.Domain.Models;
using MartinKMe.FunctionsV4;
using MartinKMe.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace MartinKMe.FunctionsV4
{
    public class Startup : FunctionsStartup
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        private IConfiguration configuration;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            FunctionsHostBuilderContext context = builder.GetContext();
            configuration = context.Configuration;

            _ = builder.Services.AddHttpClient();
            _ = builder.Services.AddLogging();

            _ = builder.Services.AddSingleton<IGithubService, GithubService>();
            _ = builder.Services.AddSingleton<IMarkdownService, MarkdownService>();
            _ = builder.Services.AddSingleton<IStorageService, StorageService>();
            _ = builder.Services.AddSingleton<IYamlService, YamlService>();

            _ = builder.Services.AddOptions<StorageConfiguration>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(nameof(StorageConfiguration)).Bind(settings);
                });
        }
    }
}
