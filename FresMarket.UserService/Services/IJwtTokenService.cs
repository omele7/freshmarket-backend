using FresMarket.UserService.Models;

namespace FresMarket.UserService.Services;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}

