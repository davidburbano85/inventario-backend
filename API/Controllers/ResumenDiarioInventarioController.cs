using inventarioWebAI.Aplicacion.DTOs.ResumenDiarioInventario;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using Microsoft.AspNetCore.Mvc;

namespace inventarioWebAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResumenDiarioInventarioController : ControllerBase
    {
        private readonly IResumenDiarioInventarioServicio _servicio;

        public ResumenDiarioInventarioController(IResumenDiarioInventarioServicio servicio)
        {
            _servicio = servicio;
        }

        [HttpPost("generar")]
        public async Task<IActionResult> Generar([FromBody] CrearResumenDiarioDto request)
        {
            if (request == null)
                return BadRequest("Request inválido.");

            var result = await _servicio.GenerarResumenDiarioAsync(
                request.AlmacenId,
                request.Fecha
            );

            return Ok(new { RegistrosProcesados = result });
        }

        [HttpPost("generar-rango")]
        public async Task<IActionResult> GenerarRango([FromBody] GenerarResumenRangoRequestDto request)
        {
            if (request == null)
                return BadRequest("Request inválido.");

            var result = await _servicio.GenerarResumenPorRangoAsync(
                request.AlmacenId,
                request.FechaInicio,
                request.FechaFin
            );

            return Ok(new { RegistrosProcesados = result });
        }

        [HttpGet("rango")]
        public async Task<IActionResult> ObtenerRango(
            [FromQuery] Guid almacenId,
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            var result = await _servicio.ObtenerPorRangoAsync(
                almacenId,
                fechaInicio,
                fechaFin
            );

            return Ok(result);
        }

        [HttpGet("producto")]
        public async Task<IActionResult> ObtenerPorProducto(
            [FromQuery] Guid almacenId,
            [FromQuery] Guid productoId,
            [FromQuery] DateTime fecha)
        {
            var result = await _servicio.ObtenerPorProductoYFechaAsync(
                almacenId,
                productoId,
                fecha
            );

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("recalcular-producto")]
        public async Task<IActionResult> RecalcularProducto([FromBody] RecalcularProductoRequestDto request)
        {
            if (request == null)
                return BadRequest("Request inválido.");

            var result = await _servicio.RecalcularProductoAsync(
                request.AlmacenId,
                request.ProductoId,
                request.FechaInicio,
                request.FechaFin
            );

            return Ok(new { Success = result });
        }

        // =====================================================
        // NUEVOS ENDPOINTS (MONITOREO)
        // =====================================================

        [HttpPost("ejecutar-diario")]
        public async Task<IActionResult> EjecutarDiario([FromQuery] Guid empresaId, [FromQuery] Guid? almacenId = null)
        {
            var result = await _servicio.EjecutarDiarioAsync(empresaId, almacenId);
            return Ok(new { MonitoreoId = result });
        }
    }
}