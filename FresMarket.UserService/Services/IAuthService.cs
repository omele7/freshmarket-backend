using FresMarket.UserService.Models;
using FresMarket.UserService.Models.DTOs;

namespace FresMarket.UserService.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<UserDto?> GetUserByIdAsync(int userId);
    string GenerateJwtToken(User user);
}

