using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace FunWithGBFS
{
    //TODO: implement interface
    public class GbfsFetcher
    {
        private static readonly HttpClient Http = new();

        public static async Task<List<Station>> FetchGbfsData(Provider provider)
        {
            try
            {
                // Fetch main GBFS index
                string gbfsJson = await Http.GetStringAsync(provider.GbfsUrl);
                using JsonDocument gbfsDoc = JsonDocument.Parse(gbfsJson);

                //TODO: mapper + handle errors
                var feeds = gbfsDoc.RootElement
                    .GetProperty("data")
                    .EnumerateObject().First().Value
                    .GetProperty("feeds");

                string? stationInfoUrl = null;
                string? stationStatusUrl = null;

                //TODO: use LINQ
                foreach (var feed in feeds.EnumerateArray())
                {
                    var name = feed.GetProperty("name").GetString();
                    var url = feed.GetProperty("url").GetString();

                    if (name == "station_information")
                    {
                        stationInfoUrl = url;
                    }
                    if (name == "station_status")
                    {
                        stationStatusUrl = url;
                    }
                }

                if (stationInfoUrl == null || stationStatusUrl == null)
                {
                    Console.WriteLine("Station feed URLs not found.");
                    return new List<Station>();
                }

                // Fetch station details
                string infoJson = await Http.GetStringAsync(stationInfoUrl);
                string statusJson = await Http.GetStringAsync(stationStatusUrl);

                var info = JsonSerializer.Deserialize<StationInfoRoot>(infoJson);
                var status = JsonSerializer.Deserialize<StationStatusRoot>(statusJson);

                var stations = new List<Station>();

                foreach (var i in info?.data?.stations ?? new())
                {
                    var s = status?.data?.stations?.Find(x => x.station_id == i.station_id);
                    stations.Add(new Station
                    {
                        Id = i.station_id,
                        Name = i.name,
                        Lat = i.lat,
                        Lon = i.lon,
                        BikesAvailable = s?.num_bikes_available ?? 0
                    });
                }

                return stations;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data for {provider.City}: {ex.Message}");
                return new List<Station>();
            }
        }
    }
}
