using FreshMarket.OrderService.Application.DTOs;
using FreshMarket.OrderService.Domain.Entities;

namespace FreshMarket.OrderService.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de carrito de compras
/// </summary>
public interface ICartService
{
    /// <summary>
    /// Obtener el carrito del usuario con información de productos
    /// </summary>
    Task<CartSummaryDto> GetCartAsync(int userId);

    /// <summary>
    /// Agregar un producto al carrito
    /// Si ya existe, incrementa la cantidad
    /// </summary>
    Task<CartItemDto> AddToCartAsync(int userId, AddToCartDto dto);

    /// <summary>
    /// Actualizar la cantidad de un producto en el carrito
    /// </summary>
    Task<CartItemDto> UpdateCartItemAsync(int userId, int productId, UpdateCartItemDto dto);

    /// <summary>
    /// Eliminar un producto del carrito
    /// </summary>
    Task<bool> RemoveFromCartAsync(int userId, int productId);

    /// <summary>
    /// Vaciar todo el carrito del usuario
    /// </summary>
    Task<bool> ClearCartAsync(int userId);

    /// <summary>
    /// Finalizar compra: Convierte todos los items del carrito en órdenes
    /// y vacía el carrito
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>Lista de órdenes creadas</returns>
    Task<CheckoutResponseDto> CheckoutAsync(int userId);
}

