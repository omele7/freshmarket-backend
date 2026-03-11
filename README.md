# FreshMarket Backend - Microservicios

Sistema de microservicios para la plataforma de e-commerce FreshMarket, desarrollado con .NET 8.0, Clean Architecture y patrones modernos.

---

## Inicio Rápido

### Método Rápido (1 comando)

```powershell
.\iniciar-todos-los-servicios.ps1
```

---

## Microservicios

### UserService - Puerto 5001
**Propósito:** Autenticación y gestión de usuarios  
**Swagger:** https://localhost:5001/swagger  
**Health:** https://localhost:5001/health

**Características:**
- Registro de usuarios
- Login con JWT
- Roles: Customer, Seller, Admin
- Token expira en 24 horas

### ProductService - Puerto 5003
**Propósito:** Catálogo de productos  
**Swagger:** https://localhost:5003/swagger  
**Health:** https://localhost:5003/health

**Características:**
- CRUD de productos
- Filtrado por categoría
- Gestión de stock
- Clean Architecture

### OrderService - Puerto 5002
**Propósito:** Gestión de órdenes y carritos  
**Swagger:** https://localhost:5002/swagger  
**Health:** https://localhost:5002/health

**Características:**
- Carrito de compras
- Checkout con validación
- Historial de órdenes
- Comunicación con ProductService
- Resiliencia con Polly

---

## Flujo Completo de Uso

### 1. Registrar Usuario
```bash
POST https://localhost:5001/api/auth/register
{
  "name": "Juan Pérez",
  "email": "juan@test.com",
  "password": "test123",
  "role": "Customer"
}
```

### 2. Crear Productos
```bash
POST https://localhost:5003/api/products
{
  "name": "Manzana Roja",
  "price": 2.50,
  "stock": 100,
  "category": "Frutas"
}
```

### 3. Agregar al Carrito
```bash
POST https://localhost:5002/api/cart/items
Headers: Authorization: Bearer {TOKEN}
{
  "productId": 1,
  "quantity": 2
}
```

### 4. Procesar Checkout
```bash
POST https://localhost:5002/api/cart/checkout
Headers: Authorization: Bearer {TOKEN}
```

---

## Arquitectura

```
FresMarketBackend/
│
├── FresMarket.UserService/       (Puerto 5001)
│   ├── Controllers/                 Endpoints API
│   ├── Data/                        DbContext
│   ├── Models/                      Entidades
│   └── Services/                    Lógica de negocio
│
├── FreshMarket.ProductService/   (Puerto 5003)
│   ├── Application/                 DTOs, Interfaces, Services
│   ├── Domain/                      Entidades de negocio
│   ├── Infrastructure/              Repositorios, DbContext
│   └── Controllers/                 API REST
│
├── FreshMarket.OrderService/     (Puerto 5002)
│   ├── Application/                 CQRS, DTOs
│   ├── Domain/                      Entidades, Enums
│   ├── Infrastructure/              Repositorios, Servicios externos
│   └── Controllers/                 API REST
│
├── INICIO_RAPIDO_COMPLETO.md     Guía completa
├── COMO_EJECUTAR.md              Guía de ejecución
└── iniciar-todos-los-servicios.ps1
```

---

## Tecnologías

- **.NET 8.0** - Framework principal
- **Entity Framework Core** - ORM
- **SQL Server** - Base de datos
- **JWT** - Autenticación
- **Swagger/OpenAPI** - Documentación
- **Polly** - Resiliencia y reintentos
- **Clean Architecture** - Patrón arquitectónico
- **CQRS** - Separación de comandos y consultas
- **Repository Pattern** - Acceso a datos

---

## Puertos

| Servicio | HTTPS | HTTP | Base de Datos |
|----------|-------|------|---------------|
| UserService | 5001 | - | FreshMarketDB |
| ProductService | 5003 | 5004 | FreshMarketDB |
| OrderService | 5002 | 5007 | FreshMarketDB |

---

## Checklist de Verificación

Antes de empezar, asegúrate de tener:

- [ ] .NET 8.0 SDK instalado
- [ ] SQL Server ejecutándose
- [ ] PowerShell disponible
- [ ] Puertos 5001, 5002, 5003 disponibles

---

## Documentación

### Guías de Inicio
- **[COMO_EJECUTAR.md](COMO_EJECUTAR.md)** - Cómo ejecutar los servicios
- **[INICIO_RAPIDO_COMPLETO.md](FresMarket.UserService/INICIO_RAPIDO_COMPLETO.md)** - Guía completa de todos los servicios

### Por Microservicio
- **[UserService/INICIO_RAPIDO.md](FresMarket.UserService/INICIO_RAPIDO.md)**
- **[ProductService/INICIO_RAPIDO.md](FreshMarket.ProductService/INICIO_RAPIDO.md)**
- **[OrderService/INICIO_RAPIDO.md](FreshMarket.OrderService/INICIO_RAPIDO.md)**

---

## Solución de Problemas

### Puerto en uso
```powershell
netstat -ano | findstr :5001
```

### Base de datos no existe
```powershell
cd [NombreServicio]
dotnet ef database update
```

### Error al ejecutar script
```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
.\iniciar-todos-los-servicios.ps1
```

### Logs detallados
```powershell
dotnet run --verbosity detailed
```

---

## Testing

### Health Checks
```powershell
curl https://localhost:5001/health
curl https://localhost:5003/health
curl https://localhost:5002/health
```

### Swagger UI
- https://localhost:5001/swagger
- https://localhost:5003/swagger
- https://localhost:5002/swagger

---

## Orden de Inicio Recomendado

1. **UserService** (independiente)
2. **ProductService** (independiente)
3. **OrderService** (requiere ProductService)

> El script `iniciar-todos-los-servicios.ps1` lo hace automáticamente.

---

## Comandos Útiles

### Compilar todo
```powershell
dotnet build FresMarketBackend.sln
```

### Limpiar y reconstruir
```powershell
dotnet clean
dotnet build
```

### Restaurar paquetes
```powershell
dotnet restore
```

### Crear migración
```powershell
dotnet ef migrations add NombreMigracion
```

### Aplicar migraciones
```powershell
dotnet ef database update
```

---

## Despliegue

### Local
```powershell
.\iniciar-todos-los-servicios.ps1
```

### Producción
- Configurar connection strings en `appsettings.Production.json`
- Configurar variables de entorno para JWT Secret
- Habilitar HTTPS con certificados válidos
- Configurar CORS para dominios de producción

---

## Contribución

Este proyecto sigue:
- Clean Architecture
- SOLID Principles
- Repository Pattern
- CQRS (en OrderService)
- API REST best practices

---

## Notas

- **CORS:** Configurado para https://localhost:4200 (Angular)
- **JWT:** Token expira en 24 horas
- **Tax Rate:** 18% configurado en OrderService
- **Resiliencia:** Polly configurado con 3 reintentos

---

## Información del Proyecto

**Versión:** MVP  
**Fecha:** 2026-03-02  
**.NET Version:** 8.0  
**Estado:** Funcional

---

## Listo para Desarrollar

Los 3 microservicios están completamente configurados y listos para:
- Desarrollo local
- Pruebas con Swagger
- Integración con Angular Frontend
- Pruebas de flujo completo

---

**Desarrollado con .NET 8.0, Clean Architecture y buenas prácticas**

