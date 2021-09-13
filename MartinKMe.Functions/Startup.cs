using MartinKMe.Domain.Interfaces;
using MartinKMe.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MartinKMe.Functions
{
    public class Startup : FunctionsStartup
    {
        /// <inheritdoc />
        public override void Configure(IFunctionsHostBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            var services = builder.Services;
            builder.Services.AddSingleton<IUtilityService, UtilityService>();
            services.BuildServiceProvider();
        }
    }
}
