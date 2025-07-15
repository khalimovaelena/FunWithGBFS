using FunWithGBFS.Application.Stations.Interfaces;
using FunWithGBFS.Core.Models;
using FunWithGBFS.Infrastructure.Http.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FunWithGBFS.Infrastructure.Gbfs
{
    public class GbfsStationProvider : IStationProvider
    {
        private readonly IHttpJsonFetcher _httpFetcher;
        private readonly IStationDataMapper _dataMapper;
        private readonly ILogger<GbfsStationProvider> _logger;

        public GbfsStationProvider(
            IHttpJsonFetcher httpFetcher,
            IStationDataMapper dataMapper,
            ILogger<GbfsStationProvider> logger)
        {
            _httpFetcher = httpFetcher;
            _dataMapper = dataMapper;
            _logger = logger;
        }

        public async Task<List<Station>> GetStationsAsync(Provider provider)
        {
            try
            {
                JsonDocument gbfsDoc = await _httpFetcher.FetchAsync(provider.SourceUrl);

                if (!TryGetFeeds(gbfsDoc, out var feeds))
                {
                    _logger.LogWarning("Feeds not found or invalid format for provider {Provider}", provider.Name);
                    return new();
                }

                string? infoUrl = null;
                string? statusUrl = null;

                foreach (var feed in feeds.EnumerateArray())
                {
                    var name = feed.GetProperty("name").GetString();
                    var url = feed.GetProperty("url").GetString();

                    if (name == "station_information")
                        infoUrl = url;

                    if (name == "station_status")
                        statusUrl = url;
                }

                if (string.IsNullOrEmpty(infoUrl) || string.IsNullOrEmpty(statusUrl))
                {
                    _logger.LogWarning("Missing station information or status URLs for provider {Provider}", provider.Name);
                    return new();
                }

                string infoJson = await DownloadRawJson(infoUrl);
                string statusJson = await DownloadRawJson(statusUrl);

                return _dataMapper.MapStations(infoJson, statusJson, provider.City);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching stations for provider {Provider}", provider.Name);
                return new();
            }
        }

        private async Task<string> DownloadRawJson(string url)
        {
            using var jsonDoc = await _httpFetcher.FetchAsync(url);
            return jsonDoc.RootElement.GetRawText();
        }

        private static bool TryGetFeeds(JsonDocument gbfsDoc, out JsonElement feeds)
        {
            feeds = default;

            if (!gbfsDoc.RootElement.TryGetProperty("data", out var dataElement))
                return false;

            JsonElement inner;

            if (dataElement.ValueKind == JsonValueKind.Object &&
                dataElement.EnumerateObject().First().Value.ValueKind == JsonValueKind.Object)
            {
                // e.g., data -> en -> feeds
                inner = dataElement.EnumerateObject().First().Value;
            }
            else
            {
                // e.g., data -> feeds
                inner = dataElement;
            }

            if (inner.TryGetProperty("feeds", out feeds) && feeds.ValueKind == JsonValueKind.Array)
                return true;

            return false;
        }
    }
}
