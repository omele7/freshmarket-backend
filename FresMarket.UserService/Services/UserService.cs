using FresMarket.UserService.Data;
using FresMarket.UserService.Models;
using FresMarket.UserService.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FresMarket.UserService.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateUserProfileResponse> UpdateProfileAsync(int userId, UpdateUserProfileRequest request)
    {
        // Buscar usuario
        var user = await _context.Users
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new KeyNotFoundException($"Usuario con ID {userId} no encontrado");
        }

        // Actualizar perfil
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        
        // Normalizar Phone: convertir strings vacíos a null
        user.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone;
        
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Mapear a DTO
        var userDto = MapUserToDto(user);

        return new UpdateUserProfileResponse
        {
            User = userDto,
            Message = "Perfil actualizado correctamente"
        };
    }

    public async Task<AddressDto> SaveAddressAsync(int userId, SaveAddressRequest request)
    {
        // Verificar que el usuario existe
        var user = await _context.Users
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new KeyNotFoundException($"Usuario con ID {userId} no encontrado");
        }

        Address address;

        if (user.Address != null)
        {
            // Actualizar dirección existente
            address = user.Address;
            address.Street = request.Street;
            address.City = request.City;
            address.State = request.State;
            address.ZipCode = request.ZipCode;
            address.Country = request.Country;
            address.IsDefault = request.IsDefault;
            address.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            // Crear nueva dirección
            address = new Address
            {
                UserId = userId,
                Street = request.Street,
                City = request.City,
                State = request.State,
                ZipCode = request.ZipCode,
                Country = request.Country,
                IsDefault = request.IsDefault,
                CreatedAt = DateTime.UtcNow
            };

            _context.Addresses.Add(address);
        }

        await _context.SaveChangesAsync();

        return MapAddressToDto(address);
    }

    public async Task<AddressDto?> GetAddressAsync(int userId)
    {
        // Verificar que el usuario existe
        var user = await _context.Users
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new KeyNotFoundException($"Usuario con ID {userId} no encontrado");
        }

        return user.Address != null ? MapAddressToDto(user.Address) : null;
    }

    public async Task DeleteAddressAsync(int userId)
    {
        // Buscar dirección del usuario
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.UserId == userId);

        if (address == null)
        {
            throw new KeyNotFoundException($"El usuario con ID {userId} no tiene dirección");
        }

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();
    }

    // Métodos auxiliares de mapeo
    private UserDto MapUserToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id.ToString(),
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone,
            CreatedAt = user.CreatedAt,
            Address = user.Address != null ? MapAddressToDto(user.Address) : null
        };
    }

    private AddressDto MapAddressToDto(Address address)
    {
        return new AddressDto
        {
            Id = address.Id.ToString(),
            Street = address.Street,
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode,
            Country = address.Country,
            IsDefault = address.IsDefault,
            CreatedAt = address.CreatedAt
        };
    }
}

