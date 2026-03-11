namespace FreshMarket.OrderService.Domain.Entities;

/// <summary>
/// Entidad de dominio que representa un item en el carrito de compras
/// Persiste el carrito del usuario en la base de datos
/// </summary>
public class CartItem
{
    /// <summary>
    /// Identificador único del item en el carrito
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del usuario dueño del carrito
    /// Referencia a User.Id en UserService
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Identificador del producto en el carrito
    /// Referencia a Product.Id en ProductService
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Cantidad de unidades del producto
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Fecha de creación del item
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Fecha de última actualización
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Constructor por defecto para EF Core
    /// </summary>
    public CartItem()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Constructor con parámetros
    /// </summary>
    public CartItem(int userId, int productId, int quantity)
    {
        if (userId <= 0)
            throw new ArgumentException("El ID de usuario debe ser mayor a cero", nameof(userId));

        if (productId <= 0)
            throw new ArgumentException("El ID de producto debe ser mayor a cero", nameof(productId));

        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero", nameof(quantity));

        UserId = userId;
        ProductId = productId;
        Quantity = quantity;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Actualiza la cantidad y la fecha de modificación
    /// </summary>
    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero", nameof(quantity));

        Quantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Incrementa la cantidad en el valor especificado
    /// </summary>
    public void IncrementQuantity(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("El incremento debe ser mayor a cero", nameof(amount));

        Quantity += amount;
        UpdatedAt = DateTime.UtcNow;
    }
}

