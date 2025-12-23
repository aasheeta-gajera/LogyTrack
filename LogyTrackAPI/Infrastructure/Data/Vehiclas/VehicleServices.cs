using Dapper;
using LogyTrackAPI.Data;
using LogyTrackAPI.Infrastructure.Core;
using LogyTrackAPI.Infrastructure.Data.Vehiclas;
using LogyTrackAPI.Models;
using System.Data;

namespace LogyTrackAPI.Infrastructure.Data.Vehicles
{
    public class VehicleServices : BaseRepository<Vehicle>, IVehicleRepository
    {
        protected override string GetAllProcedure => "sp_GetAllVehicles";
        protected override string GetByIdProcedure => "sp_GetVehicleById";
        //protected override string CreateProcedure => "sp_CreateVehicle";
        //protected override string UpdateProcedure => "sp_UpdateVehicle";
        //protected override string DeleteProcedure => "sp_DeleteVehicle";

        // ← ADDED: Correct parameter name for Vehicles
        protected override string IdParameterName => "@VehicleId";

        public VehicleServices(DapperContext context) : base(context) { }

        public async Task<IEnumerable<Vehicle>> GetByStatusAsync(string status)
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Vehicle>(
                    "sp_GetByStatusVehicles",
                    new { Status = status },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<dynamic>> GetVehiclesWithDriverAsync()
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync(
                    "sp_GetVehiclesWithDriver",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> UpdateStatusAsync(int vehicleId, string status)
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(
                    "sp_UpdateVehicleStatus",
                    new { VehicleId = vehicleId, Status = status, UpdatedDate = DateTime.Now },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> AssignDriverAsync(int vehicleId, int driverId)
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(
                    "sp_AssignDriver",
                    new { VehicleId = vehicleId, DriverId = driverId, UpdatedDate = DateTime.Now },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> UnassignDriverAsync(int vehicleId)
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(
                    "sp_UnassignDriver",
                    new { VehicleId = vehicleId, UpdatedDate = DateTime.Now },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public override async Task<int> CreateAsync(Vehicle entity)
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>(
                    "sp_CreateVehicle",
                    new
                    {
                        entity.VehicleNumber,
                        entity.Model,
                        entity.CapacityKg,
                        entity.DriverId,
                        entity.Status,
                        entity.CreatedDate,
                        entity.UpdatedDate
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public override async Task<int> UpdateAsync(Vehicle entity)
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(
                    "sp_UpdateVehicle",
                    new
                    {
                        entity.VehicleId,
                        entity.VehicleNumber,
                        entity.Model,
                        entity.CapacityKg,
                        entity.Status,
                        entity.DriverId,
                        entity.UpdatedDate
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public override async Task<int> DeleteAsync(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(
                    "sp_DeleteVehicle",
                    new { VehicleId = id },
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}

