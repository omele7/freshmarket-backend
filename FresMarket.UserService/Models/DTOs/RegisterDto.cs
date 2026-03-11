using System.ComponentModel.DataAnnotations;

namespace FresMarket.UserService.Models.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es requerido")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [OptionalPhone(ErrorMessage = "Formato de teléfono inválido")]
    [StringLength(20)]
    public string? Phone { get; set; }
}

