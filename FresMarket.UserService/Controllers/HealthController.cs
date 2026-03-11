using Microsoft.AspNetCore.Mvc;

namespace FresMarket.UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Endpoint de salud básico
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "Healthy",
            service = "FreshMarket.UserService",
            timestamp = DateTime.UtcNow,
            version = "1.0.0"
        });
    }

    /// <summary>
    /// Endpoint de salud detallado
    /// </summary>
    [HttpGet("detailed")]
    public IActionResult GetDetailed()
    {
        return Ok(new
        {
            status = "Healthy",
            service = "FreshMarket.UserService",
            timestamp = DateTime.UtcNow,
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            uptime = TimeSpan.FromMilliseconds(Environment.TickCount64).ToString(),
            memoryUsage = $"{GC.GetTotalMemory(false) / 1024 / 1024} MB"
        });
    }
}

