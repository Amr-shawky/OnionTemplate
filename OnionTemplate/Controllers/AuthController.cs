using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using OnionTemplate.Application.DTOs.User;
using OnionTemplate.Application.Interfaces;
using OnionTemplate.Application.Interfaces.Services;
using OnionTemplate.Core.Entities;
using OnionTemplate.Core.Enums;

namespace OnionTemplate.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IUnitOfWork unitOfWork,
            IAuthService authService,
            IMapper mapper,
            ILogger<AuthController> logger)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(CreateUserDto createUserDto)
        {
            try
            {
                // Check if email already exists
                var existingUser = await _unitOfWork.Users.GetByEmailAsync(createUserDto.Email);
                if (existingUser != null)
                {
                    return BadRequest("Email already exists");
                }

                // Create new user
                var user = _mapper.Map<User>(createUserDto);
                user.PasswordHash = await _authService.HashPasswordAsync(createUserDto.Password);
                user.Role = UserRole.Customer;

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var userDto = _mapper.Map<UserDto>(user);
                _logger.LogInformation("User registered successfully: {Email}", createUserDto.Email);

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user: {Email}", createUserDto.Email);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
        {
            try
            {
                // Find user by email
                var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    return Unauthorized("Invalid credentials");
                }

                // Validate password
                var isValidPassword = await _authService.ValidatePasswordAsync(loginDto.Password, user.PasswordHash);
                if (!isValidPassword)
                {
                    return Unauthorized("Invalid credentials");
                }

                // Generate JWT token
                var token = await _authService.GenerateJwtTokenAsync(user);
                
                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var response = new LoginResponseDto
                {
                    Token = token,
                    User = _mapper.Map<UserDto>(user),
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };

                _logger.LogInformation("User logged in successfully: {Email}", loginDto.Email);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in user: {Email}", loginDto.Email);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

