using System.Text.Json;
using FresMarket.UserService.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FresMarket.UserService.Middleware;

/// <summary>
/// Middleware global para manejo centralizado de excepciones
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var errorResponse = CreateErrorResponse(exception);
        
        // Loggear la excepción
        LogException(exception, context);

        // Configurar respuesta HTTP
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = errorResponse.StatusCode;

        // Serializar y retornar la respuesta
        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    private ErrorResponse CreateErrorResponse(Exception exception)
    {
        var errorResponse = new ErrorResponse
        {
            Timestamp = DateTime.UtcNow
        };

        switch (exception)
        {
            // ArgumentNullException → 400 Bad Request (debe ir antes de ArgumentException)
            case ArgumentNullException argNullEx:
                errorResponse.StatusCode = StatusCodes.Status400BadRequest;
                errorResponse.Message = $"El parámetro '{argNullEx.ParamName}' no puede ser nulo.";
                
                if (_environment.IsDevelopment())
                {
                    errorResponse.StackTrace = argNullEx.StackTrace;
                }
                break;

            // ArgumentException → 400 Bad Request
            case ArgumentException argEx:
                errorResponse.StatusCode = StatusCodes.Status400BadRequest;
                errorResponse.Message = argEx.Message;
                
                if (_environment.IsDevelopment())
                {
                    errorResponse.StackTrace = argEx.StackTrace;
                }
                break;

            // DbUpdateException → 409 Conflict
            case DbUpdateException dbEx:
                errorResponse.StatusCode = StatusCodes.Status409Conflict;
                errorResponse.Message = "Error al actualizar la base de datos. El recurso puede estar en conflicto.";
                
                // En Development, incluir más detalles
                if (_environment.IsDevelopment())
                {
                    errorResponse.Message = dbEx.InnerException?.Message ?? dbEx.Message;
                    errorResponse.StackTrace = dbEx.StackTrace;
                }
                break;

            // UnauthorizedAccessException → 401 Unauthorized
            case UnauthorizedAccessException unauthEx:
                errorResponse.StatusCode = StatusCodes.Status401Unauthorized;
                errorResponse.Message = "No tiene autorización para acceder a este recurso.";
                
                if (_environment.IsDevelopment())
                {
                    errorResponse.Message = unauthEx.Message;
                    errorResponse.StackTrace = unauthEx.StackTrace;
                }
                break;

            // KeyNotFoundException → 404 Not Found
            case KeyNotFoundException notFoundEx:
                errorResponse.StatusCode = StatusCodes.Status404NotFound;
                errorResponse.Message = "El recurso solicitado no fue encontrado.";
                
                if (_environment.IsDevelopment())
                {
                    errorResponse.Message = notFoundEx.Message;
                    errorResponse.StackTrace = notFoundEx.StackTrace;
                }
                break;

            // InvalidOperationException → 409 Conflict (para operaciones de negocio)
            case InvalidOperationException invOpEx:
                errorResponse.StatusCode = StatusCodes.Status409Conflict;
                errorResponse.Message = invOpEx.Message;
                
                if (_environment.IsDevelopment())
                {
                    errorResponse.StackTrace = invOpEx.StackTrace;
                }
                break;

            // TimeoutException → 408 Request Timeout
            case TimeoutException timeoutEx:
                errorResponse.StatusCode = StatusCodes.Status408RequestTimeout;
                errorResponse.Message = "La solicitud ha excedido el tiempo de espera.";
                
                if (_environment.IsDevelopment())
                {
                    errorResponse.Message = timeoutEx.Message;
                    errorResponse.StackTrace = timeoutEx.StackTrace;
                }
                break;

            // OperationCanceledException → 499 Client Closed Request
            case OperationCanceledException:
                errorResponse.StatusCode = 499; // Client Closed Request
                errorResponse.Message = "La operación fue cancelada.";
                break;

            // Otras excepciones → 500 Internal Server Error
            default:
                errorResponse.StatusCode = StatusCodes.Status500InternalServerError;
                errorResponse.Message = "Ha ocurrido un error interno en el servidor.";
                
                // En Development, incluir detalles completos
                if (_environment.IsDevelopment())
                {
                    errorResponse.Message = exception.Message;
                    errorResponse.StackTrace = exception.StackTrace;
                    
                    // Incluir inner exception si existe
                    if (exception.InnerException != null)
                    {
                        errorResponse.Errors = new Dictionary<string, string[]>
                        {
                            { "InnerException", new[] { exception.InnerException.Message } }
                        };
                    }
                }
                break;
        }

        return errorResponse;
    }

    private void LogException(Exception exception, HttpContext context)
    {
        var logLevel = GetLogLevel(exception);
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        _logger.Log(
            logLevel,
            exception,
            "Excepción capturada: {ExceptionType} - {Method} {Path} - {Message}",
            exception.GetType().Name,
            requestMethod,
            requestPath,
            exception.Message
        );

        // Log adicional para excepciones críticas
        if (logLevel == LogLevel.Critical || logLevel == LogLevel.Error)
        {
            _logger.LogError(
                "Detalles adicionales: Usuario={User}, IP={IP}, UserAgent={UserAgent}",
                context.User.Identity?.Name ?? "Anónimo",
                context.Connection.RemoteIpAddress?.ToString() ?? "Desconocida",
                context.Request.Headers["User-Agent"].ToString()
            );
        }
    }

    private static LogLevel GetLogLevel(Exception exception)
    {
        return exception switch
        {
            ArgumentException or ArgumentNullException => LogLevel.Warning,
            KeyNotFoundException => LogLevel.Information,
            UnauthorizedAccessException => LogLevel.Warning,
            OperationCanceledException => LogLevel.Information,
            _ => LogLevel.Error
        };
    }
}

