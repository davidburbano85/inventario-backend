//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using inventarioWebAI.Aplicacion.DTOs;
//using inventarioWebAI.Aplicacion.Interfaces.Iservicios;

//namespace inventarioWebAI.API.Controllers;

//[ApiController]
//[Route("api/categorias")]
//[Authorize]
//public class CategoriaController : ControllerBase
//{
//    private readonly ICategoriaServicio _servicio;

//    public CategoriaController(ICategoriaServicio servicio)
//    {
//        _servicio = servicio;
//    }

//    [HttpGet("{empresaId}")]
//    public async Task<IActionResult> ObtenerPorEmpresa(Guid empresaId)
//    {
//        var result = await _servicio.ObtenerPorEmpresa(empresaId);
//        return Ok(result);
//    }

//    [HttpPost]
//    public async Task<IActionResult> Crear([FromBody] CrearCategoriaDTO dto)
//    {
//        if (!ModelState.IsValid) return BadRequest(ModelState);
//        var id = await _servicio.Crear(dto);
//        return Ok(new { id });
//    }
//}
