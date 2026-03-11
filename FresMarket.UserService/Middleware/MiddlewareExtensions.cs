namespace FresMarket.UserService.Middleware;

/// <summary>
/// Extension methods para configurar middleware personalizado
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Registra el middleware global de manejo de excepciones
    /// </summary>
    /// <param name="app">Application builder</param>
    /// <returns>Application builder para encadenamiento</returns>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }

    /// <summary>
    /// Registra el middleware de manejo de errores (legacy)
    /// </summary>
    /// <param name="app">Application builder</param>
    /// <returns>Application builder para encadenamiento</returns>
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}

