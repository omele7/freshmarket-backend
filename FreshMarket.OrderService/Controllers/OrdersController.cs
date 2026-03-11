using FreshMarket.OrderService.Application.DTOs;
using FreshMarket.OrderService.Application.Exceptions;
using FreshMarket.OrderService.Application.Interfaces;
using FreshMarket.OrderService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FreshMarket.OrderService.Controllers;

/// <summary>
/// Controlador para la gestión de órdenes
/// Endpoints REST para crear y consultar pedidos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly CreateOrderCommandHandler _createOrderHandler;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductServiceClient _productServiceClient;
    private readonly ILogger<OrdersController> _logger;

    /// <summary>
    /// Constructor con inyección de dependencias
    /// </summary>
    /// <param name="createOrderHandler">Handler para crear órdenes</param>
    /// <param name="orderRepository">Repositorio de órdenes</param>
    /// <param name="productServiceClient">Cliente para ProductService</param>
    /// <param name="logger">Logger para registro de eventos</param>
    public OrdersController(
        CreateOrderCommandHandler createOrderHandler,
        IOrderRepository orderRepository,
        IProductServiceClient productServiceClient,
        ILogger<OrdersController> logger)
    {
        _createOrderHandler = createOrderHandler ?? throw new ArgumentNullException(nameof(createOrderHandler));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _productServiceClient = productServiceClient ?? throw new ArgumentNullException(nameof(productServiceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Crea una nueva orden
    /// </summary>
    /// <param name="request">Datos de la orden a crear</param>
    /// <returns>Orden creada con código 201</returns>
    /// <response code="201">Orden creada exitosamente</response>
    /// <response code="400">Datos inválidos o producto no existe</response>
    /// <response code="404">Producto no encontrado</response>
    /// <response code="422">Stock insuficiente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderCommand request)
    {
        try
        {
            _logger.LogInformation(
                "POST /api/orders - Recibida solicitud de creación de orden: UserId={UserId}, ProductId={ProductId}, Quantity={Quantity}",
                request.UserId,
                request.ProductId,
                request.Quantity);

            // Validar modelo
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido en CreateOrder: {Errors}", ModelState);
                return BadRequest(new ErrorResponse
                {
                    Message = "Datos de la orden inválidos",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            // Procesar el comando
            var result = await _createOrderHandler.HandleAsync(request);

            _logger.LogInformation(
                "Orden creada exitosamente: OrderId={OrderId}, TotalPrice={TotalPrice}",
                result.Id,
                result.TotalPrice);

            // Retornar 201 Created con el recurso creado
            return CreatedAtAction(
                nameof(GetOrderById),
                new { id = result.Id },
                result);
        }
        catch (ProductNotFoundException ex)
        {
            _logger.LogWarning(
                ex,
                "Producto no encontrado al crear orden: ProductId={ProductId}",
                ex.ProductId);

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
                "Stock insuficiente al crear orden: ProductId={ProductId}, Requested={Requested}, Available={Available}",
                ex.ProductId,
                ex.RequestedQuantity,
                ex.AvailableStock);

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
        catch (ArgumentException ex)
        {
            _logger.LogWarning(
                ex,
                "Argumentos inválidos en CreateOrder: {Message}",
                ex.Message);

            return BadRequest(new ErrorResponse
            {
                Message = ex.Message,
                ErrorCode = "INVALID_ARGUMENT",
                Details = new { ParameterName = ex.ParamName }
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                ex,
                "Operación inválida en CreateOrder: {Message}",
                ex.Message);

            return BadRequest(new ErrorResponse
            {
                Message = ex.Message,
                ErrorCode = "INVALID_OPERATION"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error inesperado al crear orden: UserId={UserId}, ProductId={ProductId}",
                request.UserId,
                request.ProductId);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponse
                {
                    Message = "Error interno del servidor al procesar la orden",
                    ErrorCode = "INTERNAL_SERVER_ERROR"
                });
        }
    }

    /// <summary>
    /// Obtiene una orden por su ID
    /// </summary>
    /// <param name="id">ID de la orden</param>
    /// <returns>Datos de la orden</returns>
    /// <response code="200">Orden encontrada</response>
    /// <response code="404">Orden no encontrada</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponseDto>> GetOrderById(int id)
    {
        _logger.LogInformation("GET /api/orders/{Id} - Obteniendo orden", id);

        // TODO: Implementar GetOrderByIdQuery
        // Por ahora retornamos NotImplemented
        return StatusCode(
            StatusCodes.Status501NotImplemented,
            new ErrorResponse
            {
                Message = "Endpoint no implementado aún",
                ErrorCode = "NOT_IMPLEMENTED"
            });
    }

    /// <summary>
    /// Obtiene todas las órdenes del usuario autenticado agrupadas por fecha
    /// </summary>
    /// <returns>Lista de órdenes agrupadas del usuario</returns>
    /// <response code="200">Órdenes encontradas</response>
    /// <response code="401">Usuario no autenticado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("user")]
    [ProducesResponseType(typeof(IEnumerable<OrderGroupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<OrderGroupDto>>> GetUserOrders()
    {
        try
        {
            var userId = GetUserIdFromRequest();

            if (userId == 0)
            {
                return Unauthorized(new ErrorResponse
                {
                    Message = "Usuario no autenticado. Incluir header 'X-User-Id' o query param 'userId'",
                    ErrorCode = "UNAUTHORIZED"
                });
            }

            _logger.LogInformation("GET /api/orders/user - UserId={UserId}", userId);

            var orders = await _orderRepository.GetByUserIdAsync(userId);

            var groupedOrders = orders
                .GroupBy(o => o.OrderNumber)
                .Select(g => new
                {
                    OrderNumber = g.Key,
                    Orders = g.OrderBy(o => o.CreatedAt).ToList()
                })
                .OrderByDescending(x => x.OrderNumber)
                .ToList();

            var orderGroupDtos = new List<OrderGroupDto>();

            foreach (var group in groupedOrders)
            {
                var items = new List<OrderItemDto>();
                decimal subtotal = 0;

                foreach (var order in group.Orders)
                {
                    try
                    {
                        // Obtener información del producto
                        var product = await _productServiceClient.GetProductByIdAsync(order.ProductId);

                        items.Add(new OrderItemDto
                        {
                            ProductId = order.ProductId,
                            ProductName = product?.Name ?? "Producto #" + order.ProductId.ToString(),
                            Quantity = order.Quantity,
                            UnitPrice = order.UnitPrice,
                            Subtotal = order.TotalPrice
                        });

                        subtotal += order.TotalPrice;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex,
                            "Error al obtener producto para orden: OrderId={OrderId}, ProductId={ProductId}",
                            order.Id, order.ProductId);

                        // Agregar item sin nombre de producto
                        items.Add(new OrderItemDto
                        {
                            ProductId = order.ProductId,
                            ProductName = "Producto #" + order.ProductId.ToString(),
                            Quantity = order.Quantity,
                            UnitPrice = order.UnitPrice,
                            Subtotal = order.TotalPrice
                        });

                        subtotal += order.TotalPrice;
                    }
                }

                var tax = subtotal * 0.18m;
                var total = subtotal + tax;

                orderGroupDtos.Add(new OrderGroupDto
                {
                    Id = group.Orders.First().Id,
                    OrderNumber = group.Orders.First().OrderNumber,
                    Items = items,
                    TotalItems = items.Sum(i => i.Quantity),
                    Subtotal = subtotal,
                    Tax = tax,
                    Total = total,
                    CreatedAt = group.Orders.First().CreatedAt
                });
            }

            _logger.LogInformation(
                "Órdenes agrupadas obtenidas: UserId={UserId}, OrderGroups={Count}",
                userId, orderGroupDtos.Count);

            return Ok(orderGroupDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener órdenes del usuario");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error al obtener las órdenes",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }

    [HttpDelete("order/{orderNumber}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteOrderByNumber(int orderNumber)
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
                "DELETE /api/orders/order/{OrderNumber} - UserId={UserId}",
                orderNumber, userId);

            var deleted = await _orderRepository.DeleteByOrderNumberAsync(userId, orderNumber);

            if (!deleted)
            {
                return NotFound(new ErrorResponse
                {
                    Message = $"Pedido #{orderNumber} no encontrado",
                    ErrorCode = "ORDER_NOT_FOUND"
                });
            }

            await _orderRepository.SaveChangesAsync();
            
            // Reiniciar el contador de ID si la tabla quedó vacía
            await _orderRepository.ResetIdentityIfEmptyAsync();

            _logger.LogInformation(
                "Pedido eliminado: OrderNumber={OrderNumber}, UserId={UserId}",
                orderNumber, userId);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar pedido");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error al eliminar el pedido",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }

    [HttpDelete("user/all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> DeleteAllUserOrders()
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

            _logger.LogInformation("DELETE /api/orders/user/all - UserId={UserId}", userId);

            var deletedCount = await _orderRepository.DeleteAllByUserIdAsync(userId);
            await _orderRepository.SaveChangesAsync();
            
            // Reiniciar el contador de ID si la tabla quedó vacía
            await _orderRepository.ResetIdentityIfEmptyAsync();

            _logger.LogInformation(
                "Pedidos eliminados: Count={Count}, UserId={UserId}",
                deletedCount, userId);

            return Ok(new
            {
                Message = $"Se eliminaron {deletedCount} pedido(s) exitosamente",
                DeletedCount = deletedCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar todos los pedidos");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error al eliminar los pedidos",
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

/// <summary>
/// Modelo de respuesta para errores
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Mensaje de error principal
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Código de error para identificación
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Lista de errores de validación
    /// </summary>
    public List<string>? Errors { get; set; }

    /// <summary>
    /// Detalles adicionales del error
    /// </summary>
    public object? Details { get; set; }

    /// <summary>
    /// Timestamp del error
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

