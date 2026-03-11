using System.ComponentModel.DataAnnotations;

namespace FresMarket.UserService.Models.DTOs;

public class RegisterRequest
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es requerido")]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Teléfono del usuario (OPCIONAL)
    /// </summary>
    [OptionalPhone(ErrorMessage = "Teléfono inválido")]
    [MaxLength(20)]
    public string? Phone { get; set; }

    /// <summary>
    /// Normaliza el campo Phone: convierte strings vacíos a null
    /// </summary>
    public void NormalizePhone()
    {
        if (string.IsNullOrWhiteSpace(Phone))
        {
            Phone = null;
        }
    }
}

