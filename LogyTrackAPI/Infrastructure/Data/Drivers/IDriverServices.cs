using LogyTrackAPI.Infrastructure.Core;
using LogyTrackAPI.Models;

namespace LogyTrackAPI.Infrastructure.Data.Drivers
{
    public interface IDriverRepository : IRepository<Driver>
    {
        Task<IEnumerable<Driver>> GetActiveDriversAsync();
        Task<IEnumerable<dynamic>> GetDriversWithVehiclesAsync();
    }
}
