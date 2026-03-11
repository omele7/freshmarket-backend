# FreshMarket Backend - Microservicios

Sistema de microservicios para la plataforma de e-commerce FreshMarket, desarrollado con .NET 8.0, Clean Architecture y patrones modernos de desarrollo.

## Tabla de Contenidos

- [Descripción General](#descripción-general)
- [Arquitectura del Sistema](#arquitectura-del-sistema)
- [Microservicios](#microservicios)
- [Tecnologías y Herramientas](#tecnologías-y-herramientas)
- [Requisitos Previos](#requisitos-previos)
- [Instalación y Ejecución](#instalación-y-ejecución)
- [Endpoints Principales](#endpoints-principales)
- [Configuración](#configuración)
- [Base de Datos](#base-de-datos)
- [Seguridad](#seguridad)
- [Testing y Monitoreo](#testing-y-monitoreo)
- [Comandos Útiles](#comandos-útiles)
- [Solución de Problemas](#solución-de-problemas)

## Descripción General

FreshMarket Backend es una arquitectura de microservicios diseñada para soportar una plataforma de comercio electrónico. El sistema está compuesto por tres microservicios independientes que se comunican entre sí para proporcionar funcionalidades completas de autenticación, gestión de productos y procesamiento de órdenes.

### Características Principales

- Arquitectura de microservicios desacoplada
- Autenticación y autorización con JWT
- Clean Architecture con separación de responsabilidades
- Persistencia con Entity Framework Core
- Migraciones automáticas en desarrollo
- Documentación interactiva con Swagger
- Health checks integrados
- Comunicación resiliente entre servicios con Polly
- Manejo global de excepciones
- CORS configurado para integración frontend


## Arquitectura del Sistema

```
FresMarketBackend/
├── FresMarket.UserService/          (Puerto 5001/5000)
│   ├── Controllers/                 API endpoints
│   │   ├── AuthController.cs       Autenticación y registro
│   │   ├── UsersController.cs      Gestión de usuarios
│   │   └── HealthController.cs     Health checks
│   ├── Data/                        Capa de datos
│   │   └── ApplicationDbContext.cs EF Core DbContext
│   ├── Models/                      Entidades de dominio
│   │   ├── User.cs                 Entidad usuario
│   │   ├── Address.cs              Dirección de usuario
│   │   └── DTOs/                   Data Transfer Objects
│   ├── Services/                    Lógica de negocio
│   │   ├── AuthService.cs          Servicio de autenticación
│   │   ├── JwtTokenService.cs      Generación de tokens
│   │   └── UserService.cs          Gestión de usuarios
│   └── Middleware/                  Middleware personalizado
│       └── GlobalExceptionHandler.cs
│
├── FreshMarket.ProductService/      (Puerto 5003/5004)
│   ├── Application/                 Capa de aplicación
│   │   ├── DTOs/                   Data Transfer Objects
│   │   ├── Interfaces/             Contratos de servicios
│   │   └── Services/               Lógica de negocio
│   ├── Domain/                      Capa de dominio
│   │   └── Entities/
│   │       └── Product.cs          Entidad producto
│   ├── Infrastructure/              Capa de infraestructura
│   │   ├── Data/
│   │   │   └── ProductDbContext.cs EF Core DbContext
│   │   └── Repositories/           Implementación de repositorios
│   └── Controllers/
│       └── ProductsController.cs   API REST completa
│
├── FreshMarket.OrderService/        (Puerto 5002/5007)
│   ├── Application/                 Capa de aplicación
│   │   ├── DTOs/                   Data Transfer Objects
│   │   ├── Interfaces/             Contratos de servicios
│   │   ├── Services/               Lógica de negocio
│   │   │   └── CreateOrderCommandHandler.cs
│   │   └── Exceptions/             Excepciones de dominio
│   ├── Domain/                      Capa de dominio
│   │   ├── Entities/
│   │   │   ├── Order.cs            Entidad orden
│   │   │   ├── CartItem.cs         Item de carrito
│   │   │   ├── OrderItem.cs        Item de orden
│   │   │   └── ShippingAddress.cs  Dirección de envío
│   │   └── Enums/                  Enumeraciones de dominio
│   ├── Infrastructure/              Capa de infraestructura
│   │   ├── Data/
│   │   │   └── OrderDbContext.cs   EF Core DbContext
│   │   ├── Repositories/           Implementación de repositorios
│   │   └── Services/
│   │       └── ProductServiceClient.cs HttpClient para ProductService
│   └── Controllers/
│       ├── CartController.cs       Gestión de carrito
│       └── OrdersController.cs     Gestión de órdenes
│
└── iniciar-todos-los-servicios.ps1 Script de inicio automatizado
```

## Microservicios

### 1. UserService (Servicio de Usuarios)

**Puerto:** HTTPS 5001, HTTP 5000  
**Swagger:** https://localhost:5001/swagger  
**Base de Datos:** FreshMarketDB

**Responsabilidades:**
- Registro de nuevos usuarios
- Autenticación con credenciales
- Generación y validación de tokens JWT
- Gestión de perfiles de usuario
- Manejo de direcciones

**Características Técnicas:**
- Hashing de contraseñas con BCrypt.Net
- Tokens JWT con expiración de 1440 minutos (24 horas)
- Middleware de manejo global de excepciones
- Validación de claims personalizados
- CORS configurado para localhost:4200

**Endpoints Principales:**
- `POST /api/auth/register` - Registro de usuarios
- `POST /api/auth/login` - Autenticación
- `GET /api/auth/me` - Obtener usuario actual (requiere autenticación)
- `GET /api/users` - Listar usuarios
- `GET /api/users/{id}` - Obtener usuario por ID

**Tecnologías:**
- ASP.NET Core 8.0
- Entity Framework Core 8.0.11
- Microsoft.AspNetCore.Authentication.JwtBearer 8.0.11
- BCrypt.Net-Next 4.0.3
- System.IdentityModel.Tokens.Jwt 8.2.1

### 2. ProductService (Servicio de Productos)

**Puerto:** HTTPS 5003, HTTP 5004  
**Swagger:** https://localhost:5003/swagger  
**Base de Datos:** FreshMarketDB

**Responsabilidades:**
- CRUD completo de productos
- Filtrado por categoría
- Gestión de inventario y stock
- Consulta de productos disponibles
- Validación de disponibilidad

**Características Técnicas:**
- Clean Architecture con separación de capas
- Repository Pattern
- Retry logic con Entity Framework
- Health checks con verificación de base de datos
- Logging estructurado
- Migraciones automáticas en desarrollo

**Endpoints Principales:**
- `GET /api/products` - Listar todos los productos
- `GET /api/products/{id}` - Obtener producto por ID
- `GET /api/products/category/{category}` - Filtrar por categoría
- `GET /api/products/available` - Productos con stock disponible
- `POST /api/products` - Crear producto
- `PUT /api/products/{id}` - Actualizar producto
- `DELETE /api/products/{id}` - Eliminar producto

**Tecnologías:**
- ASP.NET Core 8.0
- Entity Framework Core 8.0.24
- Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore 8.0.24

### 3. OrderService (Servicio de Órdenes)

**Puerto:** HTTPS 5002, HTTP 5007  
**Swagger:** https://localhost:5002/swagger  
**Base de Datos:** FreshMarketDB

**Responsabilidades:**
- Gestión de carrito de compras persistente
- Creación y procesamiento de órdenes
- Validación de stock con ProductService
- Cálculo de totales y subtotales
- Historial de órdenes por usuario
- Checkout con validaciones de negocio

**Características Técnicas:**
- Comunicación HTTP con ProductService
- Polly para resiliencia y reintentos
- CQRS con Command Handlers
- Excepciones de dominio personalizadas
- Validación de stock en tiempo real
- Carrito persistente en base de datos
- Soporte para autenticación JWT (header X-User-Id para desarrollo)

**Endpoints Principales:**

**Carrito:**
- `GET /api/cart` - Obtener carrito del usuario
- `POST /api/cart/items` - Agregar producto al carrito
- `PUT /api/cart/items/{productId}` - Actualizar cantidad
- `DELETE /api/cart/items/{productId}` - Eliminar del carrito
- `DELETE /api/cart` - Vaciar carrito
- `POST /api/cart/checkout` - Procesar checkout

**Órdenes:**
- `POST /api/orders` - Crear orden
- `GET /api/orders` - Listar órdenes
- `GET /api/orders/{id}` - Obtener orden por ID
- `GET /api/orders/user/{userId}` - Órdenes de un usuario

**Tecnologías:**
- ASP.NET Core 8.0
- Entity Framework Core 8.0.24
- Microsoft.Extensions.Http.Polly 10.0.3
- HttpClient con certificados de desarrollo

## Tecnologías y Herramientas

### Framework y Runtime
- **.NET 8.0** - Framework principal
- **C# 12** - Lenguaje de programación

### Persistencia
- **Entity Framework Core 8.0** - ORM
- **SQL Server** - Base de datos relacional
- **Migraciones EF Core** - Versionado de esquema

### Seguridad
- **JWT (JSON Web Tokens)** - Autenticación stateless
- **BCrypt** - Hashing de contraseñas
- **HTTPS** - Comunicación cifrada
- **CORS** - Control de acceso entre orígenes

### Comunicación
- **ASP.NET Core Web API** - APIs RESTful
- **HttpClient** - Comunicación entre servicios
- **Polly** - Resiliencia y políticas de reintento

### Documentación y Testing
- **Swagger/OpenAPI** - Documentación interactiva
- **Health Checks** - Monitoreo de estado
- **Logging** - Registro estructurado

### Patrones de Diseño
- **Clean Architecture** - Separación de responsabilidades
- **Repository Pattern** - Abstracción de acceso a datos
- **CQRS** - Separación de comandos y consultas
- **Dependency Injection** - Inversión de control

## Requisitos Previos

- **.NET 8.0 SDK** o superior
- **SQL Server** (LocalDB, Express o Full)
- **PowerShell 5.1** o superior
- **Puertos disponibles:** 5000, 5001, 5002, 5003, 5004, 5007
- **Visual Studio 2022** / **Visual Studio Code** / **JetBrains Rider** (opcional)

### Verificar instalación de .NET

```powershell
dotnet --version
```

Debería mostrar `8.0.0` o superior.

### Verificar SQL Server

```powershell
sqlcmd -S localhost -Q "SELECT @@VERSION"
```

## Instalación y Ejecución

### Método 1: Script Automatizado (Recomendado)


```powershell
# Desde la raíz del proyecto
.\iniciar-todos-los-servicios.ps1
```

Este script:
1. Verifica la instalación de .NET
2. Restaura dependencias NuGet
3. Compila la solución
4. Aplica migraciones automáticamente en desarrollo
5. Inicia los 3 servicios en ventanas separadas de PowerShell

### Método 2: Ejecución Manual

#### 1. Restaurar dependencias

```powershell
dotnet restore FresMarketBackend.sln
```

#### 2. Compilar la solución

```powershell
dotnet build FresMarketBackend.sln
```

#### 3. Aplicar migraciones (si es necesario)

```powershell
# UserService
cd FresMarket.UserService
dotnet ef database update

# ProductService
cd ..\FreshMarket.ProductService
dotnet ef database update

# OrderService
cd ..\FreshMarket.OrderService
dotnet ef database update
```

#### 4. Ejecutar servicios (en terminales separadas)

```powershell
# Terminal 1 - UserService
cd FresMarket.UserService
dotnet run

# Terminal 2 - ProductService
cd FreshMarket.ProductService
dotnet run

# Terminal 3 - OrderService
cd FreshMarket.OrderService
dotnet run
```

### Verificación de Servicios

Una vez iniciados, verificar que los servicios estén funcionando:

```powershell
curl https://localhost:5001/swagger  # UserService Swagger
curl https://localhost:5003/swagger  # ProductService Swagger
curl https://localhost:5002/swagger  # OrderService Swagger
```

## Endpoints Principales

### Flujo Completo de Uso

#### 1. Registrar Usuario

```http
POST https://localhost:5001/api/auth/register
Content-Type: application/json

{
  "email": "usuario@freshmarket.com",
  "password": "Password123!",
  "firstName": "Juan",
  "lastName": "Pérez",
  "phone": "987654321"
}
```

**Respuesta:**
```json
{
  "user": {
    "id": 1,
    "email": "usuario@freshmarket.com",
    "firstName": "Juan",
    "lastName": "Pérez"
  },
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### 2. Iniciar Sesión

```http
POST https://localhost:5001/api/auth/login
Content-Type: application/json

{
  "email": "usuario@freshmarket.com",
  "password": "Password123!"
}
```

#### 3. Crear Producto

```http
POST https://localhost:5003/api/products
Content-Type: application/json

{
  "name": "Manzana Fuji",
  "description": "Manzanas frescas importadas",
  "price": 3.50,
  "stock": 100,
  "category": "Frutas",
  "imageUrl": "https://example.com/manzana.jpg"
}
```

#### 4. Agregar al Carrito

```http
POST https://localhost:5002/api/cart/items
Content-Type: application/json
X-User-Id: 1

{
  "productId": 1,
  "quantity": 5
}
```

**Nota:** En producción, el `X-User-Id` se obtiene del token JWT. En desarrollo se puede usar el header para pruebas.

#### 5. Ver Carrito

```http
GET https://localhost:5002/api/cart
X-User-Id: 1
```

**Respuesta:**
```json
{
  "userId": 1,
  "items": [
    {
      "productId": 1,
      "productName": "Manzana Fuji",
      "quantity": 5,
      "unitPrice": 3.50,
      "subtotal": 17.50
    }
  ],
  "totalItems": 1,
  "totalQuantity": 5,
  "subtotal": 17.50,
  "total": 17.50
}
```

#### 6. Procesar Checkout

```http
POST https://localhost:5002/api/cart/checkout
X-User-Id: 1
```

## Configuración

### Connection Strings

Todos los servicios comparten la misma base de datos configurada en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FreshMarketDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

### JWT Settings (UserService)

```json
{
  "JwtSettings": {
    "SecretKey": "FreshM@rket2026$ecur3K3y!Pr0duct10n#JWT&T0k3nG3n3r@t0r",
    "Issuer": "FreshMarket.UserService",
    "Audience": "FreshMarket.Clients",
    "ExpirationMinutes": 1440
  }
}
```

### CORS Configuration

**UserService:**
- Origen permitido: `http://localhost:4200`, `https://localhost:4200`
- Política: `FreshMarketPolicy`

**ProductService y OrderService:**
- Política: `AllowAll` (desarrollo)
- En producción: configurar orígenes específicos

### Puertos Configurados

| Servicio | HTTPS | HTTP | Protocolo Preferido |
|----------|-------|------|---------------------|
| UserService | 5001 | 5000 | HTTPS |
| ProductService | 5003 | 5004 | HTTPS |
| OrderService | 5002 | 5007 | HTTPS |

## Base de Datos

### Esquema

**Base de Datos:** `FreshMarketDB`

**Tablas Principales:**

#### UserService
- `Users` - Usuarios del sistema
- `Addresses` - Direcciones de usuarios

#### ProductService
- `Products` - Catálogo de productos

#### OrderService
- `Orders` - Órdenes procesadas
- `CartItems` - Carrito de compras persistente
- `OrderItems` - Items de órdenes
- `ShippingAddresses` - Direcciones de envío

### Migraciones

Las migraciones se aplican automáticamente en modo desarrollo al iniciar cada servicio.

**Crear nueva migración:**

```powershell
cd [NombreServicio]
dotnet ef migrations add [NombreMigracion]
```

**Aplicar migraciones manualmente:**

```powershell
dotnet ef database update
```

**Revertir última migración:**

```powershell
dotnet ef database update [MigraciónAnterior]
```

**Eliminar última migración:**

```powershell
dotnet ef migrations remove
```

## Seguridad

### Autenticación JWT

- **Algoritmo:** HS256 (HMAC-SHA256)
- **Emisor:** FreshMarket.UserService
- **Audiencia:** FreshMarket.Clients
- **Expiración:** 24 horas
- **Claims incluidos:**
  - `sub` (Subject): User ID
  - `email`: Email del usuario
  - `given_name`: Nombre
  - `family_name`: Apellido
  - `jti`: JWT ID único

### Uso de Tokens

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Hashing de Contraseñas

- **Librería:** BCrypt.Net-Next
- **Work Factor:** 12 (por defecto)
- **Salt:** Generado automáticamente por BCrypt

## Testing y Monitoreo

### Health Checks

```http
GET https://localhost:5001/health  # UserService
GET https://localhost:5003/health  # ProductService
GET https://localhost:5002/health  # OrderService
```

**Respuesta esperada:** `Healthy`

### Swagger UI

Accede a la documentación interactiva:

- **UserService:** https://localhost:5001/swagger
- **ProductService:** https://localhost:5003/swagger
- **OrderService:** https://localhost:5002/swagger

Swagger permite:
- Explorar todos los endpoints
- Probar peticiones directamente
- Ver esquemas de DTOs
- Autenticarse con JWT (en UserService)


## Comandos Útiles

### Compilación

```powershell
# Compilar toda la solución
dotnet build FresMarketBackend.sln

# Compilar en modo Release
dotnet build FresMarketBackend.sln -c Release

# Limpiar binarios
dotnet clean FresMarketBackend.sln
```

### Ejecución

```powershell
# Ejecutar con hot reload
dotnet watch run

# Ejecutar con logs detallados
dotnet run --verbosity detailed

# Ejecutar en modo Release
dotnet run -c Release
```

### Migraciones

```powershell
# Listar migraciones
dotnet ef migrations list

# Ver script SQL de una migración
dotnet ef migrations script

# Actualizar a una migración específica
dotnet ef database update [NombreMigración]

# Eliminar base de datos
dotnet ef database drop
```

### NuGet

```powershell
# Restaurar paquetes
dotnet restore

# Actualizar paquete específico
dotnet add package [NombrePaquete]

# Listar paquetes
dotnet list package
```

## Solución de Problemas

### Error: Puerto en uso

```powershell
# Ver procesos usando el puerto
netstat -ano | findstr :5001

# Matar proceso por PID
taskkill /PID [PID] /F
```

### Error: Base de datos no accesible

1. Verificar que SQL Server esté ejecutándose
2. Verificar connection string en `appsettings.json`
3. Intentar aplicar migraciones manualmente:

```powershell
cd [NombreServicio]
dotnet ef database update
```

### Error: No se puede ejecutar el script

```powershell
# Cambiar política de ejecución temporalmente
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
.\iniciar-todos-los-servicios.ps1
```

### Error: JWT inválido o expirado

- Verificar que el token no haya expirado (24 horas)
- Generar nuevo token haciendo login nuevamente
- Verificar que `JwtSettings` sea consistente entre generación y validación

### Error: Comunicación entre servicios

**OrderService no puede comunicarse con ProductService:**

1. Verificar que ProductService esté ejecutándose en `https://localhost:5003`
2. Revisar logs de OrderService para ver detalles del error
3. Verificar certificado SSL en desarrollo (debería ser aceptado automáticamente)

### Logs y Debugging

```powershell
# Habilitar logs detallados en appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

## Documentación Adicional

- **[INICIO_RAPIDO_COMPLETO.md](FresMarket.UserService/INICIO_RAPIDO_COMPLETO.md)** - Guía detallada de todos los servicios

## Información del Proyecto

**Versión:** 1.0.0  
**Estado:** Producción  
**.NET Version:** 8.0  
**Última Actualización:** 2026-03-11

## Licencia

Este proyecto es de uso interno para FreshMarket.

## Contacto y Soporte

Para reportar problemas o solicitar nuevas funcionalidades, contactar al equipo de desarrollo.

---

Desarrollado con .NET 8.0, siguiendo principios de Clean Architecture y SOLID

