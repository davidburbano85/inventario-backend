using inventarioWebAI.Aplicacion.DTOs.Comprar;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inventarioWebAI.Api.Controllers;

[ApiController]
[Route("api/compras")]
[Authorize]
public class ComprasController : ControllerBase
{
    private readonly IComprasServicio _servicio;

    public ComprasController(IComprasServicio servicio)
    {
        _servicio = servicio;
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearCompraDTO dto)
    {
        try
        {
            // 🔥 CREA LA COMPRA USANDO SOLO DATOS DEL DTO
            // Usuario, empresa y total se resuelven en el backend
            var id = await _servicio.CrearAsync(
                dto.ProveedorId,
                dto.Factura,
                dto.Detalles
            );

            return Created($"/api/compras/{id}", new { id });
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
            // 🔥 OBTIENE TODAS LAS COMPRAS DE LA EMPRESA ACTIVA DEL USUARIO
            var data = await _servicio.ObtenerPorEmpresaAsync();

            if (!data.Any())
                return NotFound("No hay compras");

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
            // 🔥 OBTIENE UNA COMPRA VALIDANDO QUE PERTENEZCA A LA EMPRESA DEL USUARIO
            var data = await _servicio.ObtenerPorIdAsync(id);

            if (data == null)
                return NotFound("Compra no encontrada");

            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id}/detalle")]
    public async Task<IActionResult> ObtenerDetalle(Guid id)
    {
        try
        {
            // 🔥 OBTIENE SOLO LOS DETALLES DE UNA COMPRA
            // No es un CRUD independiente, solo lectura dentro del contexto de compra
            var data = await _servicio.ObtenerDetalleAsync(id);

            if (!data.Any())
                return NotFound("No hay detalles");

            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Anular(Guid id)
    {
        try
        {
            // 🔥 ANULA (ELIMINACIÓN LÓGICA) DE LA COMPRA
            // No se borra físicamente por trazabilidad del inventario
            var ok = await _servicio.AnularAsync(id);

            if (!ok)
                return NotFound("Compra no encontrada");

            return Ok("Compra anulada correctamente");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("factura/{empresaId:guid}/{factura}")]
    public async Task<IActionResult> EncontrarPorFactura(Guid empresaId, string factura)
    {
        try
        {
            var existe = await _servicio.EncontrarPorFacturaAsync(empresaId, factura);

            return Ok(existe);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

}