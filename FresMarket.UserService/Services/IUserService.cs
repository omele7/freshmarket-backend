using FresMarket.UserService.Models.DTOs;

namespace FresMarket.UserService.Services;

public interface IUserService
{
    Task<UpdateUserProfileResponse> UpdateProfileAsync(int userId, UpdateUserProfileRequest request);
    Task<AddressDto> SaveAddressAsync(int userId, SaveAddressRequest request);
    Task<AddressDto?> GetAddressAsync(int userId);
    Task DeleteAddressAsync(int userId);
}

