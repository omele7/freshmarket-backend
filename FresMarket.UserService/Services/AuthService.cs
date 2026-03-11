using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FresMarket.UserService.Data;
using FresMarket.UserService.Models;
using FresMarket.UserService.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FresMarket.UserService.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            request.NormalizePhone();

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                throw new InvalidOperationException("El usuario con este email ya existe");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id.ToString(),
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    CreatedAt = user.CreatedAt,
                    Address = null
                },
                ExpiresIn = 86400
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al registrar usuario: {ex.Message}", ex);
        }
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Credenciales inválidas");
            }

            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id.ToString(),
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    CreatedAt = user.CreatedAt,
                    Address = user.Address != null ? new AddressDto
                    {
                        Id = user.Address.Id.ToString(),
                        Street = user.Address.Street,
                        City = user.Address.City,
                        State = user.Address.State,
                        ZipCode = user.Address.ZipCode,
                        Country = user.Address.Country,
                        IsDefault = user.Address.IsDefault,
                        CreatedAt = user.Address.CreatedAt
                    } : null
                },
                ExpiresIn = 86400
            };
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al iniciar sesión: {ex.Message}", ex);
        }
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            return new UserDto
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                CreatedAt = user.CreatedAt,
                Address = user.Address != null ? new AddressDto
                {
                    Id = user.Address.Id.ToString(),
                    Street = user.Address.Street,
                    City = user.Address.City,
                    State = user.Address.State,
                    ZipCode = user.Address.ZipCode,
                    Country = user.Address.Country,
                    IsDefault = user.Address.IsDefault,
                    CreatedAt = user.Address.CreatedAt
                } : null
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener usuario: {ex.Message}", ex);
        }
    }

    public string GenerateJwtToken(User user)
    {
        try
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("SecretKey no configurada");
            var issuer = jwtSettings["Issuer"] ?? "FreshMarket.UserService";
            var audience = jwtSettings["Audience"] ?? "FreshMarket.Clients";
            var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "1440");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al generar token JWT: {ex.Message}", ex);
        }
    }
}

