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
    public async Task<ActionResult<Guid>> Crear([FromBody] CrearAlmacenDTO dto)
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

            return CreatedAtAction(nameof(Crear), new { id }, id);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    [HttpGet]
    public async Task<IActionResult> Obtener()
    {
        try
        {
           var data = await _servicio.ObtenerAlmacenActivoPorEmpresaAsync();

            return Ok(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ALMACEN CONTROLLER ERROR] {ex.Message}");
            return StatusCode(500, ex.Message);
        }
    }



}