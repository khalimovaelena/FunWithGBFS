using FunWithGBFS.Application.Stations.Interfaces;
using FunWithGBFS.Core.Models;
using System.Text.Json;

namespace FunWithGBFS.Infrastructure.Gbfs
{
    public class GbfsStationDataMapper : IStationDataMapper
    {
        public List<Station> MapStations(string rawInfoJson, string rawStatusJson, string city)
        {
            using var infoDoc = JsonDocument.Parse(rawInfoJson);
            using var statusDoc = JsonDocument.Parse(rawStatusJson);

            var infoStations = ExtractStationList(infoDoc);
            var statusStations = ExtractStationList(statusDoc);

            var statusDict = statusStations
                .EnumerateArray()
                .ToDictionary(s => s.GetProperty("station_id").GetString());

            var stations = new List<Station>();

            foreach (var stationInfo in infoStations.EnumerateArray())
            {
                var stationId = stationInfo.GetProperty("station_id").GetString();
                statusDict.TryGetValue(stationId, out var status);

                stations.Add(new Station
                {
                    Id = stationId,
                    Name = stationInfo.GetProperty("name").GetString(),
                    Lat = stationInfo.GetProperty("lat").GetDouble(),
                    Lon = stationInfo.GetProperty("lon").GetDouble(),
                    BikesAvailable = status.GetProperty("num_bikes_available").GetInt32(),
                    City = city,
                });
            }

            return stations;
        }

        private JsonElement ExtractStationInfo(JsonDocument doc)
        {
            var root = doc.RootElement;

            if (root.TryGetProperty("data", out var dataElement))
            {
                // Check if the value is an object with a language-layer (e.g., "en")
                if (dataElement.ValueKind == JsonValueKind.Object)
                {
                    // If it's nested under a language key
                    var firstValue = dataElement.EnumerateObject().FirstOrDefault().Value;

                    if (firstValue.TryGetProperty("feeds", out var feeds))
                    {
                        return feeds;
                    }
                }

                // If it's flat structure
                if (dataElement.TryGetProperty("feeds", out var feedsDirect))
                {
                    return feedsDirect;
                }
            }

            throw new InvalidOperationException("Invalid GBFS structure for station info");
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
