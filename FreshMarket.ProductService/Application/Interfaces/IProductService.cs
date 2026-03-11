using FreshMarket.ProductService.Application.DTOs;

namespace FreshMarket.ProductService.Application.Interfaces;

/// <summary>
/// Interfaz del servicio de productos
/// Define el contrato para la lógica de negocio de productos
/// </summary>
public interface IProductService
{
    Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
    Task<ProductResponseDto?> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(string category);
    Task<IEnumerable<ProductResponseDto>> GetAvailableProductsAsync();
    Task<ProductResponseDto> CreateProductAsync(CreateProductDto createProductDto);
    Task<ProductResponseDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
    Task<bool> DeleteProductAsync(int id);
}

