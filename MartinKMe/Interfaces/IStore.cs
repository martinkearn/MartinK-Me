using MartinKMe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKMe.Interfaces
{
    public interface IStore
    {
        Task<List<Link>> GetLinks();

        Task StoreLink(Link Link);

        Task<List<Shortcut>> GetShortcuts();

        Task StoreShortcut(Shortcut Shortcut);

        Task DeleteShortcut(string title);

        Task<List<Event>> GetEvents(int take);

        Task StoreEvent(Event Event);

        Task<List<Talk>> GetTalks();

        Task StoreTalk(Talk Talk);

        Task<List<Content>> GetContents();

        Task<Content> GetContent(string id);

        Task<List<string>> GetWallpaperUris();
    }
}
