using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FunWithGBFS.Infrastructure.Http.Interfaces
{
    public interface IHttpJsonFetcher
    {
        Task<JsonDocument> FetchAsync(string url);
    }
}
