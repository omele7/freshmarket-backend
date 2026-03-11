using System.ComponentModel.DataAnnotations;

namespace FreshMarket.OrderService.Application.DTOs;

/// <summary>
/// DTO para agregar un producto al carrito
/// </summary>
public class AddToCartDto
{
    /// <summary>
    /// ID del producto a agregar
    /// </summary>
    [Required(ErrorMessage = "El ProductId es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El ProductId debe ser mayor a 0")]
    public int ProductId { get; set; }

    /// <summary>
    /// Cantidad de unidades
    /// </summary>
    [Required(ErrorMessage = "La cantidad es requerida")]
    [Range(1, 9999, ErrorMessage = "La cantidad debe estar entre 1 y 9999")]
    public int Quantity { get; set; }
}

/// <summary>
/// DTO para actualizar la cantidad de un item en el carrito
/// </summary>
public class UpdateCartItemDto
{
    /// <summary>
    /// Nueva cantidad
    /// </summary>
    [Required(ErrorMessage = "La cantidad es requerida")]
    [Range(1, 9999, ErrorMessage = "La cantidad debe estar entre 1 y 9999")]
    public int Quantity { get; set; }
}

/// <summary>
/// DTO de respuesta con información del item del carrito
/// Incluye datos del producto
/// </summary>
public class CartItemDto
{
    /// <summary>
    /// ID del item en el carrito
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID del usuario
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// ID del producto
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Nombre del producto
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Precio del producto
    /// </summary>
    public decimal ProductPrice { get; set; }

    /// <summary>
    /// URL de la imagen del producto
    /// </summary>
    public string ProductImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Cantidad de unidades
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Subtotal (precio * cantidad)
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Fecha de creación
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Fecha de actualización
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO de respuesta con el resumen completo del carrito
/// </summary>
public class CartSummaryDto
{
    /// <summary>
    /// Lista de items en el carrito
    /// </summary>
    public List<CartItemDto> Items { get; set; } = new();

    /// <summary>
    /// Total de items (suma de cantidades)
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Subtotal (suma de todos los subtotales)
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Impuesto (18% IGV - Perú)
    /// </summary>
    public decimal Tax { get; set; }

    /// <summary>
    /// Total a pagar (subtotal + tax)
    /// </summary>
    public decimal Total { get; set; }
}

