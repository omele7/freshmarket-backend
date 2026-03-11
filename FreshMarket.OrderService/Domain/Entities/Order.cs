namespace FreshMarket.OrderService.Domain.Entities;

public class Order
{
    public int Id { get; set; }

    public int OrderNumber { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public Order()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public Order(int userId, int productId, int quantity, decimal unitPrice)
    {
        if (userId <= 0)
            throw new ArgumentException("El ID de usuario debe ser mayor a cero", nameof(userId));

        if (productId <= 0)
            throw new ArgumentException("El ID de producto debe ser mayor a cero", nameof(productId));

        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero", nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException("El precio unitario no puede ser negativo", nameof(unitPrice));

        UserId = userId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = CalculateTotalPrice(quantity, unitPrice);
        CreatedAt = DateTime.UtcNow;
    }

    private static decimal CalculateTotalPrice(int quantity, decimal unitPrice)
    {
        return quantity * unitPrice;
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new InvalidOperationException("La cantidad debe ser mayor a cero");

        Quantity = newQuantity;
        TotalPrice = CalculateTotalPrice(Quantity, UnitPrice);
    }

    public void UpdateUnitPrice(decimal newUnitPrice)
    {
        if (newUnitPrice < 0)
            throw new InvalidOperationException("El precio unitario no puede ser negativo");

        UnitPrice = newUnitPrice;
        TotalPrice = CalculateTotalPrice(Quantity, UnitPrice);
    }

    public void RecalculateTotalPrice()
    {
        TotalPrice = CalculateTotalPrice(Quantity, UnitPrice);
    }
}
