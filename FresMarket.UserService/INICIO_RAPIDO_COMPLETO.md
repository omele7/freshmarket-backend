# 🚀 INICIO RÁPIDO - FreshMarket Backend (Todos los Microservicios)

## 📋 Índice
- [Inicio Rápido General](#-inicio-rápido-general)
- [UserService (Puerto 5001)](#-1-userservice---puerto-5001-https)
- [OrderService (Puerto 5002)](#-2-orderservice---puerto-5002-https)
- [ProductService (Puerto 5003)](#-3-productservice---puerto-5003-https)
- [Verificación Completa](#-verificación-completa)
- [Solución de Problemas](#-solución-de-problemas)

---

## ⚡ Inicio Rápido General

### 🎯 Requisitos Previos
- ✅ .NET 8.0 SDK instalado
- ✅ SQL Server ejecutándose
- ✅ PowerShell (para scripts automáticos)

### 🚀 Opción 1: Iniciar Todos los Servicios (Automático)

Desde la raíz del proyecto (`FresMarketBackend/`), ejecuta:

```powershell
.\iniciar-todos-los-servicios.ps1
```

Este script iniciará automáticamente los 3 microservicios en terminales separadas.

### 🔧 Opción 2: Iniciar Manualmente (Paso a Paso)

Abre **3 terminales PowerShell** separadas y ejecuta en cada una:

#### Terminal 1 - UserService
```powershell
cd FresMarket.UserService
dotnet run
```

#### Terminal 2 - ProductService
```powershell
cd FreshMarket.ProductService
dotnet run
```

#### Terminal 3 - OrderService
```powershell
cd FreshMarket.OrderService
dotnet run
```

---

## 🔐 1. UserService - Puerto 5001 (HTTPS)

### 📍 Información General
- **Puerto HTTPS:** 5001
- **Base de datos:** FreshMarketUsers (LocalDB)
- **Swagger:** https://localhost:5001/swagger/index.html
- **Propósito:** Autenticación y gestión de usuarios

### 🎯 Endpoints Principales

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| POST | /api/auth/register | Registrar usuario | No |
| POST | /api/auth/login | Iniciar sesión | No |
| GET | /api/auth/me | Usuario actual | Sí |
| GET | /api/health | Health check | No |

### 🧪 Prueba Rápida

#### 1. Registrar un usuario
```bash
POST https://localhost:5001/api/auth/register
```

**Body:**
```json
{
  "name": "Juan Pérez",
  "email": "juan@test.com",
  "password": "test123",
  "role": "Customer"
}
```

**Respuesta esperada (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "name": "Juan Pérez",
    "email": "juan@test.com",
    "role": "Customer"
  }
}
```

#### 2. Iniciar sesión
```bash
POST https://localhost:5001/api/auth/login
```

**Body:**
```json
{
  "email": "juan@test.com",
  "password": "test123"
}
```

#### 3. Obtener información del usuario actual
```bash
GET https://localhost:5001/api/auth/me
```

**Headers:**
```
Authorization: Bearer {TOKEN_AQUI}
```

### 🔐 Roles Disponibles
- **Customer** - Cliente (por defecto)
- **Seller** - Vendedor
- **Admin** - Administrador

### ⚙️ Configuración
- **Token expira en:** 24 horas (1440 minutos)
- **CORS:** Configurado para https://localhost:4200

---

## 🛍️ 2. OrderService - Puerto 5002 (HTTPS)

### 📍 Información General
- **Puerto HTTPS:** 5002
- **Puerto HTTP:** 5007
- **Base de datos:** FreshMarketDB (SQL Server)
- **Swagger:** https://localhost:5002/swagger/index.html
- **Propósito:** Gestión de órdenes y carritos de compra

### ⚠️ IMPORTANTE
**OrderService DEBE iniciarse después de ProductService** ya que necesita comunicarse con él para validar productos y stock.

### 🎯 Endpoints Principales

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| POST | /api/cart/items | Agregar item al carrito | Sí |
| GET | /api/cart | Obtener carrito del usuario | Sí |
| PUT | /api/cart/items/{productId} | Actualizar cantidad | Sí |
| DELETE | /api/cart/items/{productId} | Eliminar del carrito | Sí |
| POST | /api/cart/checkout | Procesar checkout | Sí |
| POST | /api/orders | Crear orden | Sí |
| GET | /api/orders/{id} | Obtener orden por ID | Sí |
| GET | /api/orders/user/{userId} | Órdenes del usuario | Sí |
| GET | /health | Health check | No |

### 🧪 Prueba Rápida

#### 1. Agregar producto al carrito
```bash
POST https://localhost:5002/api/cart/items
```

**Headers:**
```
Authorization: Bearer {TOKEN}
X-User-Id: 1
```

**Body:**
```json
{
  "productId": 1,
  "quantity": 2
}
```

#### 2. Ver carrito
```bash
GET https://localhost:5002/api/cart
```

**Headers:**
```
Authorization: Bearer {TOKEN}
X-User-Id: 1
```

**Respuesta esperada:**
```json
{
  "items": [
    {
      "productId": 1,
      "productName": "Manzana Roja",
      "quantity": 2,
      "unitPrice": 2.50,
      "subtotal": 5.00
    }
  ],
  "totalItems": 2,
  "subtotal": 5.00,
  "tax": 0.90,
  "total": 5.90
}
```

#### 3. Procesar checkout
```bash
POST https://localhost:5002/api/cart/checkout
```

**Headers:**
```
Authorization: Bearer {TOKEN}
X-User-Id: 1
```

**Respuesta esperada (200 OK):**
```json
{
  "orderId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "totalAmount": 5.90,
  "status": "Pending",
  "createdAt": "2026-03-02T16:30:00Z"
}
```

### 🏗️ Arquitectura
- **Clean Architecture**
- **CQRS** (Command Query Responsibility Segregation)
- **Repository Pattern**
- **Microservices Communication** con ProductService

### 🔄 Resiliencia con Polly
- **Retry Policy:** 3 reintentos con backoff exponencial
- **Circuit Breaker:** Se abre tras 5 fallos por 30 segundos

---

## 📦 3. ProductService - Puerto 5003 (HTTPS)

### 📍 Información General
- **Puerto HTTPS:** 5003
- **Puerto HTTP:** 5004
- **Base de datos:** FreshMarketUsers_Dev (SQL Server)
- **Swagger:** https://localhost:5003/swagger/index.html
- **Propósito:** Gestión del catálogo de productos

### 🎯 Endpoints Principales

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | /api/products | Listar todos los productos | No |
| GET | /api/products/{id} | Obtener producto por ID | No |
| GET | /api/products/category/{category} | Filtrar por categoría | No |
| POST | /api/products | Crear producto | No |
| PUT | /api/products/{id} | Actualizar producto | No |
| DELETE | /api/products/{id} | Eliminar producto | No |

### 🧪 Prueba Rápida

#### 1. Crear un producto
```bash
POST https://localhost:5003/api/products
```

**Body:**
```json
{
  "name": "Manzana Roja",
  "description": "Manzanas frescas de alta calidad",
  "price": 2.50,
  "stock": 100,
  "category": "Frutas",
  "imageUrl": "https://example.com/manzana.jpg"
}
```

**Respuesta esperada (201 Created):**
```json
{
  "id": 1,
  "name": "Manzana Roja",
  "description": "Manzanas frescas de alta calidad",
  "price": 2.50,
  "stock": 100,
  "category": "Frutas",
  "imageUrl": "https://example.com/manzana.jpg"
}
```

#### 2. Obtener todos los productos
```bash
GET https://localhost:5003/api/products
```

#### 3. Obtener producto por ID
```bash
GET https://localhost:5003/api/products/1
```

#### 4. Filtrar por categoría
```bash
GET https://localhost:5003/api/products/category/Frutas
```

### ⚙️ Configuración
- **CORS:** Configurado para https://localhost:4200
- **Entity Framework Core:** Configurado con SQL Server

---

## ✅ Verificación Completa

### 1️⃣ Verifica que todos los servicios estén corriendo

Deberías ver en las consolas:

**UserService:**
```
Now listening on: https://localhost:5001
```

**ProductService:**
```
Now listening on: https://localhost:5003
Now listening on: http://localhost:5004
```

**OrderService:**
```
Now listening on: https://localhost:5002
Now listening on: http://localhost:5007
```

### 2️⃣ Health Checks

Ejecuta estos comandos en PowerShell o desde el navegador:

```powershell
# UserService
curl https://localhost:5001/health

# ProductService
curl https://localhost:5003/health

# OrderService
curl https://localhost:5002/health
```

Todos deberían responder: `Healthy` ✅

### 3️⃣ Swagger UI

Abre en tu navegador:

- **UserService:** https://localhost:5001/swagger/index.html
- **ProductService:** https://localhost:5003/swagger/index.html
- **OrderService:** https://localhost:5002/swagger/index.html

### 4️⃣ Flujo Completo de Prueba

#### Paso 1: Registrar usuario (UserService)
```bash
POST https://localhost:5001/api/auth/register
```
**Body:**
```json
{
  "name": "Test User",
  "email": "test@example.com",
  "password": "test123",
  "role": "Customer"
}
```
✅ **Guarda el token** de la respuesta

#### Paso 2: Crear productos (ProductService)
```bash
POST https://localhost:5003/api/products
```
**Body:**
```json
{
  "name": "Manzana Roja",
  "description": "Manzanas frescas",
  "price": 2.50,
  "stock": 100,
  "category": "Frutas",
  "imageUrl": "https://example.com/manzana.jpg"
}
```

```json
{
  "name": "Lechuga Orgánica",
  "description": "Lechuga fresca orgánica",
  "price": 1.80,
  "stock": 50,
  "category": "Verduras",
  "imageUrl": "https://example.com/lechuga.jpg"
}
```

#### Paso 3: Agregar al carrito (OrderService)
```bash
POST https://localhost:5002/api/cart/items
```
**Headers:**
```
Authorization: Bearer {TOKEN_DEL_PASO_1}
X-User-Id: 1
```
**Body:**
```json
{
  "productId": 1,
  "quantity": 2
}
```

#### Paso 4: Ver carrito
```bash
GET https://localhost:5002/api/cart
```
**Headers:**
```
Authorization: Bearer {TOKEN_DEL_PASO_1}
X-User-Id: 1
```

#### Paso 5: Procesar checkout
```bash
POST https://localhost:5002/api/cart/checkout
```
**Headers:**
```
Authorization: Bearer {TOKEN_DEL_PASO_1}
X-User-Id: 1
```

✅ **¡Orden creada exitosamente!**

---

## 🛠️ Solución de Problemas

### ❌ Error: Base de datos no existe

**Solución para cada servicio:**

```powershell
# UserService
cd FresMarket.UserService
dotnet ef database update

# ProductService
cd FreshMarket.ProductService
dotnet ef database update

# OrderService
cd FreshMarket.OrderService
dotnet ef database update
```

### ❌ Error: Paquetes no restaurados

```powershell
# Desde la raíz del proyecto
dotnet restore
dotnet build
```

### ❌ Error: Puerto en uso

Cambiar puertos en `Properties/launchSettings.json` de cada servicio.

### ❌ Error: ProductService no está corriendo (OrderService)

**Logs de error:**
```
[ERROR] Error de conexión HTTP al intentar obtener producto
Verifique que ProductService esté ejecutándose en http://localhost:5004
```

**Solución:**
1. Asegúrate de iniciar **ProductService primero**
2. Verifica que esté corriendo en el puerto correcto
3. Reinicia OrderService

### ❌ Error: Stock insuficiente

**Respuesta:**
```json
{
  "message": "Stock insuficiente para el producto ID 1. Solicitado: 1000, Disponible: 50",
  "errorCode": "INSUFFICIENT_STOCK"
}
```

**Solución:** Reduce la cantidad solicitada en el carrito.

### ❌ Error: Token inválido o expirado

**Solución:**
1. Vuelve a iniciar sesión en UserService
2. Usa el nuevo token en las peticiones

### ❌ Error: CORS

Si desde el frontend Angular aparecen errores de CORS, verifica que todos los servicios tengan configurado en `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("https://localhost:4200", "http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

---

## 📊 Resumen de Puertos

| Servicio | HTTPS | HTTP |
|----------|-------|------|
| UserService | 5001 | - |
| OrderService | 5002 | 5007 |
| ProductService | 5003 | 5004 |

---

## 🚀 Scripts Útiles

### Compilar todos los servicios
```powershell
dotnet build FresMarketBackend.sln
```

### Limpiar y reconstruir
```powershell
dotnet clean
dotnet build
```

### Ver logs detallados
```powershell
dotnet run --verbosity detailed
```

### Crear migraciones (en cada servicio)
```powershell
dotnet ef migrations add NombreMigracion
dotnet ef database update
```

---

## 📁 Estructura del Proyecto

```
FresMarketBackend/
├── FresMarket.UserService/          (Puerto 5001)
│   ├── Controllers/
│   ├── Data/
│   ├── Models/
│   └── Services/
├── FreshMarket.ProductService/       (Puerto 5003)
│   ├── Controllers/
│   ├── Application/
│   ├── Domain/
│   └── Infrastructure/
├── FreshMarket.OrderService/         (Puerto 5002)
│   ├── Controllers/
│   ├── Application/
│   ├── Domain/
│   └── Infrastructure/
├── INICIO_RAPIDO_COMPLETO.md        ⭐ ESTE ARCHIVO
└── iniciar-todos-los-servicios.ps1  (Script automático)
```

---

## 🎯 Orden Recomendado de Inicio

1. **UserService** (primero - independiente)
2. **ProductService** (segundo - independiente)
3. **OrderService** (último - depende de ProductService)

---

## ✅ Checklist de Verificación

- [ ] SQL Server ejecutándose
- [ ] UserService ejecutándose en https://localhost:5001
- [ ] ProductService ejecutándose en https://localhost:5003
- [ ] OrderService ejecutándose en https://localhost:5002
- [ ] Todos los Swagger accesibles
- [ ] Todos los Health checks retornan "Healthy"
- [ ] Puedes registrar usuarios
- [ ] Puedes crear productos
- [ ] Puedes agregar al carrito
- [ ] Puedes hacer checkout

---

## 📚 Documentación Individual

Si necesitas más detalles sobre un servicio específico:

- **UserService:** `FresMarket.UserService/INICIO_RAPIDO.md`
- **ProductService:** `FreshMarket.ProductService/INICIO_RAPIDO.md`
- **OrderService:** `FreshMarket.OrderService/INICIO_RAPIDO.md`

---

## Todo Listo!

Los 3 microservicios están completamente funcionales y listos para:
- Desarrollo local
- Pruebas con Swagger
- Integración con Angular Frontend
- Pruebas de flujo completo

---

**Desarrollado con:** Clean Architecture, CQRS, Repository Pattern, Polly Resiliency, JWT Authentication  
**Estado:** Funcional para MVP  
**Fecha:** 2026-03-02

**Feliz desarrollo!**

