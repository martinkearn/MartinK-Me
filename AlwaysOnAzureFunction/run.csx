#r "Newtonsoft.Json"

using System;
using Newtonsoft.Json;

public class Resource
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string FAIconClass { get; set; }
    public string Description { get; set; }
    public string ShortUrl { get; set; }
    public string TargetUrl { get; set; }
    public bool VisibleOnSite { get; set; }
    public bool AlwaysOn { get; set; }
}

public async static void Run(TimerInfo myTimer, TraceWriter log)
{
    //get 'alwayson' resources from api
    var apiUri = "http://martink.me/api/Resources?AlwaysOn=true";
    using (var httpClient = new HttpClient())
    {
        httpClient.BaseAddress = new Uri(apiUri);
        var response = await httpClient.GetAsync(apiUri);
        var responseContent = await response.Content.ReadAsStringAsync();
        var resources = JsonConvert.DeserializeObject<IEnumerable<Resource>>(responseContent);
    
        //enumerate resources
        foreach(var resource in resources)
        {
            //ping the url to keep it on
            using (var httpClientResource = new HttpClient())
            {
                httpClientResource.BaseAddress = new Uri(resource.TargetUrl);
                await httpClientResource.GetAsync(resource.TargetUrl);
                log.Info("Pinged " +resource.TargetUrl);
            }
        }
    }
}