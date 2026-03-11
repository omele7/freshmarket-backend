namespace FreshMarket.ProductService.Domain.Entities;

/// <summary>
/// Entidad de dominio que representa un producto en FreshMarket
/// </summary>
public class Product
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
    
    public string Category { get; set; } = string.Empty;
    
    public int Stock { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public bool IsAvailable { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Métodos de dominio
    public void UpdateStock(int quantity)
    {
        if (quantity < 0)
            throw new InvalidOperationException("El stock no puede ser negativo");
        
        Stock = quantity;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void DecrementStock(int quantity)
    {
        if (quantity <= 0)
            throw new InvalidOperationException("La cantidad debe ser mayor a cero");
        
        if (Stock < quantity)
            throw new InvalidOperationException("Stock insuficiente");
        
        Stock -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new InvalidOperationException("El precio no puede ser negativo");
        
        Price = newPrice;
        UpdatedAt = DateTime.UtcNow;
    }
}

