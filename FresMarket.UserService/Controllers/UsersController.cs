using FresMarket.UserService.Models.DTOs;
using FresMarket.UserService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FresMarket.UserService.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPut("{userId}/profile")]
    [ProducesResponseType(typeof(UpdateUserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<UpdateUserProfileResponse>> UpdateProfile(
        [FromRoute] int userId,
        [FromBody] UpdateUserProfileRequest request)
    {
        if (!ValidateUserAuthorization(userId))
        {
            _logger.LogWarning("Usuario no autorizado intentó actualizar perfil de userId: {UserId}", userId);
            return Forbid();
        }

        try
        {
            var response = await _userService.UpdateProfileAsync(userId, request);
            _logger.LogInformation("Perfil actualizado exitosamente para userId: {UserId}", userId);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Usuario no encontrado: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar perfil de userId: {UserId}", userId);
            throw;
        }
    }

    [HttpPost("{userId}/address")]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<object>> SaveAddress(
        [FromRoute] int userId,
        [FromBody] SaveAddressRequest request)
    {
        if (!ValidateUserAuthorization(userId))
        {
            _logger.LogWarning("Usuario no autorizado intentó guardar dirección de userId: {UserId}", userId);
            return Forbid();
        }

        try
        {
            var existingAddress = await _userService.GetAddressAsync(userId);
            var isUpdate = existingAddress != null;

            var address = await _userService.SaveAddressAsync(userId, request);
            
            _logger.LogInformation("Dirección {Action} exitosamente para userId: {UserId}", 
                isUpdate ? "actualizada" : "creada", userId);

            if (isUpdate)
            {
                return Ok(new { address });
            }
            else
            {
                return CreatedAtAction(
                    nameof(GetAddress),
                    new { userId },
                    new { address });
            }
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Usuario no encontrado: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar dirección de userId: {UserId}", userId);
            throw;
        }
    }

    [HttpGet("{userId}/address")]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<AddressDto?>> GetAddress([FromRoute] int userId)
    {
        if (!ValidateUserAuthorization(userId))
        {
            _logger.LogWarning("Usuario no autorizado intentó obtener dirección de userId: {UserId}", userId);
            return Forbid();
        }

        try
        {
            var address = await _userService.GetAddressAsync(userId);
            _logger.LogInformation("Dirección obtenida para userId: {UserId}", userId);
            return Ok(address);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Usuario no encontrado: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener dirección de userId: {UserId}", userId);
            throw;
        }
    }

    [HttpDelete("{userId}/address")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAddress([FromRoute] int userId)
    {
        if (!ValidateUserAuthorization(userId))
        {
            _logger.LogWarning("Usuario no autorizado intentó eliminar dirección de userId: {UserId}", userId);
            return Forbid();
        }

        try
        {
            await _userService.DeleteAddressAsync(userId);
            _logger.LogInformation("Dirección eliminada exitosamente para userId: {UserId}", userId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Dirección no encontrada: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar dirección de userId: {UserId}", userId);
            throw;
        }
    }

    private bool ValidateUserAuthorization(int userId)
    {
        Console.WriteLine("====================================");
        Console.WriteLine($"🔒 Validando autorización para userId: {userId}");
        
        Console.WriteLine("📋 Claims disponibles:");
        foreach (var claim in User.Claims)
        {
            Console.WriteLine($"   {claim.Type} = {claim.Value}");
        }
        
        var authenticatedUserIdClaim = User.FindFirst("sub") 
            ?? User.FindFirst(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirst(ClaimTypes.NameIdentifier);
        
        if (authenticatedUserIdClaim == null || string.IsNullOrEmpty(authenticatedUserIdClaim.Value))
        {
            Console.WriteLine("❌ No se pudo obtener el claim 'sub' del token JWT");
            _logger.LogWarning("No se pudo obtener el claim 'sub' del token JWT");
            Console.WriteLine("====================================");
            return false;
        }

        var authenticatedUserId = authenticatedUserIdClaim.Value;
        Console.WriteLine($"👤 User ID del token: {authenticatedUserId}");
        Console.WriteLine($"🎯 User ID del path: {userId}");
        
        var isAuthorized = authenticatedUserId == userId.ToString();
        
        if (isAuthorized)
        {
            Console.WriteLine("✅ Usuario AUTORIZADO");
        }
        else
        {
            Console.WriteLine($"❌ Usuario NO AUTORIZADO (token userId: {authenticatedUserId}, path userId: {userId})");
        }
        
        Console.WriteLine("====================================");
        return isAuthorized;
    }
}

