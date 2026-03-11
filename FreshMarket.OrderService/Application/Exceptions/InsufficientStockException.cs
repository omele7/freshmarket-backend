namespace FreshMarket.OrderService.Application.Exceptions;

/// <summary>
/// Excepción lanzada cuando un producto no tiene stock suficiente
/// </summary>
public class InsufficientStockException : Exception
{
    public int ProductId { get; }
    public int RequestedQuantity { get; }
    public int AvailableStock { get; }

    public InsufficientStockException(int productId, int requestedQuantity, int availableStock)
        : base($"Stock insuficiente para el producto ID {productId}. Solicitado: {requestedQuantity}, Disponible: {availableStock}")
    {
        ProductId = productId;
        RequestedQuantity = requestedQuantity;
        AvailableStock = availableStock;
    }

    public InsufficientStockException(int productId, int requestedQuantity, int availableStock, string message)
        : base(message)
    {
        ProductId = productId;
        RequestedQuantity = requestedQuantity;
        AvailableStock = availableStock;
    }
}

