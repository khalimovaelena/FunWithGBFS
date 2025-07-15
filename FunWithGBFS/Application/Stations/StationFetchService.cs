using FunWithGBFS.Application.Stations.Interfaces;
using FunWithGBFS.Core.Models;
using FunWithGBFS.Presentation.Interfaces;

namespace FunWithGBFS.Application.Stations
{
    public class StationFetchService
    {
        private readonly IStationProvider _stationProvider;
        private readonly IUserInteraction _interaction;

        public StationFetchService(IStationProvider stationProvider, IUserInteraction interaction)
        {
            _stationProvider = stationProvider;
            _interaction = interaction;
        }

        public async Task<List<Station>> LoadStationsAsync(List<Provider> providers)
        {
            var stations = new List<Station>();

            foreach (var provider in providers)
            {
                _interaction.ShowMessage($"\nFetching data for {provider.Name}...");
                var providerStations = await _stationProvider.GetStationsAsync(provider);

                _interaction.ShowMessage($"Stations found: {providerStations.Count}");
                if (providerStations.Count > 0)
                {
                    stations.AddRange(providerStations);
                }
                else
                {
                    _interaction.ShowWarning($"No stations found for {provider.Name}.");
                }
            }

            return stations;
        }
    }

}
