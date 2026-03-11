using System.ComponentModel.DataAnnotations;

namespace FreshMarket.ProductService.Application.DTOs;

/// <summary>
/// DTO para crear un nuevo producto
/// Contiene solo las propiedades necesarias para la creación
/// </summary>
public class CreateProductDto
{
    /// <summary>
    /// Nombre del producto
    /// </summary>
    [Required(ErrorMessage = "El nombre del producto es requerido")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Descripción del producto
    /// </summary>
    [Required(ErrorMessage = "La descripción del producto es requerida")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 1000 caracteres")]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Precio del producto
    /// </summary>
    [Required(ErrorMessage = "El precio del producto es requerido")]
    [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre 0.01 y 999,999.99")]
    public decimal Price { get; set; }
    
    /// <summary>
    /// Categoría del producto
    /// </summary>
    [Required(ErrorMessage = "La categoría del producto es requerida")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "La categoría debe tener entre 3 y 100 caracteres")]
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// Cantidad en stock
    /// </summary>
    [Required(ErrorMessage = "El stock del producto es requerido")]
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }
    
    /// <summary>
    /// URL de la imagen del producto (opcional)
    /// </summary>
    [StringLength(500, ErrorMessage = "La URL de la imagen no puede exceder 500 caracteres")]
    [Url(ErrorMessage = "La URL de la imagen no es válida")]
    public string? ImageUrl { get; set; }
}

