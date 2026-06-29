using inventarioWebAI.Aplicacion.DTOs.Producto;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Aplicacion.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/productos")]
[Authorize]
public class ProductoController : ControllerBase
{
    private readonly IProductoServicio _servicio;

    public ProductoController(IProductoServicio servicio)
    {
        _servicio = servicio;
    }

    [HttpPost]
    public async Task<IActionResult> CrearAsync([FromBody] CrearProductoDTO dto)
    {
        try
        {
            var id = await _servicio.CrearProductoAsync(
                dto.Nombre,
                dto.CodigoSku,
                dto.PrecioVenta,
                dto.PrecioCompra,
                dto.CategoriaId
            );

            return Created($"/api/productos/{id}", new { id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerActivosAsync()
    {
        try
        {
            var data = await _servicio.ObtenerProductosActivosPorEmpresaAsync();

            if (data == null || !data.Any())
                return NotFound("No hay productos.");

            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("categoria/{categoriaId}")]
    public async Task<IActionResult> ObtenerPorCategoriaAsync(Guid categoriaId)
    {
        try
        {
            var data = await _servicio.ObtenerProductosPorCategoriaAsync(categoriaId);

            if (data == null || !data.Any())
                return NotFound("No hay productos en esta categoría.");

            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorIdAsync(Guid id)
    {
        try
        {
            var data = await _servicio.ObtenerProductoPorIdAsync(id);

            if (data == null)
                return NotFound("Producto no encontrado.");

            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarAsync(Guid id, [FromBody] CrearProductoDTO dto)
    {
        try
        {
            var ok = await _servicio.ActualizarProductoAsync(
                id,
                dto.Nombre,
                dto.CodigoSku,
                dto.PrecioVenta,
                dto.PrecioCompra,
                dto.CategoriaId
            );

            if (!ok)
                return NotFound("No se pudo actualizar el producto.");

            return Ok(ok);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarAsync(Guid id)
    {
        try
        {
            var ok = await _servicio.EliminarProductoAsync(id);

            if (!ok)
                return NotFound("No se pudo eliminar el producto.");

            return Ok("Producto eliminado correctamente.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    [HttpGet("{id:guid}/stock")]
    public async Task<IActionResult> ObtenerStock(Guid id, [FromQuery] Guid almacenId)
    {
        var stock = await _servicio.ObtenerStockAsync(id, almacenId);

        return Ok(new
        {
            ProductoId = id,
            AlmacenId = almacenId,
            Stock = stock
        });
    }
}