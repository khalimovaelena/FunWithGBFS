using FunWithGBFS.Core.Models;
using System.Text.Json;

namespace FunWithGBFS.Infrastructure.Gbfs
{
    public static class ProvidersLoader
    {
        public static List<Provider> LoadProviders(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<Provider>();
            }

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Provider>>(json) ?? new();
        }
    }
}
