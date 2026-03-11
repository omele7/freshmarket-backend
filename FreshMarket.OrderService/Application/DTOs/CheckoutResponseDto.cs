namespace FreshMarket.OrderService.Application.DTOs;

public class CheckoutResponseDto
{
    public int OrderNumber { get; set; }
    
    public List<OrderDto> Orders { get; set; } = new();

    public int TotalItems { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Tax { get; set; }

    public decimal Total { get; set; }

    public string Message { get; set; } = string.Empty;
}

public class OrderDto
{
    public int Id { get; set; }
    public int OrderNumber { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
}

