using logyTrack.Models;
using LogyTrackAPI.Infrastructure.Core;

namespace LogyTrackAPI.Infrastructure.Data.Customers
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByNameAsync(string name);
    }
}
