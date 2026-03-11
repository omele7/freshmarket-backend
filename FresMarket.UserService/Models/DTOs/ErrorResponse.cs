﻿namespace FresMarket.UserService.Models.DTOs;

/// <summary>
/// Modelo de respuesta para errores
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Mensaje de error user-friendly
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Código de estado HTTP
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Diccionario de errores de validación (opcional)
    /// </summary>
    public Dictionary<string, string[]>? Errors { get; set; }

    /// <summary>
    /// Timestamp de cuando ocurrió el error
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// StackTrace de la excepción (solo en Development)
    /// </summary>
    public string? StackTrace { get; set; }
}

