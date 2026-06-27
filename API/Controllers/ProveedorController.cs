using inventarioWebAI.Aplicacion.DTOs.Proveedor;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inventarioWebAI.Api.Controllers;

[ApiController]
[Route("api/proveedores")]
[Authorize]
public class ProveedorController : ControllerBase
{
    private readonly IProveedorServicio _servicio;

    public ProveedorController(IProveedorServicio servicio)
    {
        _servicio = servicio;
    }

    [HttpPost]
    public async Task<IActionResult> CrearAsync([FromBody] CrearProveedorDTO dto)
    {
        try
        {
            var id = await _servicio.CrearAsync(dto.Nombre, dto.Contacto);
            return Created($"/api/proveedores/{id}", new { id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerPorEmpresaAsync()
    {
        try
        {
            var data = await _servicio.ObtenerPorEmpresaAsync();

            if (data == null || !data.Any())
                return NotFound("No hay proveedores.");

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
            var data = await _servicio.ObtenerPorIdAsync(id);

            if (data == null)
                return NotFound("Proveedor no encontrado.");

            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarAsync(Guid id, [FromBody] CrearProveedorDTO dto)
    {
        try
        {
            var ok = await _servicio.ActualizarAsync(id, dto.Nombre, dto.Contacto);

            if (!ok)
                return NotFound("No se pudo actualizar el proveedor.");

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
            var ok = await _servicio.EliminarAsync(id);

            if (!ok)
                return NotFound("No se pudo eliminar el proveedor.");

            return Ok("Proveedor eliminado correctamente.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}