namespace FreshMarket.OrderService.Application.Exceptions;

/// <summary>
/// Excepción lanzada cuando un item del carrito no es encontrado
/// </summary>
public class CartItemNotFoundException : Exception
{
    public int UserId { get; }
    public int ProductId { get; }

    public CartItemNotFoundException(int userId, int productId)
        : base($"El producto {productId} no fue encontrado en el carrito del usuario {userId}")
    {
        UserId = userId;
        ProductId = productId;
    }
}

