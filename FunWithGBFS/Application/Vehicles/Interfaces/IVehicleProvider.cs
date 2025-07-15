using FunWithGBFS.Core.Models;
using FunWithGBFS.Domain.Models;

namespace FunWithGBFS.Application.Vehicles.Interfaces
{
    public interface IVehicleProvider
    {
        Task<List<Vehicle>> GetVehiclesAsync(Provider provider);
    }
}
