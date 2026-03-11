﻿using FreshMarket.OrderService.Application.DTOs;
using FreshMarket.OrderService.Application.Exceptions;
using FreshMarket.OrderService.Application.Interfaces;
using FreshMarket.OrderService.Domain.Entities;
using FreshMarket.OrderService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FreshMarket.OrderService.Application.Services;

/// <summary>
/// Servicio de aplicación para la gestión del carrito de compras
/// </summary>
public class CartService : ICartService
{
    private readonly OrderDbContext _context;
    private readonly IProductServiceClient _productServiceClient;
    private readonly ILogger<CartService> _logger;
    private const decimal TaxRate = 0.18m; // IGV 18% - Perú

    public CartService(
        OrderDbContext context,
        IProductServiceClient productServiceClient,
        ILogger<CartService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _productServiceClient = productServiceClient ?? throw new ArgumentNullException(nameof(productServiceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtener el carrito del usuario con información completa de productos
    /// </summary>
    public async Task<CartSummaryDto> GetCartAsync(int userId)
    {
        _logger.LogInformation("Obteniendo carrito para UserId={UserId}", userId);

        // Obtener todos los items del carrito del usuario
        var cartItems = await _context.CartItems
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        if (!cartItems.Any())
        {
            _logger.LogInformation("Carrito vacío para UserId={UserId}", userId);
            return new CartSummaryDto
            {
                Items = new List<CartItemDto>(),
                TotalItems = 0,
                Subtotal = 0,
                Tax = 0,
                Total = 0
            };
        }

        // Obtener información de todos los productos del carrito
        var cartItemDtos = new List<CartItemDto>();
        decimal subtotal = 0;

        foreach (var item in cartItems)
        {
            try
            {
                // Llamar al ProductService para obtener información del producto
                var product = await _productServiceClient.GetProductByIdAsync(item.ProductId);

                if (product == null)
                {
                    _logger.LogWarning(
                        "Producto no encontrado en carrito: ProductId={ProductId}, eliminando del carrito",
                        item.ProductId);
                    
                    // Eliminar item del carrito si el producto ya no existe
                    _context.CartItems.Remove(item);
                    continue;
                }

                // Calcular subtotal del item
                var itemSubtotal = product.Price * item.Quantity;
                subtotal += itemSubtotal;

                // Crear DTO con información completa
                cartItemDtos.Add(new CartItemDto
                {
                    Id = item.Id,
                    UserId = item.UserId,
                    ProductId = item.ProductId,
                    ProductName = product.Name,
                    ProductPrice = product.Price,
                    ProductImageUrl = product.ImageUrl ?? string.Empty,
                    Quantity = item.Quantity,
                    Subtotal = itemSubtotal,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto ProductId={ProductId}", item.ProductId);
                // Continuar con los demás productos
            }
        }

        // Guardar cambios si se eliminaron items
        await _context.SaveChangesAsync();

        // Calcular tax y total
        var tax = subtotal * TaxRate;
        var total = subtotal + tax;

        var summary = new CartSummaryDto
        {
            Items = cartItemDtos,
            TotalItems = cartItemDtos.Sum(x => x.Quantity),
            Subtotal = subtotal,
            Tax = tax,
            Total = total
        };

        _logger.LogInformation(
            "Carrito obtenido: UserId={UserId}, Items={ItemCount}, Total={Total}",
            userId, cartItemDtos.Count, total);

        return summary;
    }

    /// <summary>
    /// Agregar un producto al carrito
    /// Si ya existe, incrementa la cantidad
    /// </summary>
    public async Task<CartItemDto> AddToCartAsync(int userId, AddToCartDto dto)
    {
        _logger.LogInformation(
            "Agregando al carrito: UserId={UserId}, ProductId={ProductId}, Quantity={Quantity}",
            userId, dto.ProductId, dto.Quantity);

        // Validar que el producto existe y obtener su información
        var product = await _productServiceClient.GetProductByIdAsync(dto.ProductId);
        
        if (product == null)
        {
            throw new ProductNotFoundException(dto.ProductId);
        }

        // Validar stock disponible
        if (product.Stock < dto.Quantity)
        {
            throw new InsufficientStockException(dto.ProductId, dto.Quantity, product.Stock);
        }

        // Verificar si el producto ya está en el carrito
        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);

        CartItem cartItem;

        if (existingItem != null)
        {
            // Producto ya existe: incrementar cantidad
            var newQuantity = existingItem.Quantity + dto.Quantity;

            // Validar stock total
            if (product.Stock < newQuantity)
            {
                throw new InsufficientStockException(dto.ProductId, newQuantity, product.Stock);
            }

            existingItem.IncrementQuantity(dto.Quantity);
            cartItem = existingItem;

            _logger.LogInformation(
                "Cantidad actualizada en carrito: CartItemId={Id}, NewQuantity={Quantity}",
                existingItem.Id, existingItem.Quantity);
        }
        else
        {
            // Producto nuevo: crear item
            cartItem = new CartItem(userId, dto.ProductId, dto.Quantity);
            _context.CartItems.Add(cartItem);

            _logger.LogInformation(
                "Nuevo item agregado al carrito: UserId={UserId}, ProductId={ProductId}",
                userId, dto.ProductId);
        }

        await _context.SaveChangesAsync();

        // Retornar DTO con información completa
        var itemSubtotal = product.Price * cartItem.Quantity;

        return new CartItemDto
        {
            Id = cartItem.Id,
            UserId = cartItem.UserId,
            ProductId = cartItem.ProductId,
            ProductName = product.Name,
            ProductPrice = product.Price,
            ProductImageUrl = product.ImageUrl ?? string.Empty,
            Quantity = cartItem.Quantity,
            Subtotal = itemSubtotal,
            CreatedAt = cartItem.CreatedAt,
            UpdatedAt = cartItem.UpdatedAt
        };
    }

    /// <summary>
    /// Actualizar la cantidad de un producto en el carrito
    /// </summary>
    public async Task<CartItemDto> UpdateCartItemAsync(int userId, int productId, UpdateCartItemDto dto)
    {
        _logger.LogInformation(
            "Actualizando item del carrito: UserId={UserId}, ProductId={ProductId}, NewQuantity={Quantity}",
            userId, productId, dto.Quantity);

        // Buscar el item en el carrito
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

        if (cartItem == null)
        {
            throw new CartItemNotFoundException(userId, productId);
        }

        // Validar que el producto existe y tiene stock
        var product = await _productServiceClient.GetProductByIdAsync(productId);
        
        if (product == null)
        {
            throw new ProductNotFoundException(productId);
        }

        if (product.Stock < dto.Quantity)
        {
            throw new InsufficientStockException(productId, dto.Quantity, product.Stock);
        }

        // Actualizar cantidad
        cartItem.UpdateQuantity(dto.Quantity);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Item del carrito actualizado: CartItemId={Id}, Quantity={Quantity}",
            cartItem.Id, cartItem.Quantity);

        var itemSubtotal = product.Price * cartItem.Quantity;

        return new CartItemDto
        {
            Id = cartItem.Id,
            UserId = cartItem.UserId,
            ProductId = cartItem.ProductId,
            ProductName = product.Name,
            ProductPrice = product.Price,
            ProductImageUrl = product.ImageUrl ?? string.Empty,
            Quantity = cartItem.Quantity,
            Subtotal = itemSubtotal,
            CreatedAt = cartItem.CreatedAt,
            UpdatedAt = cartItem.UpdatedAt
        };
    }

    /// <summary>
    /// Eliminar un producto del carrito
    /// </summary>
    public async Task<bool> RemoveFromCartAsync(int userId, int productId)
    {
        _logger.LogInformation(
            "Eliminando del carrito: UserId={UserId}, ProductId={ProductId}",
            userId, productId);

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

        if (cartItem == null)
        {
            _logger.LogWarning(
                "Item no encontrado en carrito: UserId={UserId}, ProductId={ProductId}",
                userId, productId);
            return false;
        }

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Item eliminado del carrito: CartItemId={Id}",
            cartItem.Id);

        return true;
    }

    /// <summary>
    /// Vaciar todo el carrito del usuario
    /// Resetea el IDENTITY si la tabla queda completamente vacía
    /// </summary>
    public async Task<bool> ClearCartAsync(int userId)
    {
        _logger.LogInformation("Vaciando carrito: UserId={UserId}", userId);

        var cartItems = await _context.CartItems
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!cartItems.Any())
        {
            _logger.LogInformation("Carrito ya está vacío: UserId={UserId}", userId);
            return false;
        }

        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Carrito vaciado: UserId={UserId}, ItemsRemoved={Count}",
            userId, cartItems.Count);

        // Verificar si la tabla CartItems está completamente vacía
        var remainingItems = await _context.CartItems.AnyAsync();
        
        if (!remainingItems)
        {
            // Resetear el IDENTITY counter si no quedan items en la tabla
            try
            {
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('CartItems', RESEED, 0)");
                _logger.LogInformation("IDENTITY counter de CartItems reseteado a 0");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo resetear el IDENTITY counter de CartItems");
                // No lanzar excepción, solo log - el carrito se vació correctamente
            }
        }

        return true;
    }

