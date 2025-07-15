using FunWithGBFS.Infrastructure.Http.Interfaces;
using System.Text.Json;

namespace FunWithGBFS.Infrastructure.Http
{
    public class HttpJsonFetcher : IHttpJsonFetcher
    {
        private readonly HttpClient _httpClient = new();

        public async Task<JsonDocument> FetchAsync(string url)
        {
            var json = await _httpClient.GetStringAsync(url);
            return JsonDocument.Parse(json);
        }
    }

}
