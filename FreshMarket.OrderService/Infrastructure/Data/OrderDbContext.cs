using FreshMarket.OrderService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FreshMarket.OrderService.Infrastructure.Data;

/// <summary>
/// Contexto de base de datos para el servicio de pedidos (Orders)
/// Implementa DbContext siguiendo los principios de Clean Architecture
/// </summary>
public class OrderDbContext : DbContext
{
    /// <summary>
    /// Constructor que recibe las opciones de configuración del contexto
    /// </summary>
    /// <param name="options">Opciones de configuración del DbContext</param>
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// DbSet de pedidos para acceso a la tabla Orders
    /// </summary>
    public DbSet<Order> Orders { get; set; }

    /// <summary>
    /// DbSet de items del carrito para acceso a la tabla CartItems
    /// </summary>
    public DbSet<CartItem> CartItems { get; set; }

    /// <summary>
    /// Configuración del modelo utilizando Fluent API
    /// Aplica configuraciones de entidades, restricciones y relaciones
    /// </summary>
    /// <param name="modelBuilder">Constructor de modelos de Entity Framework</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureOrderEntity(modelBuilder);
        ConfigureCartItemEntity(modelBuilder);
    }

    /// <summary>
    /// Configuración de la entidad Order usando Fluent API
    /// Define esquema de tabla, propiedades, restricciones e índices
    /// </summary>
    /// <param name="modelBuilder">Constructor de modelos</param>
    private void ConfigureOrderEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            // ═══════════════════════════════════════════════════════════════
            // CONFIGURACIÓN DE TABLA
            // ═══════════════════════════════════════════════════════════════
            
            entity.ToTable("Orders");

            // ═══════════════════════════════════════════════════════════════
            // CLAVE PRIMARIA
            // ═══════════════════════════════════════════════════════════════
            
            entity.HasKey(e => e.Id);

            // ═══════════════════════════════════════════════════════════════
            // CONFIGURACIÓN DE PROPIEDADES
            // ═══════════════════════════════════════════════════════════════

            // Id - int auto-incremental (generado por la base de datos)
            entity.Property(e => e.Id)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasComment("Identificador único del pedido (auto-incremental)");


            // UserId - int del usuario que realiza el pedido (referencia a UserService)
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasComment("Identificador del usuario que realiza el pedido (int, referencia a UserService)");

            // ProductId - int del producto pedido (referencia a ProductService)
            entity.Property(e => e.ProductId)
                .IsRequired()
                .HasComment("Identificador del producto en el pedido (int, referencia a ProductService)");

            // Quantity - Cantidad de productos (debe ser > 0)
            entity.Property(e => e.Quantity)
                .IsRequired()
                .HasComment("Cantidad de productos en el pedido");

            // UnitPrice - Precio unitario con precisión decimal (18,2)
            entity.Property(e => e.UnitPrice)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasComment("Precio unitario del producto al momento de la compra");

            // TotalPrice - Precio total calculado con precisión decimal (18,2)
            entity.Property(e => e.TotalPrice)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasComment("Precio total del pedido (Quantity * UnitPrice)");

            // CreatedAt - Fecha de creación con valor por defecto
            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .HasComment("Fecha y hora de creación del pedido (UTC)");

            // ═══════════════════════════════════════════════════════════════
            // ÍNDICES PARA OPTIMIZACIÓN DE CONSULTAS
            // ═══════════════════════════════════════════════════════════════

            // Índice en UserId para búsquedas por usuario
            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_Orders_UserId")
                .HasFilter(null);

            // Índice en ProductId para búsquedas por producto
            entity.HasIndex(e => e.ProductId)
                .HasDatabaseName("IX_Orders_ProductId")
                .HasFilter(null);

            // Índice en CreatedAt para ordenamiento por fecha
            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("IX_Orders_CreatedAt")
                .IsDescending();

            // Índice compuesto para consultas de usuario + fecha
            entity.HasIndex(e => new { e.UserId, e.CreatedAt })
                .HasDatabaseName("IX_Orders_UserId_CreatedAt")
                .IsDescending(false, true); // UserId ASC, CreatedAt DESC

            // Índice compuesto para consultas de producto + fecha
            entity.HasIndex(e => new { e.ProductId, e.CreatedAt })
                .HasDatabaseName("IX_Orders_ProductId_CreatedAt")
                .IsDescending(false, true); // ProductId ASC, CreatedAt DESC


            // ═══════════════════════════════════════════════════════════════
            // RESTRICCIONES DE VALIDACIÓN (Check Constraints)
            // ═══════════════════════════════════════════════════════════════

            // Asegurar que Quantity sea mayor a 0
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Orders_Quantity_Positive",
                "[Quantity] > 0"
            ));

            // Asegurar que UnitPrice sea mayor o igual a 0
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Orders_UnitPrice_NonNegative",
                "[UnitPrice] >= 0"
            ));

            // Asegurar que TotalPrice sea mayor o igual a 0
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Orders_TotalPrice_NonNegative",
                "[TotalPrice] >= 0"
            ));
        });
    }

    /// <summary>
    /// Configuración de la entidad CartItem usando Fluent API
    /// </summary>
    private void ConfigureCartItemEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CartItem>(entity =>
        {
            // ═══════════════════════════════════════════════════════════════
            // CONFIGURACIÓN DE TABLA
            // ═══════════════════════════════════════════════════════════════
            
            entity.ToTable("CartItems");

            // ═══════════════════════════════════════════════════════════════
            // CLAVE PRIMARIA
            // ═══════════════════════════════════════════════════════════════
            
            entity.HasKey(e => e.Id);

            // ═══════════════════════════════════════════════════════════════
            // CONFIGURACIÓN DE PROPIEDADES
            // ═════════════════════════════════════════��═════════════════════

            entity.Property(e => e.Id)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasComment("Identificador único del item en el carrito");

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasComment("ID del usuario (referencia a UserService)");

            entity.Property(e => e.ProductId)
                .IsRequired()
                .HasComment("ID del producto (referencia a ProductService)");

            entity.Property(e => e.Quantity)
                .IsRequired()
                .HasComment("Cantidad de unidades del producto");

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .HasComment("Fecha de creación del item");

            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .HasComment("Fecha de última actualización");

            // ═══════════════════════════════════════════════════════════════
            // ÍNDICES
            // ═══════════════════════════════════════════════════════════════

            // Índice único compuesto: Un usuario no puede tener el mismo producto duplicado
            entity.HasIndex(e => new { e.UserId, e.ProductId })
                .IsUnique()
                .HasDatabaseName("IX_CartItems_UserId_ProductId");

            // Índice en UserId para consultas por usuario
            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_CartItems_UserId");

            // ═══════════════════════════════════════════════════════════════
            // RESTRICCIONES
            // ═══════════════════════════════════════════════════════════════

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_CartItems_Quantity_Positive",
                "[Quantity] > 0"
            ));
        });
    }
}

