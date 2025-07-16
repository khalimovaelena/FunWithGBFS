using FunWithGBFS.Application.Stations.Interfaces;
using FunWithGBFS.Domain.Models;
using System.Text.Json;

namespace FunWithGBFS.Infrastructure.Gbfs
{
    public class GbfsStationDataMapper : IStationDataMapper
    {
        public List<Station> MapStations(string rawInfoJson, string rawStatusJson, Provider provider)
        {
            try
            {
                using var infoDoc = JsonDocument.Parse(rawInfoJson);

                JsonDocument statusDoc = null;
                Dictionary<string, JsonElement> statusDict = new();

                try
                {
                    statusDoc = JsonDocument.Parse(rawStatusJson);
                    var statusStations = ExtractStationList(statusDoc);
                    statusDict = statusStations
                        .EnumerateArray()
                        .ToDictionary(
                            s => s.GetProperty("station_id").GetString(),
                            s => s
                        );
                }
                catch
                {
                    statusDict = new();// statusDict remains empty
                }

                var infoStations = ExtractStationList(infoDoc);
                var stations = new List<Station>();

                foreach (var stationInfo in infoStations.EnumerateArray())
                {
                    var stationId = stationInfo.GetProperty("station_id").GetString();
                    statusDict.TryGetValue(stationId, out var status);

                    int bikesAvailable = 0;
                    if (status.ValueKind == JsonValueKind.Object && status.TryGetProperty("num_bikes_available", out var bikesProp))
                    {
                        bikesAvailable = bikesProp.GetInt32();
                    }

                    stations.Add(new Station
                    {
                        Id = stationId,
                        Name = stationInfo.GetProperty("name").GetString(),
                        ProviderName = provider.Name,
                        Lat = stationInfo.GetProperty("lat").GetDouble(),
                        Lon = stationInfo.GetProperty("lon").GetDouble(),
                        BikesAvailable = bikesAvailable,
                        City = provider.City,
                    });
                }

                return stations;
            }
            catch
            {
                return new List<Station>();
            }
        }

        private JsonElement ExtractStationList(JsonDocument doc)
        {
            var root = doc.RootElement;

            if (root.TryGetProperty("data", out var dataElement))
            {
                if (dataElement.ValueKind == JsonValueKind.Object)
                {
                    var firstValue = dataElement.EnumerateObject().FirstOrDefault().Value;

                    if (firstValue.ValueKind == JsonValueKind.Object &&
                        firstValue.TryGetProperty("stations", out var stationsNested))
                    {
                        return stationsNested;
                    }

                    if (dataElement.TryGetProperty("stations", out var stationsDirect))
                    {
                        return stationsDirect;
                    }
                }
            }

            throw new InvalidOperationException("Invalid GBFS structure for stations list");
        }
    }
}
