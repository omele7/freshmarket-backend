using FresMarket.UserService.Models.DTOs;
using FresMarket.UserService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FresMarket.UserService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Validación de modelo fallida para registro: {Email}", request.Email);
            return UnprocessableEntity(new ErrorResponse
            {
                Message = "Datos de registro inválidos",
                Errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                ),
                StatusCode = 422
            });
        }

        try
        {
            var result = await _authService.RegisterAsync(request);
            _logger.LogInformation("Usuario registrado exitosamente: {Email}", request.Email);
            return CreatedAtAction(nameof(GetCurrentUser), new { }, result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Conflicto al registrar usuario: {Message}", ex.Message);
            return Conflict(new ErrorResponse
            {
                Message = ex.Message,
                StatusCode = 409
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error interno al registrar usuario: {Email}", request.Email);
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error interno del servidor",
                StatusCode = 500
            });
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Validación de modelo fallida para login: {Email}", request.Email);
            return UnprocessableEntity(new ErrorResponse
            {
                Message = "Datos de login inválidos",
                Errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                ),
                StatusCode = 422
            });
        }

        try
        {
            var result = await _authService.LoginAsync(request);
            _logger.LogInformation("Usuario autenticado exitosamente: {Email}", request.Email);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Intento de inicio de sesión fallido: {Message}", ex.Message);
            return Unauthorized(new ErrorResponse
            {
                Message = ex.Message,
                StatusCode = 401
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error interno al iniciar sesión: {Email}", request.Email);
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error interno del servidor",
                StatusCode = 500
            });
        }
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        try
        {
            Console.WriteLine("====================================");
            Console.WriteLine("🔍 Endpoint /api/auth/me llamado");

            Console.WriteLine($"User.Identity.IsAuthenticated: {User.Identity?.IsAuthenticated}");
            Console.WriteLine($"User.Identity.Name: {User.Identity?.Name}");

            Console.WriteLine("📋 Claims en el token:");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"   {claim.Type} = {claim.Value}");
            }

            var userIdClaim = User.FindFirst("sub") 
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)
                ?? User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                Console.WriteLine("❌ Token JWT inválido o claim 'sub' no encontrado");
                _logger.LogWarning("Token JWT inválido o claim 'sub' no encontrado");
                return Unauthorized(new ErrorResponse
                {
                    Message = "Token inválido",
                    StatusCode = 401
                });
            }

            Console.WriteLine($"👤 User ID extraído: {userId}");

            var user = await _authService.GetUserByIdAsync(userId);

            if (user == null)
            {
                Console.WriteLine($"❌ Usuario con ID {userId} NO EXISTE en la BD");
                _logger.LogWarning("Usuario no encontrado: {UserId}", userId);
                return NotFound(new ErrorResponse
                {
                    Message = "Usuario no encontrado",
                    StatusCode = 404
                });
            }

            Console.WriteLine($"✅ Usuario encontrado: {user.Email}");
            Console.WriteLine("====================================");
            _logger.LogInformation("Información de usuario obtenida: {UserId}", userId);
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener información del usuario actual");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error interno del servidor",
                StatusCode = 500
            });
        }
    }
}

