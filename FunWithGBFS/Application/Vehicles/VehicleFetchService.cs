using FunWithGBFS.Application.Vehicles.Interfaces;
using FunWithGBFS.Domain.Models;
using FunWithGBFS.Presentation.Interfaces;

namespace FunWithGBFS.Application.Vehicles
{
    public class VehicleFetchService
    {
        private readonly IVehicleProvider _vehicleProvider;
        private readonly IUserInteraction _interaction;

        public VehicleFetchService(IVehicleProvider vehicleProvider, IUserInteraction interaction)
        {
            _vehicleProvider = vehicleProvider;
            _interaction = interaction;
        }

        public async Task<List<Vehicle>> LoadVehiclesAsync(List<Provider> providers)
        {
            var vehicles = new List<Vehicle>();

            foreach (var provider in providers)
            {
                _interaction.ShowMessage($"\nFetching vehicles data for {provider.Name}...");
                var providerVehicles = await _vehicleProvider.GetVehiclesAsync(provider);

                _interaction.ShowMessage($"Vehicles found: {providerVehicles.Count}");
                if (providerVehicles.Count > 0)
                {
                    vehicles.AddRange(providerVehicles);
                }
                else
                {
                    _interaction.ShowWarning($"No Vehicles found for {provider.Name}.");
                }
            }

            return vehicles;
        }
    }
}
