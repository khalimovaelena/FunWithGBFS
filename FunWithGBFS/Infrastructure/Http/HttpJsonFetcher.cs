using FunWithGBFS.Infrastructure.Http.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
