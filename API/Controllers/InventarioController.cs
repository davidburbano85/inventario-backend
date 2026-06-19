//// Ubicación: /src/API/Controllers/InventarioController.cs

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using inventarioWebAI.Aplicacion.DTOs;
//using inventarioWebAI.Aplicacion.Interfaces.Iservicios;

//namespace inventarioWebAI.API.Controllers;

//// NUEVO: Controller para exponer endpoints de inventario
//// POR QUÉ:
//// Actualmente solo existe ProductoController
//// Inventario (movimientos) ya tiene servicio pero no API pública
//// Esto bloquea uso desde frontend
//[ApiController]
//[Route("api/inventario")]
//[Authorize]
//public class InventarioController : ControllerBase
//{
//    private readonly IInventarioServicio _servicio;

//    // NUEVO: inyección de servicio existente
//    public InventarioController(IInventarioServicio servicio)
//    {
//        _servicio = servicio;
//    }

//    // GET: api/inventario/{empresaId}
//    // NUEVO: expone consulta de movimientos
//    [HttpGet("{empresaId}")]
//    public async Task<IActionResult> ObtenerMovimientos(Guid empresaId)
//    {
//        var resultado = await _servicio.ObtenerMovimientos(empresaId); // reutiliza servicio existente

//        return Ok(resultado);
//    }

//    // POST: api/inventario
//    // NUEVO: permite registrar movimientos manuales
//    [HttpPost]
//    public async Task<IActionResult> RegistrarMovimiento([FromBody] CrearMovimientoDTO dto)
//    {
//        if (!ModelState.IsValid)
//            return BadRequest(ModelState);

//        // NUEVO: validaciones mínimas antes de delegar al servicio
//        if (dto.Cantidad <= 0) // coherente con CHECK en BD (>0)
//            return BadRequest("La cantidad debe ser mayor a 0.");

//        if (dto.EmpresaId == Guid.Empty) // evita datos inválidos multi-tenant
//            return BadRequest("EmpresaId es requerido.");

//        if (dto.ProductoId == Guid.Empty)
//            return BadRequest("ProductoId es requerido.");

//        if (dto.AlmacenId == Guid.Empty)
//            return BadRequest("AlmacenId es requerido.");

//        if (dto.UsuarioId == Guid.Empty)
//            return BadRequest("UsuarioId es requerido.");

//        await _servicio.RegistrarMovimiento(dto); // delega lógica (incluye validación de stock)

//        return Ok(); // respuesta simple
//    }
//}