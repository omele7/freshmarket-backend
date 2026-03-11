using FreshMarket.ProductService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FreshMarket.ProductService.Infrastructure.Data;

/// <summary>
/// Contexto de base de datos para el servicio de productos
/// Implementa DbContext siguiendo los principios de Clean Architecture
/// </summary>
public class ProductDbContext : DbContext
{
    /// <summary>
    /// Constructor que recibe las opciones de configuración del contexto
    /// </summary>
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// DbSet de productos para acceso a la tabla Products
    /// </summary>
    public DbSet<Product> Products { get; set; }

    /// <summary>
    /// Configuración del modelo utilizando Fluent API
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureProductEntity(modelBuilder);
        SeedData(modelBuilder);
    }

    /// <summary>
    /// Configuración de la entidad Product usando Fluent API
    /// </summary>
    private void ConfigureProductEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            // Configuración de la tabla
            entity.ToTable("Products");

            // Clave primaria
            entity.HasKey(e => e.Id);

            // Id - Autoincremental
            entity.Property(e => e.Id)
                .UseIdentityColumn();

            // Name - Requerido, máximo 150 caracteres
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(150);

            // Description - Requerido
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(1000);

            // Price - Requerido, precisión (18,2)
            entity.Property(e => e.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            // Stock - Requerido
            entity.Property(e => e.Stock)
                .IsRequired();

            // Category - Requerido
            entity.Property(e => e.Category)
                .IsRequired()
                .HasMaxLength(100);

            // ImageUrl - Opcional
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500);

            // IsAvailable - Requerido, valor por defecto true
            entity.Property(e => e.IsAvailable)
                .IsRequired()
                .HasDefaultValue(true);

            // CreatedAt - Requerido, valor por defecto GETUTCDATE()
            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // UpdatedAt - Opcional
            entity.Property(e => e.UpdatedAt);

            // Índices para optimización de consultas
            entity.HasIndex(e => e.Category)
                .HasDatabaseName("IX_Products_Category");

            entity.HasIndex(e => e.IsAvailable)
                .HasDatabaseName("IX_Products_IsAvailable");

            entity.HasIndex(e => new { e.Category, e.IsAvailable })
                .HasDatabaseName("IX_Products_Category_IsAvailable");
        });
    }

    /// <summary>
    /// Datos de prueba para inicializar la base de datos
    /// </summary>
    private void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Manzana Roja",
                Description = "Manzanas rojas frescas de la mejor calidad",
                Price = 2.50m,
                Category = "Frutas",
                Stock = 100,
                ImageUrl = "https://example.com/images/manzana-roja.jpg",
                IsAvailable = true,
                CreatedAt = new DateTime(2026, 2, 18, 0, 0, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id = 2,
                Name = "Lechuga Orgánica",
                Description = "Lechuga fresca cultivada orgánicamente",
                Price = 1.80m,
                Category = "Verduras",
                Stock = 50,
                ImageUrl = "https://example.com/images/lechuga.jpg",
                IsAvailable = true,
                CreatedAt = new DateTime(2026, 2, 18, 0, 0, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id = 3,
                Name = "Leche Entera",
                Description = "Leche entera pasteurizada 1L",
                Price = 3.20m,
                Category = "Lácteos",
                Stock = 75,
                ImageUrl = "https://example.com/images/leche.jpg",
                IsAvailable = true,
                CreatedAt = new DateTime(2026, 2, 18, 0, 0, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id = 4,
                Name = "Pan Integral",
                Description = "Pan integral recién horneado",
                Price = 2.00m,
                Category = "Panadería",
                Stock = 30,
                ImageUrl = "https://example.com/images/pan-integral.jpg",
                IsAvailable = true,
                CreatedAt = new DateTime(2026, 2, 18, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}

