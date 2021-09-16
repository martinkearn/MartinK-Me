using MartinKMe.Domain.Interfaces;
using MartinKMe.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
[assembly: FunctionsStartup(typeof(MartinKMe.Functions.Startup))]
namespace MartinKMe.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<IUtilityService, UtilityService>();
            builder.Services.AddLogging();
        }
    }
}
