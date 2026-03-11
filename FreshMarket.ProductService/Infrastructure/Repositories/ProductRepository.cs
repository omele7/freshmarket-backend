using FreshMarket.ProductService.Application.Interfaces;
using FreshMarket.ProductService.Domain.Entities;
using FreshMarket.ProductService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FreshMarket.ProductService.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de productos usando Entity Framework Core
/// Implementa el patrón Repository para abstraer el acceso a datos
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    /// <summary>
    /// Constructor que recibe el contexto de base de datos mediante inyección de dependencias
    /// </summary>
    /// <param name="context">Contexto de base de datos de productos</param>
    public ProductRepository(ProductDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todos los productos de forma asíncrona
    /// </summary>
    /// <returns>Colección de todos los productos ordenados por nombre</returns>
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene un producto por su identificador de forma asíncrona
    /// </summary>
    /// <param name="id">Identificador del producto</param>
    /// <returns>El producto encontrado o null si no existe</returns>
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Obtiene todos los productos de una categoría específica de forma asíncrona
    /// </summary>
    /// <param name="category">Nombre de la categoría</param>
    /// <returns>Colección de productos de la categoría especificada ordenados por nombre</returns>
    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.Category == category)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Agrega un nuevo producto de forma asíncrona
    /// </summary>
    /// <param name="product">Producto a agregar</param>
    /// <returns>El producto agregado con su ID generado</returns>
    public async Task<Product> AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        return product;
    }

    /// <summary>
    /// Actualiza un producto existente de forma asíncrona
    /// </summary>
    /// <param name="product">Producto con los datos actualizados</param>
    /// <returns>El producto actualizado</returns>
    public async Task<Product> UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        return await Task.FromResult(product);
    }

    /// <summary>
    /// Elimina un producto por su identificador de forma asíncrona
    /// </summary>
    /// <param name="id">Identificador del producto a eliminar</param>
    /// <returns>True si se eliminó exitosamente, False si no se encontró el producto</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        
        if (product == null)
            return false;

        _context.Products.Remove(product);
        return true;
    }

    /// <summary>
    /// Guarda todos los cambios pendientes en la base de datos de forma asíncrona
    /// </summary>
    /// <returns>Número de entidades afectadas</returns>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}

