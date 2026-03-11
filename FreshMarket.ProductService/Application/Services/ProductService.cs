using FreshMarket.ProductService.Application.DTOs;
using FreshMarket.ProductService.Application.Interfaces;
using FreshMarket.ProductService.Domain.Entities;

namespace FreshMarket.ProductService.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToResponseDto);
    }

    public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El ID del producto debe ser mayor que cero", nameof(id));

        var product = await _productRepository.GetByIdAsync(id);
        return product != null ? MapToResponseDto(product) : null;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("La categoría no puede estar vacía", nameof(category));

        var products = await _productRepository.GetByCategoryAsync(category);
        return products.Select(MapToResponseDto);
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAvailableProductsAsync()
    {
        var allProducts = await _productRepository.GetAllAsync();
        var availableProducts = allProducts.Where(p => p.IsAvailable && p.Stock > 0);
        return availableProducts.Select(MapToResponseDto);
    }

    public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        if (createProductDto == null)
            throw new ArgumentNullException(nameof(createProductDto), "Los datos del producto no pueden ser nulos");

        ValidateProductName(createProductDto.Name);
        ValidateProductDescription(createProductDto.Description);
        ValidateProductPrice(createProductDto.Price);
        ValidateProductCategory(createProductDto.Category);
        ValidateProductStock(createProductDto.Stock);

        var product = new Product
        {
            Name = createProductDto.Name.Trim(),
            Description = createProductDto.Description.Trim(),
            Price = createProductDto.Price,
            Category = createProductDto.Category.Trim(),
            Stock = createProductDto.Stock,
            ImageUrl = createProductDto.ImageUrl?.Trim(),
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdProduct = await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();
        
        return MapToResponseDto(createdProduct);
    }

    public async Task<ProductResponseDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
    {
        if (id <= 0)
            throw new ArgumentException("El ID del producto debe ser mayor que cero", nameof(id));

        if (updateProductDto == null)
            throw new ArgumentNullException(nameof(updateProductDto), "Los datos de actualización no pueden ser nulos");

        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Producto con ID {id} no encontrado");

        if (!string.IsNullOrWhiteSpace(updateProductDto.Name))
        {
            ValidateProductName(updateProductDto.Name);
            product.Name = updateProductDto.Name.Trim();
        }

        if (!string.IsNullOrWhiteSpace(updateProductDto.Description))
        {
            ValidateProductDescription(updateProductDto.Description);
            product.Description = updateProductDto.Description.Trim();
        }

        if (updateProductDto.Price.HasValue)
        {
            ValidateProductPrice(updateProductDto.Price.Value);
            product.UpdatePrice(updateProductDto.Price.Value);
        }

        if (!string.IsNullOrWhiteSpace(updateProductDto.Category))
        {
            ValidateProductCategory(updateProductDto.Category);
            product.Category = updateProductDto.Category.Trim();
        }

        if (updateProductDto.Stock.HasValue)
        {
            ValidateProductStock(updateProductDto.Stock.Value);
            product.UpdateStock(updateProductDto.Stock.Value);
        }

        if (updateProductDto.ImageUrl != null)
        {
            product.ImageUrl = string.IsNullOrWhiteSpace(updateProductDto.ImageUrl) 
                ? null 
                : updateProductDto.ImageUrl.Trim();
        }

        if (updateProductDto.IsAvailable.HasValue)
        {
            product.IsAvailable = updateProductDto.IsAvailable.Value;
        }

        product.UpdatedAt = DateTime.UtcNow;

        var updatedProduct = await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();
        
        return MapToResponseDto(updatedProduct);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El ID del producto debe ser mayor que cero", nameof(id));

        var deleted = await _productRepository.DeleteAsync(id);
        
        if (!deleted)
            throw new KeyNotFoundException($"Producto con ID {id} no encontrado");

        await _productRepository.SaveChangesAsync();
        return true;
    }

    private static void ValidateProductName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del producto es requerido", nameof(name));

        if (name.Length < 3)
            throw new ArgumentException("El nombre del producto debe tener al menos 3 caracteres", nameof(name));

        if (name.Length > 150)
            throw new ArgumentException("El nombre del producto no puede exceder 150 caracteres", nameof(name));
    }

    private static void ValidateProductDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("La descripción del producto es requerida", nameof(description));

        if (description.Length < 10)
            throw new ArgumentException("La descripción debe tener al menos 10 caracteres", nameof(description));

        if (description.Length > 1000)
            throw new ArgumentException("La descripción no puede exceder 1000 caracteres", nameof(description));
    }

    private static void ValidateProductPrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException("El precio no puede ser negativo", nameof(price));

        if (price == 0)
            throw new ArgumentException("El precio debe ser mayor que cero", nameof(price));

        if (price > 999999.99m)
            throw new ArgumentException("El precio no puede exceder 999,999.99", nameof(price));
    }

    private static void ValidateProductCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("La categoría del producto es requerida", nameof(category));

        if (category.Length < 3)
            throw new ArgumentException("La categoría debe tener al menos 3 caracteres", nameof(category));

        if (category.Length > 100)
            throw new ArgumentException("La categoría no puede exceder 100 caracteres", nameof(category));
    }

    private static void ValidateProductStock(int stock)
    {
        if (stock < 0)
            throw new ArgumentException("El stock no puede ser negativo", nameof(stock));
    }

    private static ProductResponseDto MapToResponseDto(Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Category = product.Category,
            Stock = product.Stock,
            ImageUrl = product.ImageUrl,
            IsAvailable = product.IsAvailable,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}

