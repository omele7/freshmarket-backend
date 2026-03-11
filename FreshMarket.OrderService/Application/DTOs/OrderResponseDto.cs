﻿namespace FreshMarket.OrderService.Application.DTOs;

/// <summary>
/// DTO de respuesta que representa una orden creada
/// </summary>
public class OrderResponseDto
{
    /// <summary>
    /// Identificador único de la orden (auto-incremental)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del usuario que realizó el pedido
    /// Referencia a User.Id en UserService (tipo int)
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Identificador del producto pedido
    /// Referencia a Product.Id en ProductService (tipo int)
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Nombre del producto (obtenido del ProductService)
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Cantidad de productos en el pedido
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Precio unitario del producto al momento de la compra
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Precio total del pedido (Quantity * UnitPrice)
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Fecha y hora de creación del pedido
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

