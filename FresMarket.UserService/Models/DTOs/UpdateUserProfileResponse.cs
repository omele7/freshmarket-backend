namespace FresMarket.UserService.Models.DTOs;

public class UpdateUserProfileResponse
{
    public UserDto User { get; set; } = null!;
    public string Message { get; set; } = "Perfil actualizado correctamente";
}

