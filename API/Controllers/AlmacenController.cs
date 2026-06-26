using inventarioWebAI.Aplicacion.DTOs.Almacen;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Aplicacion.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/almacenes")]
[Authorize]
public class AlmacenController : ControllerBase
{
    private readonly IAlmacenServicio _servicio;
    private readonly IEmpresaServicio _empresaServicio;

    public AlmacenController(
        IAlmacenServicio servicio,
        IEmpresaServicio empresaServicio)
    {
        _servicio = servicio;
        _empresaServicio = empresaServicio;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CrearAlmacenAsync([FromBody] CrearAlmacenDTO dto)
    {
        try
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(usuarioIdClaim) || !Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return Unauthorized("Usuario no válido");

            var id = await _servicio.CrearAlmacenAsync(
                usuarioId,
                dto.Nombre,
                dto.Ubicacion
            );

            return Created($"/api/almacenes/{id}", new { id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    
    
    [HttpGet]
    public async Task<IActionResult> ObtenerAlmacenesActivosAsync()
    {
        try
        {
            var data = await _servicio.ObtenerAlmacenesActivosPorEmpresaAsync();

            if (data == null || !data.Any())
            {
                Console.WriteLine("DATA VACÍA");
                return NotFound("No hay almacenes activos");
            }

            Console.WriteLine($"DATA OK -> COUNT: {data.Count()}");

            return Ok(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ALMACEN CONTROLLER ERROR] {ex.Message}");
            return StatusCode(500, ex.Message);
        }
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerAlmacenPorIdAsync(Guid id)
    {
        try
        {
            var data = await _servicio.ObtenerAlmacenPorIdAsync(id);

            if (data == null)
                return NotFound("Almacén no encontrado");

            return Ok(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ALMACEN CONTROLLER GET BY ID ERROR] {ex.Message}");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarAlmacenAsync(Guid id, [FromBody] CrearAlmacenDTO dto)
    {
        try
        {
           

            var resultado = await _servicio.ActualizarAlmacenAsync(
               
                id,
                dto.Nombre,
                dto.Ubicacion
            );

         

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ALMACEN CONTROLLER UPDATE ERROR] {ex.Message}");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarAlmacenAsync(Guid id)
    {
        try
        {
            var eliminado = await _servicio.EliminarAlmacenAsync(id);

            if (!eliminado)
                return NotFound("Almacén no encontrado.");

            return Ok("Almacén eliminado correctamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ALMACEN CONTROLLER DELETE ERROR] {ex.Message}");
            return StatusCode(500, ex.Message);
        }
    }

}