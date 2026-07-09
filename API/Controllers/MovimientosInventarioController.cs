using inventarioWebAI.Aplicacion.DTOs.MovimientoInventario;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Dominio.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inventarioWebAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MovimientosInventarioController : ControllerBase
{
    private readonly IMovimientosInventarioServicio _servicio;

    public MovimientosInventarioController(IMovimientosInventarioServicio servicio)
    {
        _servicio = servicio;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarMovimientoRequestDTO request)
    {
        var id = await _servicio.RegistrarAsync(
            request.ProductoId,
            request.AlmacenId,
            request.Cantidad,
            request.Tipo,
            request.Motivo,
            request.Factura
        );

        return Ok(id);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerPorEmpresa()
    {
        var result = await _servicio.ObtenerPorEmpresaAsync();
        return Ok(result);
    }

    [HttpGet("producto/{productoId:guid}")]
    public async Task<IActionResult> ObtenerPorProducto(Guid productoId)
    {
        var result = await _servicio.ObtenerPorProductoAsync(productoId);
        return Ok(result);
    }

    [HttpGet("almacen/{almacenId:guid}")]
    public async Task<IActionResult> ObtenerPorAlmacen(Guid almacenId)
    {
        var result = await _servicio.ObtenerPorAlmacenAsync(almacenId);
        return Ok(result);
    }

    [HttpGet("filtrar")]
    public async Task<IActionResult> Filtrar(
        [FromQuery] Guid? productoId,
        [FromQuery] Guid? almacenId,
        [FromQuery] string? tipo)
    {
        var result = await _servicio.FiltrarAsync(productoId, almacenId, tipo);
        return Ok(result);
    }


    [HttpGet("factura/{factura}")]
    public async Task<IActionResult> EncontrarPorFactura(string factura)
    {
        if (string.IsNullOrWhiteSpace(factura))
            return BadRequest("La factura es obligatoria.");

        var movimiento = await _servicio.EncontrarPorFacturaAsync(factura);

        if (movimiento == null)
            return NotFound($"No existe un movimiento asociado a la factura {factura}.");

        return Ok(movimiento);
    }
}

