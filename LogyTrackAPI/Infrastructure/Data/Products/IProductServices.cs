using LogyTrackAPI.Infrastructure.Core;
using LogyTrackAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogyTrackAPI.Infrastructure.Data.Products
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetByStatusAsync(string status);
        Task<IEnumerable<Product>> GetByVehicleAsync(int vehicleId);
        Task<IEnumerable<Product>> GetByDriverAsync(int driverId);
        Task<IEnumerable<Product>> GetUnassignedAsync();
        Task<int> AssignProductToVehicleAsync(int productId, int vehicleId, int driverId);
        Task<int> UnassignProductAsync(int productId);
        Task<int> UpdateProductStatusAsync(int productId, string status);
    }
}
