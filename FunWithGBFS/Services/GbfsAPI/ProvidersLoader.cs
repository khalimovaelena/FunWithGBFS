using FunWithGBFS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FunWithGBFS.Services.GbfsAPI
{
    public static class ProvidersLoader
    {
        public static List<Provider> LoadProviders(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Providers file not found: {filePath}");
                return new List<Provider>();
            }

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Provider>>(json) ?? new();
        }
    }
}
