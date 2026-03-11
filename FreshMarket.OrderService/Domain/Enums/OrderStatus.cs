namespace FreshMarket.OrderService.Domain.Enums;

/// <summary>
/// Estado de una orden en el sistema
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Orden creada, esperando confirmación de pago
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Orden confirmada, pago procesado
    /// </summary>
    Confirmed = 1,

    /// <summary>
    /// Orden en proceso de preparación
    /// </summary>
    Processing = 2,

    /// <summary>
    /// Orden enviada al cliente
    /// </summary>
    Shipped = 3,

    /// <summary>
    /// Orden entregada exitosamente
    /// </summary>
    Delivered = 4,

    /// <summary>
    /// Orden cancelada por el cliente o sistema
    /// </summary>
    Cancelled = 5
}

