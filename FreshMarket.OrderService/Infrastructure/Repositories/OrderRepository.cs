using FreshMarket.OrderService.Application.Interfaces;
using FreshMarket.OrderService.Domain.Entities;
using FreshMarket.OrderService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FreshMarket.OrderService.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de órdenes usando Entity Framework Core
/// Implementa el patrón Repository para abstraer el acceso a datos
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    /// <summary>
    /// Constructor que recibe el contexto de base de datos mediante inyección de dependencias
    /// </summary>
    /// <param name="context">Contexto de base de datos de órdenes</param>
    public OrderRepository(OrderDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Agrega una nueva orden a la base de datos de forma asíncrona
    /// </summary>
    /// <param name="order">Orden a agregar</param>
    /// <returns>La orden agregada con su ID generado</returns>
    public async Task<Order> AddAsync(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        await _context.Orders.AddAsync(order);
        return order;
    }

    /// <summary>
    /// Obtiene una orden por su identificador de forma asíncrona
    /// </summary>
    /// <param name="id">Identificador de la orden (int auto-incremental)</param>
    /// <returns>La orden encontrada o null si no existe</returns>
    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    /// <summary>
    /// Obtiene todas las órdenes de un usuario específico de forma asíncrona
    /// </summary>
    /// <param name="userId">Identificador del usuario (int, referencia a UserService)</param>
    /// <returns>Colección de órdenes del usuario ordenadas por fecha de creación descendente</returns>
    public async Task<IEnumerable<Order>> GetByUserIdAsync(int userId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene todas las órdenes de forma asíncrona
    /// </summary>
    /// <returns>Colección de todas las órdenes ordenadas por fecha de creación descendente</returns>
    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Actualiza una orden existente de forma asíncrona
    /// </summary>
    /// <param name="order">Orden con los datos actualizados</param>
    /// <returns>La orden actualizada</returns>
    public async Task<Order> UpdateAsync(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        _context.Orders.Update(order);
        return await Task.FromResult(order);
    }

    /// <summary>
    /// Elimina una orden por su identificador de forma asíncrona
    /// </summary>
    /// <param name="id">Identificador de la orden a eliminar (int auto-incremental)</param>
    /// <returns>True si se eliminó exitosamente, False si no se encontró</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        
        if (order == null)
            return false;

        _context.Orders.Remove(order);
        return true;
    }

    public async Task<bool> DeleteByOrderNumberAsync(int userId, int orderNumber)
    {
        var orders = await _context.Orders
            .Where(o => o.UserId == userId && o.OrderNumber == orderNumber)
            .ToListAsync();

        if (!orders.Any())
            return false;

        _context.Orders.RemoveRange(orders);
        return true;
    }

    public async Task<int> DeleteAllByUserIdAsync(int userId)
    {
        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .ToListAsync();

        if (!orders.Any())
            return 0;

        _context.Orders.RemoveRange(orders);
        return orders.Count;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Reinicia el contador de identidad de la tabla Orders si está vacía
    /// Esto permite que el siguiente ID sea 1 cuando se eliminen todas las órdenes
    /// </summary>
    public async Task ResetIdentityIfEmptyAsync()
    {
        // Verificar si la tabla está vacía
        var hasOrders = await _context.Orders.AnyAsync();
        
        if (!hasOrders)
        {
            // Reiniciar el contador de identidad a 1 para SQL Server
            await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Orders', RESEED, 0)");
        }
    }
}

