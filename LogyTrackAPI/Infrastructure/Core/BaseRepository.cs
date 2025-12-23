using Dapper;
using LogyTrackAPI.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LogyTrackAPI.Infrastructure.Core
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly DapperContext _context;
        protected abstract string GetAllProcedure { get; }
        protected abstract string GetByIdProcedure { get; }
        //protected abstract string CreateProcedure { get; }
        //protected abstract string UpdateProcedure { get; }
        //protected abstract string DeleteProcedure { get; }

        // ← ADD THIS: Override in each service to specify correct ID parameter name
        protected abstract string IdParameterName { get; }

        public BaseRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<T>(
                    GetAllProcedure,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<T> GetByIdAsync(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                // ← FIXED: Use IdParameterName from derived class
                var parameters = new DynamicParameters();
                parameters.Add(IdParameterName, id);

                return await connection.QueryFirstOrDefaultAsync<T>(
                    GetByIdProcedure,
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public abstract Task<int> CreateAsync(T entity);
        public abstract Task<int> UpdateAsync(T entity);
        public abstract Task<int> DeleteAsync(int id);
    }
}