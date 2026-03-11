﻿namespace FreshMarket.OrderService.Application.DTOs;

/// <summary>
/// Comando para crear una nueva orden
/// Representa la solicitud de creación de un pedido
/// </summary>
public class CreateOrderCommand
{
    /// <summary>
    /// Identificador del usuario que realiza el pedido
    /// Referencia a User.Id en UserService (tipo int)
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Identificador del producto a pedir
    /// Referencia a Product.Id en ProductService (tipo int)
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Cantidad de productos a pedir
    /// Debe ser mayor a cero
    /// </summary>
    public int Quantity { get; set; }
}

