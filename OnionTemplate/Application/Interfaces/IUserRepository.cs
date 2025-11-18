using OnionTemplate.Core.Entities;

namespace OnionTemplate.Application.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        Task<User?> GetUserWithOrdersAsync(Guid userId);
    }
}
