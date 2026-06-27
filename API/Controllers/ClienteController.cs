using inventarioWebAI.Aplicacion.DTOs.Cliente;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inventarioWebAI.Api.Controllers;

[ApiController]
[Route("api/clientes")]
[Authorize]
public class ClienteController : ControllerBase
{
    private readonly IClienteServicio _servicio;

    public ClienteController(IClienteServicio servicio)
    {
        _servicio = servicio;
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] ClienteDTO dto)
    {
        try
        {
            var id = await _servicio.CrearAsync(dto.Nombre, dto.Contacto);
            return Created($"/api/clientes/{id}", new { id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerPorEmpresa()
    {
        try
        {
            var data = await _servicio.ObtenerPorEmpresaAsync();

            if (!data.Any())
                return NotFound("No hay clientes");

            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        try
        {
            var data = await _servicio.ObtenerPorIdAsync(id);

            if (data == null)
                return NotFound("Cliente no encontrado");

            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] ClienteDTO dto)
    {
        try
        {
            var ok = await _servicio.ActualizarAsync(id, dto.Nombre, dto.Contacto);
            return Ok(ok);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        try
        {
            var ok = await _servicio.EliminarAsync(id);

            if (!ok)
                return NotFound("Cliente no encontrado");

            return Ok("Cliente eliminado correctamente");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}