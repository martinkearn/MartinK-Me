using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using MartinKMe.Models;
using Microsoft.Extensions.Options;
using MartinKMe.Models.SearchViewModels;

namespace MartinKMe.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppSecretSettings _appSecretSettings;

        public SearchController(IOptions<AppSecretSettings> appSecretSettings)
        {
            _appSecretSettings = appSecretSettings.Value;
        }

        public IActionResult Index(string searchTerm)
        {
            // Call search api
            BingCustomSearchResponse response;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var url = $"https://api.cognitive.microsoft.com/bingcustomsearch/v7.0/search?q={searchTerm}&customconfig={_appSecretSettings.SearchCustomConfigId}";
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _appSecretSettings.SearchSubscriptionKey);
                var httpResponseMessage = client.GetAsync(url).Result;
                var responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
                response = JsonConvert.DeserializeObject<BingCustomSearchResponse>(responseContent);
            }
            else
            {
                response = null;
            }


            // Build view model
            var vm = new IndexViewModel()
            {
                SearchResponse = response,
                SearchTerm = searchTerm
            };

            return View(vm);
        }
    }
}