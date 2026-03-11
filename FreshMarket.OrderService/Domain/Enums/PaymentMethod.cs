namespace FreshMarket.OrderService.Domain.Enums;

/// <summary>
/// Método de pago utilizado en una orden
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Tarjeta de crédito
    /// </summary>
    CreditCard = 0,

    /// <summary>
    /// Tarjeta de débito
    /// </summary>
    DebitCard = 1,

    /// <summary>
    /// PayPal
    /// </summary>
    PayPal = 2,

    /// <summary>
    /// Pago en efectivo contra entrega
    /// </summary>
    Cash = 3
}

