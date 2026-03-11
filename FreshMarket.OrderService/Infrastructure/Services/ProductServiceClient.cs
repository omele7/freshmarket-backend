using FreshMarket.OrderService.Application.DTOs;
using FreshMarket.OrderService.Application.Interfaces;
using System.Text.Json;

namespace FreshMarket.OrderService.Infrastructure.Services;

/// <summary>
/// Cliente HTTP para comunicación con el ProductService
/// Implementa el patrón de comunicación entre microservicios
/// </summary>
public class ProductServiceClient : IProductServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductServiceClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Constructor con inyección de dependencias
    /// </summary>
    /// <param name="httpClient">Cliente HTTP configurado</param>
    /// <param name="logger">Logger para registro de eventos</param>
    public ProductServiceClient(
        HttpClient httpClient,
        ILogger<ProductServiceClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Configurar opciones de serialización JSON
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = null // Mantener nombres originales
        };
    }

    /// <summary>
    /// Obtiene un producto por su ID desde el ProductService
    /// </summary>
    /// <param name="productId">ID del producto a buscar (int, no Guid)</param>
    /// <returns>ProductDto si existe, null si no se encuentra o hay error</returns>
    public async Task<ProductDto?> GetProductByIdAsync(int productId)
    {
        try
        {
            _logger.LogInformation(
                "🌐 Llamando a ProductService para obtener producto con ID: {ProductId}",
                productId);

            // Construir la URL del endpoint
            // GET http://localhost:5002/api/products/{id}
            var endpoint = $"api/products/{productId}";

            _logger.LogInformation("🔗 URL completa: {BaseAddress}{Endpoint}", 
                _httpClient.BaseAddress, endpoint);

            _logger.LogInformation("📊 HttpClient configuración - BaseAddress: {BaseAddress}, Timeout: {Timeout}", 
                _httpClient.BaseAddress, _httpClient.Timeout);

            // Realizar la petición HTTP GET
            var response = await _httpClient.GetAsync(endpoint);
            
            _logger.LogInformation("📡 RESPUESTA COMPLETA - StatusCode: {StatusCode}, ReasonPhrase: {ReasonPhrase}, Headers: {Headers}",
                (int)response.StatusCode, response.ReasonPhrase, response.Headers.ToString());

            _logger.LogInformation(
                "📡 Respuesta recibida: StatusCode={StatusCode}, IsSuccess={IsSuccess}",
                (int)response.StatusCode, response.IsSuccessStatusCode);

            // Verificar si el producto existe
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning(
                    "Producto con ID {ProductId} no encontrado en ProductService",
                    productId);
                return null;
            }

            // Verificar si la respuesta es exitosa
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Error al obtener producto {ProductId}. Status Code: {StatusCode}",
                    productId,
                    response.StatusCode);
                return null;
            }

            // Leer el contenido de la respuesta
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("📄 Contenido de respuesta (primeros 500 chars): {Content}", 
                content.Length > 500 ? content.Substring(0, 500) + "..." : content);

            // Deserializar el JSON a ProductDto
            var product = JsonSerializer.Deserialize<ProductDto>(content, _jsonOptions);

            if (product != null)
            {
                _logger.LogInformation(
                    "✅ Producto obtenido exitosamente: ID={ProductId}, Nombre={Name}, Precio={Price}, Stock={Stock}",
                    product.Id,
                    product.Name,
                    product.Price,
                    product.Stock);
            }
            else
            {
                _logger.LogWarning("⚠️  La deserialización del producto {ProductId} resultó en null. Contenido: {Content}", 
                    productId, content);
            }

            return product;
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(
                httpEx,
                "Error de conexión HTTP al intentar obtener producto {ProductId}. " +
                "Verifique que ProductService esté ejecutándose en https://localhost:5003",
                productId);
            return null;
        }
        catch (TaskCanceledException taskEx)
        {
            _logger.LogError(
                taskEx,
                "Timeout al intentar obtener producto {ProductId} desde ProductService",
                productId);
            return null;
        }
        catch (JsonException jsonEx)
        {
            _logger.LogError(
                jsonEx,
                "Error al deserializar la respuesta del ProductService para producto {ProductId}",
                productId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error inesperado al obtener producto {ProductId} desde ProductService",
                productId);
            return null;
        }
    }
}

