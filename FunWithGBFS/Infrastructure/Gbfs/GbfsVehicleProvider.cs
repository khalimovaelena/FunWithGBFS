using FunWithGBFS.Application.Vehicles.Interfaces;
using FunWithGBFS.Core.Models;
using FunWithGBFS.Domain.Models;
using FunWithGBFS.Infrastructure.Http.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FunWithGBFS.Infrastructure.Gbfs
{
    public class GbfsVehicleProvider : IVehicleProvider
    {
        private readonly IHttpJsonFetcher _httpFetcher;
        private readonly IVehicleDataMapper _mapper;
        private readonly ILogger<GbfsVehicleProvider> _logger;

        public GbfsVehicleProvider(IHttpJsonFetcher fetcher, IVehicleDataMapper mapper, ILogger<GbfsVehicleProvider> logger)
        {
            _httpFetcher = fetcher;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<Vehicle>> GetVehiclesAsync(Provider provider)
        {
            try
            {
                var autoDiscoveryDoc = await _httpFetcher.FetchAsync(provider.SourceUrl);

                // Check if the root element contains "data" and handle cases where it's missing
                if (!autoDiscoveryDoc.RootElement.TryGetProperty("data", out var dataElement))
                {
                    _logger.LogWarning("No 'data' property found for provider {Provider}", provider.Name);
                    return new List<Vehicle>();
                }

                // Check if 'feeds' exists and is of correct type (array)
                if (dataElement.TryGetProperty("feeds", out var feedsElement) && feedsElement.ValueKind == JsonValueKind.Array)
                {
                    // Look for the "vehicle_status" feed
                    var vehicleStatusUrl = feedsElement
                        .EnumerateArray()
                        .FirstOrDefault(feed =>
                            feed.TryGetProperty("name", out var name) && name.GetString() == "vehicle_status")
                        .TryGetProperty("url", out var urlProperty) ? urlProperty.GetString() : null;

                    if (vehicleStatusUrl == null)
                    {
                        _logger.LogWarning("No vehicle_status URL found for provider {Provider}", provider.Name);
                        return new List<Vehicle>();
                    }

                    // Fetch vehicle status data
                    var vehicleStatusJson = await DownloadRawJson(vehicleStatusUrl);
                    return _mapper.MapVehicles(vehicleStatusJson, provider);
                }
                else
                {
                    _logger.LogWarning("No valid 'feeds' array found for provider {Provider}", provider.Name);
                    return new List<Vehicle>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch vehicle data for provider {Provider}", provider.Name);
                return new List<Vehicle>();
            }
        }

        private async Task<string> DownloadRawJson(string url)
        {
            using var jsonDoc = await _httpFetcher.FetchAsync(url);

            // Handle the possibility of the root being an array for some cases (e.g., Check provider)
            if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
            {
                _logger.LogWarning("Root element is an array for URL: {Url}, adjusting parsing.", url);
                return jsonDoc.RootElement[0].GetRawText();  // Assuming the array contains a single object
            }

            return jsonDoc.RootElement.GetRawText();
        }
    }
}
