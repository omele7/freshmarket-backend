using FreshMarket.OrderService.Application.DTOs;
using FreshMarket.OrderService.Application.Exceptions;
using FreshMarket.OrderService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreshMarket.OrderService.Controllers;

/// <summary>
/// Controlador para la gestión del carrito de compras
/// Endpoints REST para CRUD del carrito persistente
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ILogger<CartController> _logger;

    public CartController(
        ICartService cartService,
        ILogger<CartController> logger)
    {
        _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtener el carrito del usuario autenticado
    /// </summary>
    /// <returns>Resumen del carrito con items y totales</returns>
    /// <response code="200">Carrito obtenido exitosamente</response>
    /// <response code="401">Usuario no autenticado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(CartSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CartSummaryDto>> GetCart()
    {
        try
        {
            // TODO: En producción, obtener userId del token JWT
            // var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            
            // Por ahora, usar un userId de prueba desde header o query
            var userId = GetUserIdFromRequest();

            if (userId == 0)
            {
                return Unauthorized(new ErrorResponse
                {
                    Message = "Usuario no autenticado. Incluir header 'X-User-Id' o query param 'userId'",
                    ErrorCode = "UNAUTHORIZED"
                });
            }

            _logger.LogInformation("GET /api/cart - UserId={UserId}", userId);

            var cart = await _cartService.GetCartAsync(userId);

            return Ok(cart);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener carrito");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error al obtener el carrito",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Agregar un producto al carrito
    /// Si el producto ya existe, incrementa la cantidad
    /// </summary>
    /// <param name="dto">Datos del producto a agregar</param>
    /// <returns>Item del carrito creado o actualizado</returns>
    /// <response code="200">Producto agregado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="401">Usuario no autenticado</response>
    /// <response code="404">Producto no encontrado</response>
    /// <response code="422">Stock insuficiente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("items")]
    [ProducesResponseType(typeof(CartItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CartItemDto>> AddToCart([FromBody] AddToCartDto dto)
    {
        try
        {
            var userId = GetUserIdFromRequest();

            if (userId == 0)
            {
                return Unauthorized(new ErrorResponse
                {
                    Message = "Usuario no autenticado",
                    ErrorCode = "UNAUTHORIZED"
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Datos inválidos",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            _logger.LogInformation(
                "POST /api/cart/items - UserId={UserId}, ProductId={ProductId}, Quantity={Quantity}",
                userId, dto.ProductId, dto.Quantity);

            var cartItem = await _cartService.AddToCartAsync(userId, dto);

            return Ok(cartItem);
        }
        catch (ProductNotFoundException ex)
        {
            _logger.LogWarning(ex, "Producto no encontrado: ProductId={ProductId}", ex.ProductId);
            return NotFound(new ErrorResponse
            {
                Message = ex.Message,
                ErrorCode = "PRODUCT_NOT_FOUND",
                Details = new { ProductId = ex.ProductId }
            });
        }
        catch (InsufficientStockException ex)
        {
            _logger.LogWarning(
                ex,
                "Stock insuficiente: ProductId={ProductId}, Requested={Requested}, Available={Available}",
                ex.ProductId, ex.RequestedQuantity, ex.AvailableStock);
            
            return UnprocessableEntity(new ErrorResponse
            {
                Message = ex.Message,
                ErrorCode = "INSUFFICIENT_STOCK",
                Details = new
                {
                    ProductId = ex.ProductId,
                    RequestedQuantity = ex.RequestedQuantity,
                    AvailableStock = ex.AvailableStock
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar al carrito");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error al agregar el producto al carrito",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Actualizar la cantidad de un producto en el carrito
    /// </summary>
    /// <param name="productId">ID del producto</param>
    /// <param name="dto">Nueva cantidad</param>
    /// <returns>Item actualizado</returns>
    /// <response code="200">Cantidad actualizada exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="401">Usuario no autenticado</response>
    /// <response code="404">Producto no encontrado en el carrito</response>
    /// <response code="422">Stock insuficiente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPut("items/{productId}")]
    [ProducesResponseType(typeof(CartItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CartItemDto>> UpdateCartItem(
        int productId,
        [FromBody] UpdateCartItemDto dto)
    {
        try
        {
            var userId = GetUserIdFromRequest();

            if (userId == 0)
            {
                return Unauthorized(new ErrorResponse
                {
                    Message = "Usuario no autenticado",
                    ErrorCode = "UNAUTHORIZED"
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Datos inválidos",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            _logger.LogInformation(
                "PUT /api/cart/items/{ProductId} - UserId={UserId}, NewQuantity={Quantity}",
                productId, userId, dto.Quantity);

            var cartItem = await _cartService.UpdateCartItemAsync(userId, productId, dto);

            return Ok(cartItem);
        }
        catch (CartItemNotFoundException ex)
        {
            _logger.LogWarning(
                ex,
                "Item no encontrado en carrito: UserId={UserId}, ProductId={ProductId}",
                ex.UserId, ex.ProductId);
            
            return NotFound(new ErrorResponse
            {
                Message = ex.Message,
                ErrorCode = "CART_ITEM_NOT_FOUND",
                Details = new { UserId = ex.UserId, ProductId = ex.ProductId }
            });
        }
        catch (ProductNotFoundException ex)
        {
            _logger.LogWarning(ex, "Producto no encontrado: ProductId={ProductId}", ex.ProductId);
            return NotFound(new ErrorResponse
            {
                Message = ex.Message,
                ErrorCode = "PRODUCT_NOT_FOUND",
                Details = new { ProductId = ex.ProductId }
            });
        }
        catch (InsufficientStockException ex)
        {
            _logger.LogWarning(
                ex,
                "Stock insuficiente: ProductId={ProductId}, Requested={Requested}, Available={Available}",
                ex.ProductId, ex.RequestedQuantity, ex.AvailableStock);
            
            return UnprocessableEntity(new ErrorResponse
            {
                Message = ex.Message,
                ErrorCode = "INSUFFICIENT_STOCK",
                Details = new
                {
                    ProductId = ex.ProductId,
                    RequestedQuantity = ex.RequestedQuantity,
                    AvailableStock = ex.AvailableStock
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar item del carrito");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error al actualizar el item del carrito",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Eliminar un producto del carrito
    /// </summary>
    /// <param name="productId">ID del producto a eliminar</param>
    /// <returns>204 No Content</returns>
    /// <response code="204">Producto eliminado exitosamente</response>
    /// <response code="401">Usuario no autenticado</response>
    /// <response code="404">Producto no encontrado en el carrito</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpDelete("items/{productId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        try
        {
            var userId = GetUserIdFromRequest();

            if (userId == 0)
            {
                return Unauthorized(new ErrorResponse
                {
                    Message = "Usuario no autenticado",
                    ErrorCode = "UNAUTHORIZED"
                });
            }

            _logger.LogInformation(
                "DELETE /api/cart/items/{ProductId} - UserId={UserId}",
                productId, userId);

            var removed = await _cartService.RemoveFromCartAsync(userId, productId);

            if (!removed)
            {
                return NotFound(new ErrorResponse
                {
                    Message = $"El producto {productId} no fue encontrado en el carrito",
                    ErrorCode = "CART_ITEM_NOT_FOUND"
                });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar del carrito");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error al eliminar el producto del carrito",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Vaciar todo el carrito del usuario
    /// </summary>
    /// <returns>204 No Content</returns>
    /// <response code="204">Carrito vaciado exitosamente</response>
    /// <response code="401">Usuario no autenticado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ClearCart()
    {
        try
        {
            var userId = GetUserIdFromRequest();

            if (userId == 0)
            {
                return Unauthorized(new ErrorResponse
                {
                    Message = "Usuario no autenticado",
                    ErrorCode = "UNAUTHORIZED"
                });
            }

            _logger.LogInformation("DELETE /api/cart - UserId={UserId}", userId);

            await _cartService.ClearCartAsync(userId);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al vaciar carrito");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error al vaciar el carrito",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Finalizar compra (Checkout)
    /// Convierte todos los items del carrito en órdenes y vacía el carrito
    /// </summary>
    /// <returns>Resumen de las órdenes creadas</returns>
    /// <response code="200">Checkout completado exitosamente</response>
    /// <response code="400">Carrito vacío o datos inválidos</response>
    /// <response code="401">Usuario no autenticado</response>
    /// <response code="422">Stock insuficiente para algún producto</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("checkout")]
    [ProducesResponseType(typeof(CheckoutResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CheckoutResponseDto>> Checkout()
    {
        try
        {
            var userId = GetUserIdFromRequest();

            if (userId == 0)
            {
                return Unauthorized(new ErrorResponse
                {
                    Message = "Usuario no autenticado",
                    ErrorCode = "UNAUTHORIZED"
                });
            }

            _logger.LogInformation("POST /api/cart/checkout - UserId={UserId}", userId);

            var result = await _cartService.CheckoutAsync(userId);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de validación en checkout - UserId={UserId}", GetUserIdFromRequest());
            return BadRequest(new ErrorResponse
            {
                Message = ex.Message,
                ErrorCode = "INVALID_OPERATION"
            });
        }
        catch (InsufficientStockException ex)
        {
            _logger.LogWarning(ex, "Stock insuficiente durante checkout");
            return UnprocessableEntity(new ErrorResponse
            {
                Message = $"Stock insuficiente para el producto ID {ex.ProductId}. Disponible: {ex.AvailableStock}, Requerido: {ex.RequestedQuantity}",
                ErrorCode = "INSUFFICIENT_STOCK",
                Details = new
                {
                    ProductId = ex.ProductId,
                    RequestedQuantity = ex.RequestedQuantity,
                    AvailableStock = ex.AvailableStock
                }
            });
        }
        catch (ProductNotFoundException ex)
        {
            _logger.LogWarning(ex, "Producto no encontrado durante checkout");
            return NotFound(new ErrorResponse
            {
                Message = $"El producto con ID {ex.ProductId} no fue encontrado",
                ErrorCode = "PRODUCT_NOT_FOUND",
                Details = new { ProductId = ex.ProductId }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar checkout");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error al procesar la compra. Por favor, intente nuevamente.",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // MÉTODOS AUXILIARES
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Obtiene el UserId del request (header, query o token JWT)
    /// TODO: En producción, usar solo JWT token
    /// </summary>
    private int GetUserIdFromRequest()
    {
        // Intentar obtener desde JWT token (producción)
        var userIdFromToken = User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value;
        if (!string.IsNullOrEmpty(userIdFromToken) && int.TryParse(userIdFromToken, out var tokenUserId))
        {
            return tokenUserId;
        }

        // Fallback: obtener desde header X-User-Id (desarrollo/testing)
        if (Request.Headers.TryGetValue("X-User-Id", out var headerUserId))
        {
            if (int.TryParse(headerUserId.ToString(), out var userId))
            {
                return userId;
            }
        }

        // Fallback: obtener desde query param userId (desarrollo/testing)
        if (Request.Query.TryGetValue("userId", out var queryUserId))
        {
            if (int.TryParse(queryUserId.ToString(), out var userId))
            {
                return userId;
            }
        }

        return 0; // No autenticado
    }
}

