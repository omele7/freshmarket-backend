﻿﻿using FresMarket.UserService.Models;
using Microsoft.EntityFrameworkCore;

namespace FresMarket.UserService.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Address> Addresses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            // Configuración de Primary Key
            entity.HasKey(u => u.Id);

            // Configuración de índice único para Email
            entity.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            // Configuración de propiedades requeridas
            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.PasswordHash)
                .IsRequired();

            entity.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Phone)
                .HasMaxLength(20);

            // Configuración de timestamps automáticos
            entity.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(u => u.UpdatedAt)
                .IsRequired(false);
        });

        modelBuilder.Entity<Address>(entity =>
        {
            // Configuración de Primary Key
            entity.HasKey(a => a.Id);

            // Configuración de índice en UserId
            entity.HasIndex(a => a.UserId)
                .HasDatabaseName("IX_Addresses_UserId");

            // Configuración de relación 1:1 con User
            entity.HasOne(a => a.User)
                .WithOne(u => u.Address)
                .HasForeignKey<Address>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de propiedades requeridas
            entity.Property(a => a.UserId)
                .IsRequired();

            entity.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.State)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.ZipCode)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.IsDefault)
                .IsRequired()
                .HasDefaultValue(true);

            // Configuración de timestamps automáticos
            entity.Property(a => a.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(a => a.UpdatedAt)
                .IsRequired(false);
        });
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var userEntries = ChangeTracker.Entries<User>();
        foreach (var entry in userEntries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        var addressEntries = ChangeTracker.Entries<Address>();
        foreach (var entry in addressEntries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}

