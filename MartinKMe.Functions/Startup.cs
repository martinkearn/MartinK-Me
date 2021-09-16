using MartinKMe.Domain.Interfaces;
using MartinKMe.Functions;
using MartinKMe.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace MartinKMe.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<IUtilityService, UtilityService>();
            builder.Services.AddLogging();
        }
    }
}
