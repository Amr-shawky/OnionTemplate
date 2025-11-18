using OnionTemplate.Core.Entities;

namespace OnionTemplate.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<string> GenerateJwtTokenAsync(User user);
        Task<bool> ValidatePasswordAsync(string password, string hashedPassword);
        Task<string> HashPasswordAsync(string password);
        Task<bool> ValidateTokenAsync(string token);
        Task<Guid?> GetUserIdFromTokenAsync(string token);
    }
}

