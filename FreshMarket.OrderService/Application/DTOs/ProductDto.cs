namespace FreshMarket.OrderService.Application.DTOs;

/// <summary>
/// DTO que representa un producto obtenido desde el ProductService
/// Utilizado para comunicación entre microservicios
/// </summary>
public class ProductDto
{
    /// <summary>
    /// Identificador único del producto
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Nombre del producto
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Descripción del producto
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Precio del producto
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Categoría del producto
    /// </summary>
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// Cantidad en stock disponible
    /// </summary>
    public int Stock { get; set; }
    
    /// <summary>
    /// URL de la imagen del producto
    /// </summary>
    public string? ImageUrl { get; set; }
    
    /// <summary>
    /// Indica si el producto está disponible para la venta
    /// </summary>
    public bool IsAvailable { get; set; }
    
    /// <summary>
    /// Fecha de creación del producto
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Fecha de última actualización del producto
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

