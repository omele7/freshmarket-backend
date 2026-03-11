using FreshMarket.OrderService.Application.DTOs;
using FreshMarket.OrderService.Application.Exceptions;
using FreshMarket.OrderService.Application.Interfaces;
using FreshMarket.OrderService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FreshMarket.OrderService.Application.Services;

/// <summary>
/// Servicio de aplicación para manejar la creación de órdenes
/// Implementa la lógica de negocio y orquesta la comunicación entre servicios
/// </summary>
public class CreateOrderCommandHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductServiceClient _productServiceClient;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    /// <summary>
    /// Constructor con inyección de dependencias
    /// </summary>
    /// <param name="orderRepository">Repositorio de órdenes</param>
    /// <param name="productServiceClient">Cliente para comunicación con ProductService</param>
    /// <param name="logger">Logger para registro de eventos</param>
    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductServiceClient productServiceClient,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _productServiceClient = productServiceClient ?? throw new ArgumentNullException(nameof(productServiceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Maneja el comando de creación de orden
    /// </summary>
    /// <param name="command">Comando con los datos de la orden a crear</param>
    /// <returns>DTO de respuesta con los datos de la orden creada</returns>
    /// <exception cref="ArgumentNullException">Si el comando es null</exception>
    /// <exception cref="ArgumentException">Si los datos del comando son inválidos</exception>
    /// <exception cref="ProductNotFoundException">Si el producto no existe</exception>
    /// <exception cref="InsufficientStockException">Si no hay stock suficiente</exception>
    public async Task<OrderResponseDto> HandleAsync(CreateOrderCommand command)
    {
        // ═══════════════════════════════════════════════════════════════
        // PASO 1: VALIDAR COMANDO
        // ═══════════════════════════════════════════════════════════════

        if (command == null)
            throw new ArgumentNullException(nameof(command), "El comando no puede ser null");

        _logger.LogInformation(
            "Procesando comando CreateOrder: UserId={UserId}, ProductId={ProductId}, Quantity={Quantity}",
            command.UserId,
            command.ProductId,
            command.Quantity);

        // Validar cantidad
        if (command.Quantity <= 0)
        {
            _logger.LogWarning(
                "Cantidad inválida en CreateOrder: {Quantity}",
                command.Quantity);
            throw new ArgumentException(
                "La cantidad debe ser mayor a cero",
                nameof(command.Quantity));
        }

        // Validar UserId
        if (command.UserId <= 0)
        {
            _logger.LogWarning("UserId inválido en CreateOrder: {UserId}", command.UserId);
            throw new ArgumentException(
                "El UserId debe ser mayor a cero",
                nameof(command.UserId));
        }

        // Validar ProductId
        if (command.ProductId <= 0)
        {
            _logger.LogWarning(
                "ProductId inválido en CreateOrder: {ProductId}",
                command.ProductId);
            throw new ArgumentException(
                "El ProductId debe ser mayor a cero",
                nameof(command.ProductId));
        }

        // ═══════════════════════════════════════════════════════════════
        // PASO 2: VALIDAR QUE EL PRODUCTO EXISTE (ProductService)
        // ═══════════════════════════════════════════════════════════════

        _logger.LogInformation(
            "Consultando ProductService para validar producto ID: {ProductId}",
            command.ProductId);

        var product = await _productServiceClient.GetProductByIdAsync(command.ProductId);

        if (product == null)
        {
            _logger.LogWarning(
                "Producto no encontrado en ProductService: ProductId={ProductId}",
                command.ProductId);
            throw new ProductNotFoundException(command.ProductId);
        }

        _logger.LogInformation(
            "Producto encontrado: ID={ProductId}, Nombre={ProductName}, Precio={Price}, Stock={Stock}",
            product.Id,
            product.Name,
            product.Price,
            product.Stock);

        // ═══════════════════════════════════════════════════════════════
        // PASO 3: VALIDAR STOCK DISPONIBLE
        // ═══════════════════════════════════════════════════════════════

        if (!product.IsAvailable)
        {
            _logger.LogWarning(
                "Producto no disponible para venta: ProductId={ProductId}, Name={ProductName}",
                product.Id,
                product.Name);
            throw new InvalidOperationException(
                $"El producto '{product.Name}' no está disponible para la venta");
        }

        if (product.Stock < command.Quantity)
        {
            _logger.LogWarning(
                "Stock insuficiente: ProductId={ProductId}, Solicitado={Requested}, Disponible={Available}",
                product.Id,
                command.Quantity,
                product.Stock);
            throw new InsufficientStockException(
                product.Id,
                command.Quantity,
                product.Stock);
        }

        // ═══════════════════════════════════════════════════════════════
        // PASO 4: CALCULAR PRECIO TOTAL USANDO EL PRECIO DEL PRODUCTO
        // ═══════════════════════════════════════════════════════════════

        var unitPrice = product.Price;
        var totalPrice = command.Quantity * unitPrice;

        _logger.LogInformation(
            "Precio calculado: Cantidad={Quantity} x Precio={UnitPrice} = Total={TotalPrice}",
            command.Quantity,
            unitPrice,
            totalPrice);

        // ═══════════════════════════════════════════════════════════════
        // PASO 5: CREAR LA ENTIDAD ORDER
        // ═══════════════════════════════════════════════════════════════

        var order = new Order(
            userId: command.UserId,
            productId: command.ProductId,
            quantity: command.Quantity,
            unitPrice: unitPrice
        );

        _logger.LogInformation(
            "Orden creada en memoria: OrderId={OrderId}, TotalPrice={TotalPrice}",
            order.Id,
            order.TotalPrice);

        // ═══════════════════════════════════════════════════════════════
        // PASO 6: GUARDAR LA ORDEN EN LA BASE DE DATOS
        // ═══════════════════════════════════════════════════════════════

        try
        {
            await _orderRepository.AddAsync(order);
            var savedCount = await _orderRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Orden guardada exitosamente en la base de datos: OrderId={OrderId}, Registros afectados={SavedCount}",
                order.Id,
                savedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al guardar la orden en la base de datos: OrderId={OrderId}",
                order.Id);
            throw;
        }

        // ═══════════════════════════════════════════════════════════════
        // PASO 7: RETORNAR DTO DE RESPUESTA
        // ═══════════════════════════════════════════════════════════════

        var response = new OrderResponseDto
        {
            Id = order.Id,
            UserId = order.UserId,
            ProductId = order.ProductId,
            ProductName = product.Name,
            Quantity = order.Quantity,
            UnitPrice = order.UnitPrice,
            TotalPrice = order.TotalPrice,
            CreatedAt = order.CreatedAt
        };

        _logger.LogInformation(
            "CreateOrderCommand procesado exitosamente: OrderId={OrderId}",
            response.Id);

        return response;
    }
}

