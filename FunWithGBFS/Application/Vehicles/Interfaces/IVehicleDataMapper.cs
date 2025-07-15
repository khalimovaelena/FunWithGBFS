using FunWithGBFS.Core.Models;
using FunWithGBFS.Domain.Models;

namespace FunWithGBFS.Application.Vehicles.Interfaces
{
    public interface IVehicleDataMapper
    {
        List<Vehicle> MapVehicles(string vehicleStatusJson, Provider provider);
    }
}
