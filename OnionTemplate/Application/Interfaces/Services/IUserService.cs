using OnionTemplate.Application.DTOs.Common;
using OnionTemplate.Application.DTOs.User;

namespace OnionTemplate.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task<UserDto> RegisterAsync(CreateUserDto createUserDto);
        Task<UserDto> GetUserByIdAsync(Guid userId);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<PaginatedResult<UserDto>> GetUsersAsync(int page, int pageSize);
        Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
        Task<bool> VerifyEmailAsync(Guid userId);
    }
}

