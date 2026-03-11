using FreshMarket.OrderService.Domain.Enums;

namespace FreshMarket.OrderService.Domain.Entities;

/// <summary>
/// Entidad Order COMPLETA según especificación ORDER_SERVICE_BACKEND_SPEC.md
/// Soporta múltiples items, dirección de envío, estados, etc.
/// 
/// NOTA: Esta es la versión COMPLETA. La clase Order.cs actual es una versión simple.
/// Puedes reemplazar Order.cs con esta implementación cuando estés listo.
/// </summary>
public class OrderComplete
{
    /// <summary>
    /// Identificador único de la orden (auto-increment)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del usuario que realiza la orden
    /// </summary>
    public int UserId { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // ITEMS DE LA ORDEN (Relación 1:N)
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Lista de items en la orden
    /// </summary>
    public List<OrderItem> Items { get; set; } = new();

    // ═══════════════════════════════════════════════════════════════
    // COSTOS Y TOTALES
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Subtotal de todos los items (sin impuestos ni envío)
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Impuestos aplicados
    /// </summary>
    public decimal Tax { get; set; }

    /// <summary>
    /// Costo de envío
    /// </summary>
    public decimal DeliveryFee { get; set; }

    /// <summary>
    /// Total de la orden (Subtotal + Tax + DeliveryFee)
    /// </summary>
    public decimal Total { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // ESTADO DE LA ORDEN
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Estado actual de la orden
    /// </summary>
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    // ═══════════════════════════════════════════════════════════════
    // DIRECCIÓN DE ENVÍO
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// ID de la dirección de envío (si es tabla separada)
    /// </summary>
    public int? ShippingAddressId { get; set; }

    /// <summary>
    /// Dirección de envío (navegación)
    /// </summary>
    public ShippingAddress? ShippingAddress { get; set; }

    // Propiedades embebidas de dirección (alternativa a tabla separada)
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // PAGO
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Método de pago utilizado
    /// </summary>
    public PaymentMethod PaymentMethod { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // FECHAS Y TIEMPOS
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Fecha de creación de la orden
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de última actualización
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha estimada de entrega
    /// </summary>
    public DateTime? EstimatedDelivery { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // CONSTRUCTORES
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Constructor por defecto para EF Core
    /// </summary>
    public OrderComplete()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Status = OrderStatus.Pending;
        Items = new List<OrderItem>();
    }

    /// <summary>
    /// Constructor con parámetros básicos
    /// </summary>
    public OrderComplete(int userId, PaymentMethod paymentMethod)
    {
        UserId = userId;
        PaymentMethod = paymentMethod;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Items = new List<OrderItem>();
    }

    // ═══════════════════════════════════════════════════════════════
    // MÉTODOS DE DOMINIO
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Calcula el subtotal sumando todos los items
    /// </summary>
    public void CalculateSubtotal()
    {
        Subtotal = Items.Sum(i => i.Subtotal);
    }

    /// <summary>
    /// Calcula el total de la orden
    /// </summary>
    public void CalculateTotal()
    {
        Total = Subtotal + Tax + DeliveryFee;
    }

    /// <summary>
    /// Recalcula todos los totales
    /// </summary>
    public void RecalculateAllTotals()
    {
        // Recalcular subtotal de cada item
        foreach (var item in Items)
        {
            item.CalculateSubtotal();
        }

        // Recalcular totales de la orden
        CalculateSubtotal();
        CalculateTotal();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Agrega un item a la orden
    /// </summary>
    public void AddItem(OrderItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        item.OrderId = Id;
        item.CalculateSubtotal();
        Items.Add(item);
        
        RecalculateAllTotals();
    }

    /// <summary>
    /// Remueve un item de la orden
    /// </summary>
    public void RemoveItem(OrderItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        Items.Remove(item);
        RecalculateAllTotals();
    }

    /// <summary>
    /// Cambia el estado de la orden
    /// </summary>
    public void ChangeStatus(OrderStatus newStatus)
    {
        // Validar transiciones de estado
        if (Status == OrderStatus.Cancelled && newStatus != OrderStatus.Cancelled)
        {
            throw new InvalidOperationException("No se puede cambiar el estado de una orden cancelada");
        }

        if (Status == OrderStatus.Delivered && newStatus != OrderStatus.Delivered)
        {
            throw new InvalidOperationException("No se puede cambiar el estado de una orden entregada");
        }

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancela la orden
    /// </summary>
    public void Cancel()
    {
        if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
        {
            throw new InvalidOperationException(
                "No se puede cancelar una orden que ya fue enviada o entregada");
        }

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Confirma la orden (cambia de Pending a Confirmed)
    /// </summary>
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("Solo se pueden confirmar órdenes pendientes");
        }

        Status = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;

        // Calcular fecha estimada de entrega (5 días por defecto)
        EstimatedDelivery = DateTime.UtcNow.AddDays(5);
    }

    /// <summary>
    /// Establece la dirección de envío usando un objeto ShippingAddress
    /// </summary>
    public void SetShippingAddress(ShippingAddress address)
    {
        if (address == null)
            throw new ArgumentNullException(nameof(address));

        ShippingAddress = address;
        Street = address.Street;
        City = address.City;
        State = address.State;
        ZipCode = address.ZipCode;
        Country = address.Country;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Establece la dirección de envío usando propiedades individuales
    /// </summary>
    public void SetShippingAddress(string street, string city, string state, string zipCode, string country)
    {
        Street = street ?? throw new ArgumentNullException(nameof(street));
        City = city ?? throw new ArgumentNullException(nameof(city));
        State = state ?? throw new ArgumentNullException(nameof(state));
        ZipCode = zipCode ?? throw new ArgumentNullException(nameof(zipCode));
        Country = country ?? throw new ArgumentNullException(nameof(country));
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Valida que la orden esté lista para ser procesada
    /// </summary>
    public bool IsValid()
    {
        return Items.Any() &&
               Subtotal > 0 &&
               Total > 0 &&
               !string.IsNullOrEmpty(Street) &&
               !string.IsNullOrEmpty(City) &&
               !string.IsNullOrEmpty(Country);
    }
}

