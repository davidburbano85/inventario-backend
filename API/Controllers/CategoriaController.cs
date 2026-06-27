using inventarioWebAI.Aplicacion.DTOs.Categoria;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace inventarioWebAI.Api.Controllers;

[ApiController]
[Route("api/categorias")]
[Authorize]
public class CategoriaController : ControllerBase
{
    private readonly ICategoriaServicio _servicio;
    private readonly ILogger<CategoriaController> _logger;

    public CategoriaController(
        ICategoriaServicio servicio,
        ILogger<CategoriaController> logger)
    {
        _servicio = servicio;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CrearCategoriaAsync([FromBody] CrearCategoriaDTO dto)
    {
        try
        {
            var id = await _servicio.CrearCategoriaAsync(dto.Nombre);

            return Created($"/api/categorias/{id}", new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CategoriaController] Error al crear categoría");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> ObtenerCategoriasActivasAsync()
    {
        try
        {
            var data = await _servicio.ObtenerCategoriasActivasPorEmpresaAsync();

            if (data == null || !data.Any())
                return NotFound("No hay categorías activas");

            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CategoriaController] Error al obtener categorías");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoriaDTO>> ObtenerCategoriaPorIdAsync(Guid id)
    {
        try
        {
            var data = await _servicio.ObtenerCategoriaPorIdAsync(id);

            if (data == null)
                return NotFound("Categoría no encontrada");

            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CategoriaController] Error al obtener categoría por id");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarCategoriaAsync(Guid id, [FromBody] CrearCategoriaDTO dto)
    {
        try
        {
            var ok = await _servicio.ActualizarCategoriaAsync(id, dto.Nombre);

            if (!ok)
                return NotFound("Categoría no encontrada");

            return Ok("Categoría actualizada correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CategoriaController] Error al actualizar categoría");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarCategoriaAsync(Guid id)
    {
        try
        {
            var ok = await _servicio.EliminarCategoriaAsync(id);

            if (!ok)
                return NotFound("Categoría no encontrada");

            return Ok("Categoría eliminada correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CategoriaController] Error al eliminar categoría");
            return StatusCode(500, ex.Message);
        }
    }
}