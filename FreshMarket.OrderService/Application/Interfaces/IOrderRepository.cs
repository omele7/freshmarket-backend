﻿﻿﻿using FreshMarket.OrderService.Domain.Entities;

namespace FreshMarket.OrderService.Application.Interfaces;

/// <summary>
/// Interfaz del repositorio de órdenes
/// Define el contrato para el acceso a datos de órdenes siguiendo el patrón Repository
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Agrega una nueva orden a la base de datos de forma asíncrona
    /// </summary>
    /// <param name="order">Orden a agregar</param>
    /// <returns>La orden agregada con su ID generado</returns>
    Task<Order> AddAsync(Order order);

    /// <summary>
    /// Obtiene una orden por su identificador de forma asíncrona
    /// </summary>
    /// <param name="id">Identificador de la orden (int auto-incremental)</param>
    /// <returns>La orden encontrada o null si no existe</returns>
    Task<Order?> GetByIdAsync(int id);

    /// <summary>
    /// Obtiene todas las órdenes de un usuario específico de forma asíncrona
    /// </summary>
    /// <param name="userId">Identificador del usuario (int, referencia a UserService)</param>
    /// <returns>Colección de órdenes del usuario</returns>
    Task<IEnumerable<Order>> GetByUserIdAsync(int userId);

    /// <summary>
    /// Obtiene todas las órdenes de forma asíncrona
    /// </summary>
    /// <returns>Colección de todas las órdenes</returns>
    Task<IEnumerable<Order>> GetAllAsync();

    /// <summary>
    /// Actualiza una orden existente de forma asíncrona
    /// </summary>
    /// <param name="order">Orden con los datos actualizados</param>
    /// <returns>La orden actualizada</returns>
    Task<Order> UpdateAsync(Order order);

    /// <summary>
    /// Elimina una orden por su identificador de forma asíncrona
    /// </summary>
    /// <param name="id">Identificador de la orden a eliminar (int auto-incremental)</param>
    /// <returns>True si se eliminó exitosamente, False si no se encontró</returns>
    Task<bool> DeleteAsync(int id);

    Task<bool> DeleteByOrderNumberAsync(int userId, int orderNumber);

    Task<int> DeleteAllByUserIdAsync(int userId);

    Task<int> SaveChangesAsync();

    /// <summary>
    /// Reinicia el contador de identidad de la tabla Orders si está vacía
    /// </summary>
    Task ResetIdentityIfEmptyAsync();
}

