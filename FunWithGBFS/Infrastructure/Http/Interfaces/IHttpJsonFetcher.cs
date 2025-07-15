using System.Text.Json;

namespace FunWithGBFS.Infrastructure.Http.Interfaces
{
    public interface IHttpJsonFetcher
    {
        Task<JsonDocument> FetchAsync(string url);
    }
}
