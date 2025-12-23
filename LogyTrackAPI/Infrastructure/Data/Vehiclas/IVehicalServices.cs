using LogyTrackAPI.Infrastructure.Core;
using LogyTrackAPI.Models;

namespace LogyTrackAPI.Infrastructure.Data.Vehiclas
{
    public interface IVehicleRepository : IRepository<Vehicle>
    {
        Task<IEnumerable<Vehicle>> GetByStatusAsync(string status);
        Task<IEnumerable<dynamic>> GetVehiclesWithDriverAsync();
        Task<int> UpdateStatusAsync(int vehicleId, string status);
        Task<int> AssignDriverAsync(int vehicleId, int driverId);
        Task<int> UnassignDriverAsync(int vehicleId);
    }
}
