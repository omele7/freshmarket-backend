namespace FreshMarket.OrderService.Application.DTOs;

public class OrderGroupDto
{
    public int Id { get; set; }

    public int OrderNumber { get; set; }

    public List<OrderItemDto> Items { get; set; } = new();

    public int TotalItems { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Tax { get; set; }

    public decimal Total { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}

