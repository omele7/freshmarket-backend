using System.ComponentModel.DataAnnotations;

namespace FresMarket.UserService.Models.DTOs;

public class UpdateUserProfileRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Teléfono del usuario (OPCIONAL)
    /// Puede ser null o vacío. Solo valida formato si tiene valor.
    /// </summary>
    [OptionalPhone(ErrorMessage = "Formato de teléfono inválido")]
    [MaxLength(20)]
    public string? Phone { get; set; }
}

