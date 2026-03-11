namespace FreshMarket.OrderService.Domain.Entities;

/// <summary>
/// Entidad que representa un item individual dentro de una orden
/// Almacena un snapshot del producto al momento de la compra
/// </summary>
public class OrderItem
{
    /// <summary>
    /// Identificador único del item
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador de la orden a la que pertenece
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Navegación a la orden padre
    /// </summary>
    public Order? Order { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // SNAPSHOT DEL PRODUCTO (al momento de la compra)
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// ID del producto en el ProductService
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Nombre del producto (snapshot)
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Precio del producto al momento de la compra (snapshot)
    /// </summary>
    public decimal ProductPrice { get; set; }

    /// <summary>
    /// URL de la imagen del producto (snapshot)
    /// </summary>
    public string? ProductImageUrl { get; set; }

    /// <summary>
    /// Categoría del producto (snapshot)
    /// </summary>
    public string? ProductCategory { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // CANTIDAD Y SUBTOTAL
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Cantidad de este producto en la orden
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Subtotal del item (ProductPrice * Quantity)
    /// </summary>
    public decimal Subtotal { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // MÉTODOS DE DOMINIO
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Calcula el subtotal del item
    /// </summary>
    public void CalculateSubtotal()
    {
        Subtotal = ProductPrice * Quantity;
    }
}

