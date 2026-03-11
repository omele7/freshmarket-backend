using System.ComponentModel.DataAnnotations;

namespace FreshMarket.ProductService.Application.DTOs;

/// <summary>
/// DTO para actualizar un producto existente
/// Todas las propiedades son opcionales para permitir actualizaciones parciales
/// </summary>
public class UpdateProductDto
{
    /// <summary>
    /// Nombre del producto (opcional)
    /// </summary>
    [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres")]
    public string? Name { get; set; }
    
    /// <summary>
    /// Descripción del producto (opcional)
    /// </summary>
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 1000 caracteres")]
    public string? Description { get; set; }
    
    /// <summary>
    /// Precio del producto (opcional)
    /// </summary>
    [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre 0.01 y 999,999.99")]
    public decimal? Price { get; set; }
    
    /// <summary>
    /// Categoría del producto (opcional)
    /// </summary>
    [StringLength(100, MinimumLength = 3, ErrorMessage = "La categoría debe tener entre 3 y 100 caracteres")]
    public string? Category { get; set; }
    
    /// <summary>
    /// Cantidad en stock (opcional)
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int? Stock { get; set; }
    
    /// <summary>
    /// URL de la imagen del producto (opcional)
    /// </summary>
    [StringLength(500, ErrorMessage = "La URL de la imagen no puede exceder 500 caracteres")]
    [Url(ErrorMessage = "La URL de la imagen no es válida")]
    public string? ImageUrl { get; set; }
    
    /// <summary>
    /// Indica si el producto está disponible para la venta (opcional)
    /// </summary>
    public bool? IsAvailable { get; set; }
}

