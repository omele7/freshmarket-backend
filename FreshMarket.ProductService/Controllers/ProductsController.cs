using FreshMarket.ProductService.Application.DTOs;
using FreshMarket.ProductService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreshMarket.ProductService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAllProducts()
    {
        try
        {
            _logger.LogInformation("GET /api/products - Obteniendo todos los productos");
            
            var products = await _productService.GetAllProductsAsync();
            
            _logger.LogInformation("Se obtuvieron {Count} productos exitosamente", 
                products.Count());
            
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los productos");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "Error interno al obtener los productos" });
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponseDto>> GetProductById(int id)
    {
        try
        {
            _logger.LogInformation("GET /api/products/{Id} - Obteniendo producto", id);
            
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido: {Id}", id);
                return BadRequest(new { Message = "El ID debe ser un número positivo" });
            }

            var product = await _productService.GetProductByIdAsync(id);
            
            if (product == null)
            {
                _logger.LogWarning("Producto con ID {Id} no encontrado", id);
                return NotFound(new { Message = $"Producto con ID {id} no encontrado" });
            }

            _logger.LogInformation("Producto con ID {Id} obtenido exitosamente", id);
            return Ok(product);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Argumento inválido para ID {Id}", id);
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener producto con ID {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "Error interno al obtener el producto" });
        }
    }

    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(IEnumerable<ProductResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProductsByCategory(string category)
    {
        try
        {
            _logger.LogInformation("GET /api/products/category/{Category} - Obteniendo productos", category);
            
            if (string.IsNullOrWhiteSpace(category))
            {
                _logger.LogWarning("Categoría vacía o nula recibida");
                return BadRequest(new { Message = "La categoría no puede estar vacía" });
            }

            var products = await _productService.GetProductsByCategoryAsync(category);
            
            _logger.LogInformation("Se obtuvieron {Count} productos de la categoría {Category}", 
                products.Count(), category);
            
            return Ok(products);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Categoría inválida: {Category}", category);
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos de categoría {Category}", category);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "Error interno al obtener productos por categoría" });
        }
    }

    [HttpGet("available")]
    [ProducesResponseType(typeof(IEnumerable<ProductResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAvailableProducts()
    {
        try
        {
            _logger.LogInformation("GET /api/products/available - Obteniendo productos disponibles");
            
            var products = await _productService.GetAvailableProductsAsync();
            
            _logger.LogInformation("Se obtuvieron {Count} productos disponibles", products.Count());
            
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos disponibles");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "Error interno al obtener productos disponibles" });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponseDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        try
        {
            _logger.LogInformation("POST /api/products - Creando nuevo producto: {ProductName}", 
                createProductDto?.Name ?? "null");
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido al crear producto. Errores: {Errors}", 
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return BadRequest(ModelState);
            }

            if (createProductDto == null)
            {
                _logger.LogWarning("DTO nulo recibido en CreateProduct");
                return BadRequest(new { Message = "Los datos del producto son requeridos" });
            }

            var product = await _productService.CreateProductAsync(createProductDto);
            
            _logger.LogInformation("Producto creado exitosamente con ID: {ProductId}", product.Id);
            
            return CreatedAtAction(
                nameof(GetProductById), 
                new { id = product.Id }, 
                product);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogWarning(ex, "Argumento nulo al crear producto");
            return BadRequest(new { Message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Error de validación al crear producto");
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear producto");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "Error interno al crear el producto" });
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponseDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
    {
        try
        {
            _logger.LogInformation("PUT /api/products/{Id} - Actualizando producto", id);
            
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido en actualización: {Id}", id);
                return BadRequest(new { Message = "El ID debe ser un número positivo" });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido al actualizar producto {Id}", id);
                return BadRequest(ModelState);
            }

            if (updateProductDto == null)
            {
                _logger.LogWarning("DTO nulo recibido en UpdateProduct para ID {Id}", id);
                return BadRequest(new { Message = "Los datos de actualización son requeridos" });
            }

            var product = await _productService.UpdateProductAsync(id, updateProductDto);
            
            _logger.LogInformation("Producto con ID {Id} actualizado exitosamente", id);
            
            return Ok(product);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Producto con ID {Id} no encontrado para actualizar", id);
            return NotFound(new { Message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Error de validación al actualizar producto {Id}", id);
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar producto con ID {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "Error interno al actualizar el producto" });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            _logger.LogInformation("DELETE /api/products/{Id} - Eliminando producto", id);
            
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido en eliminación: {Id}", id);
                return BadRequest(new { Message = "El ID debe ser un número positivo" });
            }

            await _productService.DeleteProductAsync(id);
            
            _logger.LogInformation("Producto con ID {Id} eliminado exitosamente", id);
            
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Producto con ID {Id} no encontrado para eliminar", id);
            return NotFound(new { Message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Error de validación al eliminar producto {Id}", id);
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar producto con ID {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "Error interno al eliminar el producto" });
        }
    }
}

