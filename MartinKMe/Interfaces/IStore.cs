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

        Task<List<Event>> GetEvents(int take);

        Task StoreEvent(Event Event);

        Task<List<Talk>> GetTalks();

        Task StoreTalk(Talk Talk);
    }
}
