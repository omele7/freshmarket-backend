# Script para iniciar todos los microservicios de FreshMarket
# Fecha: 2026-03-02
# Autor: Sistema automatizado

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "INICIANDO MICROSERVICIOS FRESHMARKET" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar que estamos en la carpeta correcta
if (-not (Test-Path "FresMarketBackend.sln")) {
    Write-Host "ERROR: No se encuentra el archivo FresMarketBackend.sln" -ForegroundColor Red
    Write-Host "   Asegúrate de ejecutar este script desde la raíz del proyecto" -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Presiona Enter para salir"
    exit 1
}

Write-Host "Directorio correcto verificado" -ForegroundColor Green
Write-Host ""

# Verificar que .NET está instalado
Write-Host "Verificando instalación de .NET..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host ".NET version $dotnetVersion detectado" -ForegroundColor Green
} catch {
    Write-Host "ERROR: .NET no está instalado o no está en el PATH" -ForegroundColor Red
    Write-Host "   Descarga .NET 8.0 desde: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Presiona Enter para salir"
    exit 1
}
Write-Host ""

# Función para iniciar un servicio en una nueva ventana de PowerShell
function Start-Microservice {
    param (
        [string]$ServiceName,
        [string]$ServicePath,
        [int]$Order
    )
    
    Write-Host "[$Order/3] Iniciando $ServiceName..." -ForegroundColor Cyan
    
    # Verificar que la carpeta existe
    if (-not (Test-Path $ServicePath)) {
        Write-Host "   ERROR: La carpeta $ServicePath no existe" -ForegroundColor Red
        return $false
    }
    
    # Iniciar el servicio en una nueva ventana de PowerShell
    $fullPath = Join-Path $PSScriptRoot $ServicePath
    
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$fullPath'; Write-Host 'Iniciando $ServiceName...' -ForegroundColor Green; Write-Host ''; dotnet run"
    
    Write-Host "   $ServiceName iniciado en nueva ventana" -ForegroundColor Green
    Write-Host ""
    
    # Pequeña pausa para evitar saturar el sistema
    Start-Sleep -Seconds 2
    
    return $true
}

# Mostrar instrucciones
Write-Host "ORDEN DE INICIO DE SERVICIOS:" -ForegroundColor Magenta
Write-Host "   1. UserService    (Puerto 5001 HTTPS)" -ForegroundColor White
Write-Host "   2. ProductService (Puerto 5003 HTTPS)" -ForegroundColor White
Write-Host "   3. OrderService   (Puerto 5002 HTTPS)" -ForegroundColor White
Write-Host ""
Write-Host "Se abrirán 3 ventanas de PowerShell..." -ForegroundColor Yellow
Write-Host ""

Start-Sleep -Seconds 2

# Iniciar servicios en orden
$success = $true

# 1. UserService (independiente)
if (-not (Start-Microservice -ServiceName "UserService" -ServicePath "FresMarket.UserService" -Order 1)) {
    $success = $false
}

# 2. ProductService (independiente)
if (-not (Start-Microservice -ServiceName "ProductService" -ServicePath "FreshMarket.ProductService" -Order 2)) {
    $success = $false
}

# 3. OrderService (depende de ProductService)
if (-not (Start-Microservice -ServiceName "OrderService" -ServicePath "FreshMarket.OrderService" -Order 3)) {
    $success = $false
}

# Resumen final
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "PROCESO DE INICIO COMPLETADO" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if ($success) {
    Write-Host "Los 3 microservicios se están iniciando..." -ForegroundColor Green
    Write-Host ""
    Write-Host "VERIFICACIÓN:" -ForegroundColor Magenta
    Write-Host "   - UserService:    https://localhost:5001/swagger" -ForegroundColor White
    Write-Host "   - ProductService: https://localhost:5003/swagger" -ForegroundColor White
    Write-Host "   - OrderService:   https://localhost:5002/swagger" -ForegroundColor White
    Write-Host ""
    Write-Host "Espera 10-15 segundos para que todos los servicios terminen de iniciar" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "HEALTH CHECKS:" -ForegroundColor Magenta
    Write-Host "   - UserService:    https://localhost:5001/health" -ForegroundColor White
    Write-Host "   - ProductService: https://localhost:5003/health" -ForegroundColor White
    Write-Host "   - OrderService:   https://localhost:5002/health" -ForegroundColor White
    Write-Host ""
    Write-Host "TIP: Para detener los servicios, cierra las 3 ventanas de PowerShell" -ForegroundColor Cyan
    Write-Host "     o presiona Ctrl+C en cada una" -ForegroundColor Cyan
} else {
    Write-Host "Hubo algunos errores al iniciar los servicios" -ForegroundColor Yellow
    Write-Host "   Revisa las ventanas de PowerShell abiertas para ver detalles" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Para más información, consulta: INICIO_RAPIDO_COMPLETO.md" -ForegroundColor Cyan
Write-Host ""

# Mantener esta ventana abierta
Write-Host "Presiona cualquier tecla para cerrar esta ventana..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

