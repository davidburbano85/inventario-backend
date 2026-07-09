using Microsoft.AspNetCore.Mvc;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VentasController : ControllerBase
{
    private readonly IVentasServicio _ventasServicio;

    public VentasController(IVentasServicio ventasServicio)
    {
        _ventasServicio = ventasServicio;
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearVentaRequest request)
    {
        if (request == null || request.Detalles == null || !request.Detalles.Any())
            return BadRequest("Datos inválidos.");

        var ventaId = await _ventasServicio.CrearAsync(
            request.ClienteId,
            request.Detalles.Select(d => new CrearVentaDetalleDTO
            {
                Id= d.Id,
                EmpresaId = d.EmpresaId,
                ProductoId = d.ProductoId,
                Cantidad = d.Cantidad,

            }).ToList()

        );

        return Ok(new { VentaId = ventaId });
    }

    
    
    
    [HttpGet]
    public async Task<IActionResult> ObtenerPorEmpresa()
    {
        var result = await _ventasServicio.ObtenerPorEmpresaAsync();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var result = await _ventasServicio.ObtenerPorIdAsync(id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Anular(Guid id)
    {
        var ok = await _ventasServicio.AnularAsync(id);

        if (!ok)
            return BadRequest("No se pudo anular la venta.");

        return Ok(true);
    }
}

/// <summary>
/// Request DTO mínimo del controller (sin exponer EmpresaId ni UsuarioId)
/// </summary>
public class CrearVentaRequest
{
    public Guid ClienteId { get; set; }
    public List<VentaDetalle> Detalles { get; set; } = new();
}