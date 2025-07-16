using FunWithGBFS.Application.Vehicles.Interfaces;
using FunWithGBFS.Domain.Models;
using System.Text.Json;

namespace FunWithGBFS.Infrastructure.Gbfs
{
    public class GbfsVehicleDataMapper : IVehicleDataMapper
    {
        public List<Vehicle> MapVehicles(string vehicleStatusJson, Provider provider)
        {
            var vehicles = new List<Vehicle>();

            using var doc = JsonDocument.Parse(vehicleStatusJson);
            if (doc.RootElement.TryGetProperty("data", out var data) &&
                data.TryGetProperty("vehicles", out var vehicleArray) &&
                vehicleArray.ValueKind == JsonValueKind.Array)
            {
                foreach (var vehicleJson in vehicleArray.EnumerateArray())
                {
                    var vehicle = new Vehicle
                    {
                        Id = vehicleJson.GetProperty("vehicle_id").GetString() ?? Guid.NewGuid().ToString(),
                        Type = vehicleJson.GetProperty("vehicle_type_id").GetString() ?? "unknown",
                        IsDisabled = vehicleJson.GetProperty("is_disabled").GetBoolean(),
                        IsReserved = vehicleJson.GetProperty("is_reserved").GetBoolean(),
                        ProviderName = provider.Name,
                        City = provider.City
                    };

                    vehicles.Add(vehicle);
                }
            }

            return vehicles;
        }
    }
}
