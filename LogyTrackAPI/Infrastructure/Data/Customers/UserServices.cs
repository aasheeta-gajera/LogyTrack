using Dapper;
using LogyTrackAPI.Data;
using logyTrack.Models;
using LogyTrackAPI.Infrastructure.Core;
using System.Data;

namespace LogyTrackAPI.Infrastructure.Data.Customers
{
    public class UserServices : BaseRepository<User>, IUserRepository
    {
        protected override string GetAllProcedure => "sp_GetAllUsers";
        protected override string GetByIdProcedure => "sp_GetUserById";
        //protected override string CreateProcedure => "sp_CreateUser";
        //protected override string UpdateProcedure => "sp_UpdateUser";
        //protected override string DeleteProcedure => "sp_DeleteUser";

        // ← ADDED: Correct parameter name for Users
        protected override string IdParameterName => "@UserId";

        public UserServices(DapperContext context) : base(context) { }

        public async Task<User> GetByNameAsync(string name)
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<User>(
                    "sp_GetUserByName",
                    new { Name = name },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public override async Task<int> CreateAsync(User entity)
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>(
                    "sp_CreateUser",
                    new
                    {
                        entity.NAME,
                        entity.PASS,
                        entity.TYPE,
                        entity.CreatedDate
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public override async Task<int> UpdateAsync(User entity)
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(
                    "sp_UpdateUser",
                    new
                    {
                        UserId = entity.ID,
                        entity.NAME,
                        entity.PASS,
                        entity.TYPE
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public override async Task<int> DeleteAsync(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(
                    "sp_DeleteUser",
                    new { UserId = id },
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}