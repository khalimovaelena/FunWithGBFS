using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FunWithGBFS.Services.GbfsAPI
{
    public static class GbfsFetcherHelper
    {
        public static bool TryGetFeeds(JsonDocument gbfsDoc, out JsonElement feeds)
        {
            feeds = default;
            if (gbfsDoc.RootElement.TryGetProperty("data", out var dataElement))
            {
                JsonElement innerElement;

                // Check if the data contains a language-layer (e.g. "en", "de", etc.)
                if (dataElement.ValueKind == JsonValueKind.Object && dataElement.EnumerateObject().First().Value.ValueKind == JsonValueKind.Object)
                {
                    // Language-keyed GBFS format (e.g. data -> en -> feeds)
                    innerElement = dataElement.EnumerateObject().First().Value;
                }
                else
                {
                    // Flat format (e.g. data -> feeds)
                    innerElement = dataElement;
                }

                if (innerElement.TryGetProperty("feeds", out feeds))
                {
                    if (feeds.ValueKind == JsonValueKind.Array)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
