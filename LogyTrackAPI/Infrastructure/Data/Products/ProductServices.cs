using Dapper;
using LogyTrackAPI.Data;
using LogyTrackAPI.Infrastructure.Core;
using LogyTrackAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LogyTrackAPI.Infrastructure.Data.Products
{
    public class ProductServices : BaseRepository<Product>, IProductRepository
    {
        protected override string GetAllProcedure => "sp_GetAllProducts";
        protected override string GetByIdProcedure => "sp_GetProductById";
        //protected override string CreateProcedure => "sp_CreateProduct";
        //protected override string UpdateProcedure => "sp_UpdateProduct";
        //protected override string DeleteProcedure => "sp_DeleteProduct";

        // ← ADDED: Correct parameter name for Products
        protected override string IdParameterName => "@ProductId";

        public ProductServices(DapperContext context) : base(context) { }

        public async Task<IEnumerable<Product>> GetByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status cannot be empty", nameof(status));

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Product>(
                    "sp_GetProductsByStatus",
                    new { Status = status },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Product>> GetByVehicleAsync(int vehicleId)
        {
            if (vehicleId <= 0)
                throw new ArgumentException("Vehicle ID must be greater than 0", nameof(vehicleId));

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Product>(
                    "sp_GetProductsByVehicle",
                    new { VehicleId = vehicleId },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Product>> GetByDriverAsync(int driverId)
        {
            if (driverId <= 0)
                throw new ArgumentException("Driver ID must be greater than 0", nameof(driverId));

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Product>(
                    "sp_GetProductsByDriver",
                    new { DriverId = driverId },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Product>> GetUnassignedAsync()
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Product>(
                    "sp_GetUnassignedProducts",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> AssignProductToVehicleAsync(int productId, int vehicleId, int driverId)
        {
            if (productId <= 0)
                throw new ArgumentException("Product ID must be greater than 0", nameof(productId));
            if (vehicleId <= 0)
                throw new ArgumentException("Vehicle ID must be greater than 0", nameof(vehicleId));
            if (driverId <= 0)
                throw new ArgumentException("Driver ID must be greater than 0", nameof(driverId));

            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(
                    "sp_AssignProductToVehicle",
                    new { ProductId = productId, VehicleId = vehicleId, DriverId = driverId, UpdatedDate = DateTime.Now },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> UnassignProductAsync(int productId)
        {
            if (productId <= 0)
                throw new ArgumentException("Product ID must be greater than 0", nameof(productId));

            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(
                    "sp_UnassignProductFromVehicle",
                    new { ProductId = productId, UpdatedDate = DateTime.Now },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> UpdateProductStatusAsync(int productId, string status)
        {
            if (productId <= 0)
                throw new ArgumentException("Product ID must be greater than 0", nameof(productId));
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status cannot be empty", nameof(status));

            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(
                    "sp_UpdateProductStatus",
                    new { ProductId = productId, Status = status, UpdatedDate = DateTime.Now },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public override async Task<int> CreateAsync(Product entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (string.IsNullOrWhiteSpace(entity.ProductName))
                throw new ArgumentException("Product name is required", nameof(entity.ProductName));
            if (string.IsNullOrWhiteSpace(entity.SKU))
                throw new ArgumentException("SKU is required", nameof(entity.SKU));
            if (entity.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0", nameof(entity.Quantity));
            if (entity.UnitPrice <= 0)
                throw new ArgumentException("Unit price must be greater than 0", nameof(entity.UnitPrice));

            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>(
                    "sp_CreateProduct",
                    new
                    {
                        entity.ProductName,
                        entity.SKU,
                        entity.Description,
                        entity.Quantity,
                        entity.UnitPrice,
                        entity.VehicleId,
                        entity.DriverId,
                        Status = entity.Status ?? "Unassigned",
                        entity.CreatedDate,
                        entity.UpdatedDate
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public override async Task<int> UpdateAsync(Product entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (entity.ProductId <= 0)
                throw new ArgumentException("Product ID must be greater than 0", nameof(entity.ProductId));
            if (string.IsNullOrWhiteSpace(entity.ProductName))
                throw new ArgumentException("Product name is required", nameof(entity.ProductName));
            if (string.IsNullOrWhiteSpace(entity.SKU))
                throw new ArgumentException("SKU is required", nameof(entity.SKU));
            if (entity.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0", nameof(entity.Quantity));
            if (entity.UnitPrice <= 0)
                throw new ArgumentException("Unit price must be greater than 0", nameof(entity.UnitPrice));

            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(
                    "sp_UpdateProduct",
                    new
                    {
                        entity.ProductId,
                        entity.ProductName,
                        entity.SKU,
                        entity.Description,
                        entity.Quantity,
                        entity.UnitPrice,
                        entity.Status,
                        entity.UpdatedDate
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public override async Task<int> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Product ID must be greater than 0", nameof(id));

            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(
                    "sp_DeleteProduct",
                    new { ProductId = id },
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
