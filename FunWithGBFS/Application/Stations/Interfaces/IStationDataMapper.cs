using FunWithGBFS.Core.Models;

namespace FunWithGBFS.Application.Stations.Interfaces
{
    public interface IStationDataMapper
    {
        List<Station> MapStations(string rawInfoJson, string rawStatusJson, Provider provider);
    }
}
