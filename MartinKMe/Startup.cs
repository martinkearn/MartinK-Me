using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MartinKMe.Interfaces;
using MartinKMe.Models;
using MartinKMe.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pioneer.Pagination;

namespace MartinKMe
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.Configure<PersonaliseOptions>(options => Configuration.GetSection("Personalise").Bind(options));

            services.Configure<AppSecretSettings>(Configuration);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Add Pagination service
            services.AddTransient<IPaginatedMetaService, PaginatedMetaService>();

            // Add repositories
            services.AddSingleton<IStore, Store>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseCors(builder => builder.AllowAnyOrigin());

            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = r => r.Context.Response.Headers.Add("Expires", DateTime.Now.AddDays(7).ToUniversalTime().ToString("r"))
            });

            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    "talk",
                    "speaking/{talk}",
                    new { controller = "Speaking", action = "Talk" });

                routes.MapRoute(
                    "article",
                    "articles/{article}",
                    new { controller = "Articles", action = "Article" });

                routes.MapRoute(
                    "tag",
                    "tags/{tag}",
                    new { controller = "Tags", action = "Tag" });

                routes.MapRoute(
                    "redirect",
                    "{tagLabel}",
                    new { controller = "Redirect", action = "Index" });
            });
        }
    }
}