    public async Task<CheckoutResponseDto> CheckoutAsync(int userId)
    {
        _logger.LogInformation("Iniciando checkout para UserId={UserId}", userId);

        var cartItems = await _context.CartItems
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!cartItems.Any())
        {
            throw new InvalidOperationException("El carrito está vacío. No se puede realizar el checkout.");
        }

        var maxOrderNumber = await _context.Orders
            .MaxAsync(o => (int?)o.OrderNumber) ?? 0;
        
        var newOrderNumber = maxOrderNumber + 1;

        _logger.LogInformation("Nuevo OrderNumber generado: {OrderNumber}", newOrderNumber);

        var orders = new List<OrderDto>();
        decimal subtotal = 0;

        foreach (var cartItem in cartItems)
        {
            try
            {
                _logger.LogInformation(
                    "🔍 Procesando item del carrito: ProductId={ProductId}, Quantity={Quantity}",
                    cartItem.ProductId, cartItem.Quantity);

                var product = await _productServiceClient.GetProductByIdAsync(cartItem.ProductId);

                if (product == null)
                {
                    _logger.LogWarning(
                        "❌ Producto no encontrado durante checkout: ProductId={ProductId}, omitiendo...",
                        cartItem.ProductId);
                    continue;
                }

                _logger.LogInformation(
                    "✅ Producto obtenido: ProductId={ProductId}, Name={Name}, Stock={Stock}, Price={Price}",
                    product.Id, product.Name, product.Stock, product.Price);

                if (product.Stock < cartItem.Quantity)
                {
                    _logger.LogWarning(
                        "Stock insuficiente durante checkout: ProductId={ProductId}, Stock={Stock}, Requerido={Quantity}",
                        cartItem.ProductId, product.Stock, cartItem.Quantity);
                    
                    throw new InsufficientStockException(
                        cartItem.ProductId, 
                        cartItem.Quantity, 
                        product.Stock);
                }

                var order = new Order(
                    userId: userId,
                    productId: cartItem.ProductId,
                    quantity: cartItem.Quantity,
                    unitPrice: product.Price
                );
                
                order.OrderNumber = newOrderNumber;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                orders.Add(new OrderDto
                {
                    Id = order.Id,
                    OrderNumber = order.OrderNumber,
                    UserId = order.UserId,
                    ProductId = order.ProductId,
                    ProductName = product.Name,
                    Quantity = order.Quantity,
                    UnitPrice = order.UnitPrice,
                    TotalPrice = order.TotalPrice,
                    CreatedAt = order.CreatedAt
                });

                subtotal += order.TotalPrice;

                _logger.LogInformation(
                    "Orden creada desde carrito: OrderId={OrderId}, OrderNumber={OrderNumber}, ProductId={ProductId}, Total={Total}",
                    order.Id, order.OrderNumber, cartItem.ProductId, order.TotalPrice);
            }
            catch (InsufficientStockException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error al procesar item del carrito durante checkout: ProductId={ProductId}",
                    cartItem.ProductId);
                
                continue;
            }
        }

        if (!orders.Any())
        {
            throw new InvalidOperationException(
                "No se pudo procesar ningún item del carrito. Verifica que los productos existan y tengan stock.");
        }

        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Carrito vaciado después de checkout: UserId={UserId}, ItemsRemoved={Count}",
            userId, cartItems.Count);

        var remainingItems = await _context.CartItems.AnyAsync();
        if (!remainingItems)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('CartItems', RESEED, 0)");
                _logger.LogInformation("IDENTITY counter de CartItems reseteado después de checkout");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo resetear el IDENTITY counter de CartItems");
            }
        }

        var tax = subtotal * TaxRate;
        var total = subtotal + tax;

        var response = new CheckoutResponseDto
        {
            OrderNumber = newOrderNumber,
            Orders = orders,
            TotalItems = orders.Sum(o => o.Quantity),
            Subtotal = subtotal,
            Tax = tax,
            Total = total,
            Message = $"¡Compra realizada exitosamente! Pedido #{newOrderNumber} - Total: S/ {total:F2}"
        };

        _logger.LogInformation(
            "Checkout completado: UserId={UserId}, OrderNumber={OrderNumber}, Órdenes={OrderCount}, Total={Total}",
            userId, newOrderNumber, orders.Count, total);

        return response;
    }
}

