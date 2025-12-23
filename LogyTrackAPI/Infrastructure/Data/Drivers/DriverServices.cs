using Dapper;
using LogyTrackAPI.Data;
using LogyTrackAPI.Infrastructure.Core;
using LogyTrackAPI.Models;
using System.Data;

namespace LogyTrackAPI.Infrastructure.Data.Drivers
{
    public class DriverServices : BaseRepository<Driver>, IDriverRepository
    {
        protected override string GetAllProcedure => "sp_GetAllDrivers";
        protected override string GetByIdProcedure => "sp_GetDriverById";

        //protected override string CreateProcedure => "sp_CreateDriver";
        //protected override string UpdateProcedure => "sp_UpdateDriver";
        //protected override string DeleteProcedure => "sp_DeleteDriver";

        // ← ADDED: Correct parameter name for Drivers
        protected override string IdParameterName => "@DriverId";

        public DriverServices(DapperContext context) : base(context) { }

        public async Task<IEnumerable<dynamic>> GetDriversWithVehiclesAsync()
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync(
                    "sp_GetDriversWithVehicles",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Driver>> GetActiveDriversAsync()
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Driver>(
                    "sp_GetActiveDrivers",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public override async Task<int> CreateAsync(Driver entity)
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>(
                    "sp_CreateDriver",
                    new
                    {
                        entity.FullName,
                        entity.PhoneNumber,
                        entity.LicenseNumber,
                        entity.IsActive,
                        entity.CreatedDate,
                        entity.UpdatedDate
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public override async Task<int> UpdateAsync(Driver entity)
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(
                    "sp_UpdateDriver",
                    new
                    {
                        entity.DriverId,
                        entity.FullName,
                        entity.PhoneNumber,
                        entity.LicenseNumber,
                        entity.IsActive,
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
                    "sp_DeleteDriver",
                    new { DriverId = id },
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}