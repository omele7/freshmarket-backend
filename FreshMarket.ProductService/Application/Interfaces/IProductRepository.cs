using FreshMarket.ProductService.Domain.Entities;

namespace FreshMarket.ProductService.Application.Interfaces;

/// <summary>
/// Interfaz del repositorio de productos
/// Define el contrato para el acceso a datos de productos siguiendo el patrón Repository
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Obtiene todos los productos de forma asíncrona
    /// </summary>
    /// <returns>Colección de todos los productos</returns>
    Task<IEnumerable<Product>> GetAllAsync();
    
    /// <summary>
    /// Obtiene un producto por su identificador de forma asíncrona
    /// </summary>
    /// <param name="id">Identificador del producto</param>
    /// <returns>El producto encontrado o null si no existe</returns>
    Task<Product?> GetByIdAsync(int id);
    
    /// <summary>
    /// Obtiene todos los productos de una categoría específica de forma asíncrona
    /// </summary>
    /// <param name="category">Nombre de la categoría</param>
    /// <returns>Colección de productos de la categoría especificada</returns>
    Task<IEnumerable<Product>> GetByCategoryAsync(string category);
    
    /// <summary>
    /// Agrega un nuevo producto de forma asíncrona
    /// </summary>
    /// <param name="product">Producto a agregar</param>
    /// <returns>El producto agregado</returns>
    Task<Product> AddAsync(Product product);
    
    /// <summary>
    /// Actualiza un producto existente de forma asíncrona
    /// </summary>
    /// <param name="product">Producto con los datos actualizados</param>
    /// <returns>El producto actualizado</returns>
    Task<Product> UpdateAsync(Product product);
    
    /// <summary>
    /// Elimina un producto por su identificador de forma asíncrona
    /// </summary>
    /// <param name="id">Identificador del producto a eliminar</param>
    /// <returns>True si se eliminó exitosamente, False si no se encontró</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Guarda todos los cambios pendientes en la base de datos de forma asíncrona
    /// </summary>
    /// <returns>Número de entidades modificadas</returns>
    Task<int> SaveChangesAsync();
}

