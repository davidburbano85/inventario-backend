//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using System.Security.Claims;
//using inventarioWebAI.Aplicacion.DTOs;
//using inventarioWebAI.Aplicacion.Interfaces.Iservicios;

//namespace inventarioWebAI.API.Controllers;

//[ApiController]
//[Route("api/ventas")]
//[Authorize]
//public class VentasController : ControllerBase
//{
//    private readonly IVentasServicio _servicio;

//    public VentasController(IVentasServicio servicio)
//    {
//        _servicio = servicio;
//    }

//    private Guid? GetRequesterId()
//    {
//        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//        if (Guid.TryParse(sub, out var id)) return id;
//        return null;
//    }

//    [HttpGet("{empresaId}")]
//    public async Task<IActionResult> ObtenerPorEmpresa(Guid empresaId)
//    {
//        var result = await _servicio.ObtenerPorEmpresa(empresaId);
//        return Ok(result);
//    }

//    [HttpPost]
//    public async Task<IActionResult> Crear([FromBody] CrearVentaDTO dto)
//    {
//        var requester = GetRequesterId();
//        if (requester is null) return Unauthorized();

//        if (!ModelState.IsValid) return BadRequest(ModelState);

//        dto.UsuarioId = requester.Value;

//        var id = await _servicio.Crear(dto);
//        return Ok(new { id });
//    }
//}
