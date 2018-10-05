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
    }
}
