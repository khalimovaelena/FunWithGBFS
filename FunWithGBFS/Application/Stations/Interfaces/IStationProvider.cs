using FunWithGBFS.Core.Models;

namespace FunWithGBFS.Application.Stations.Interfaces
{
    public interface IStationProvider
    {
        Task<List<Station>> GetStationsAsync(Provider provider);
    }
}
