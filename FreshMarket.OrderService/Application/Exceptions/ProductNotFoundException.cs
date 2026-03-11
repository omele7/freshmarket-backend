namespace FreshMarket.OrderService.Application.Exceptions;

/// <summary>
/// Excepción lanzada cuando un producto no es encontrado en el ProductService
/// </summary>
public class ProductNotFoundException : Exception
{
    public int ProductId { get; }

    public ProductNotFoundException(int productId)
        : base($"El producto con ID {productId} no fue encontrado en el ProductService")
    {
        ProductId = productId;
    }

    public ProductNotFoundException(int productId, string message)
        : base(message)
    {
        ProductId = productId;
    }

    public ProductNotFoundException(int productId, string message, Exception innerException)
        : base(message, innerException)
    {
        ProductId = productId;
    }
}

