using FunWithGBFS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Application.Stations.Interfaces
{
    public interface IStationProvider
    {
        Task<List<Station>> GetStationsAsync(Provider provider);
    }
}
