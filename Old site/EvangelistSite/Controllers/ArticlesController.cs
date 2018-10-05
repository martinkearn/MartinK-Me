using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Xml.Linq;
using EvangelistSite.Models;
using EvangelistSite.Models.ArticlesViewModels;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;

namespace EvangelistSite.Controllers
{
    public class ArticlesController : Controller
    {
        private PersonaliseOptions _options;

        public ArticlesController(IOptions<PersonaliseOptions> options)
        {
            _options = options.Value;
        }

        public async Task<IActionResult> Index()
        {
            var articles = new List<FeedItem>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_options.BlogFeed);
                var responseMessage = await client.GetAsync(_options.BlogFeed);
                var responseString = await responseMessage.Content.ReadAsStringAsync();

                //extract feed items
                XDocument doc = XDocument.Parse(responseString);
                var feedItems = from item in doc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements().Where(i => i.Name.LocalName == "item")
                                select new FeedItem
                                {
                                    Description = QuickXmlDecode(item.Elements().First(i => i.Name.LocalName == "description").Value),
                                    Link = item.Elements().First(i => i.Name.LocalName == "link").Value,
                                    PublishDate = ParseDate(item.Elements().First(i => i.Name.LocalName == "pubDate").Value),
                                    Title = item.Elements().First(i => i.Name.LocalName == "title").Value
                                };
                articles = feedItems.ToList();
            }

            var vm = new IndexViewModel()
            {
                Articles = articles
            };

            return View(vm);
        }

        private DateTime ParseDate(string date)
        {
            DateTime result;
            if (DateTime.TryParse(date, out result))
                return result;
            else
                return DateTime.MinValue;
        }

        private string QuickXmlDecode(string orginal)
        {
            var returnString = orginal.Replace("&#8217;", "'");
            returnString = returnString.Replace("&#8216;", "‘");
            returnString = returnString.Replace("&#8217;", "’");
            returnString = returnString.Replace("&#8220;", "\"");
            returnString = returnString.Replace("&#8220;", "\"");
            returnString = returnString.Replace("&#160;", " ");
            return returnString; 
        }
    }
}