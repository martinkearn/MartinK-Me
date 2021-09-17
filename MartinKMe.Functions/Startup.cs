using MartinKMe.Domain.Interfaces;
using MartinKMe.Domain.Models;
using MartinKMe.Functions;
using MartinKMe.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace MartinKMe.Functions
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

            var context = builder.GetContext();
            this.configuration = context.Configuration;

            builder.Services.AddHttpClient();
            builder.Services.AddLogging();

            builder.Services.AddSingleton<IUtilityService, UtilityService>();
            builder.Services.AddSingleton<IGithubService, GithubService>();
            builder.Services.AddSingleton<IMarkdownService, MarkdownService>();
            builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();
            builder.Services.AddSingleton<IYamlService, YamlService>();

            builder.Services.AddOptions<BlobStorageConfiguration>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(nameof(BlobStorageConfiguration)).Bind(settings);
                });
        }
    }
}
