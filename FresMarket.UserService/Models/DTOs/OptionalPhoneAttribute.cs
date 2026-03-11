using System.ComponentModel.DataAnnotations;

namespace FresMarket.UserService.Models.DTOs;

/// <summary>
/// Atributo de validación personalizado para teléfonos opcionales.
/// Solo valida el formato si el valor no es nulo ni vacío.
/// </summary>
public class OptionalPhoneAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Si es null o string vacío, es válido (campo opcional)
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return ValidationResult.Success;
        }

        // Si tiene valor, validar formato de teléfono
        var phoneValue = value.ToString()!;
        
        // Validación básica de teléfono (acepta números, espacios, guiones, paréntesis, +)
        var phonePattern = @"^[\d\s\-\(\)\+]+$";
        
        if (!System.Text.RegularExpressions.Regex.IsMatch(phoneValue, phonePattern))
        {
            return new ValidationResult(ErrorMessage ?? "El formato del teléfono es inválido");
        }

        // Validar longitud
        if (phoneValue.Length > 20)
        {
            return new ValidationResult("El teléfono no puede tener más de 20 caracteres");
        }

        return ValidationResult.Success;
    }
}

