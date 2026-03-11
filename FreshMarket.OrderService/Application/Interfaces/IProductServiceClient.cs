using FreshMarket.OrderService.Application.DTOs;

namespace FreshMarket.OrderService.Application.Interfaces;

/// <summary>
/// Interface para el cliente del ProductService
/// Define el contrato para la comunicación entre microservicios
/// </summary>
public interface IProductServiceClient
{
    /// <summary>
    /// Obtiene un producto por su ID desde el ProductService
    /// </summary>
    /// <param name="productId">ID del producto a buscar (int, no Guid)</param>
    /// <returns>ProductDto si existe, null si no se encuentra</returns>
    Task<ProductDto?> GetProductByIdAsync(int productId);
}

