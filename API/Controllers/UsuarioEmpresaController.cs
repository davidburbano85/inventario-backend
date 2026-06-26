//using inventarioWebAI.Aplicacion.DTOs;
//using inventarioWebAI.Aplicacion.DTOs.UsuarioEmpresa;
//using inventarioWebAI.Aplicacion.Enums;
//using inventarioWebAI.Aplicacion.Interfaces.IAuth;
//using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;

//namespace inventarioWebAI.API.Controllers;

//[ApiController]
//[Route("api/usuarios")]
//[Authorize]
//public class UsuarioEmpresaController : ControllerBase
//{
//    private readonly IUsuarioEmpresaServicio _service;
//    private readonly IAuthServicio _authServicio;

//   public UsuarioEmpresaController(IUsuarioEmpresaServicio service,
//                                    IAuthServicio authServicio)
//    {
//        _service = service;
//        _authServicio = authServicio;
//    }


//    // OBTENER POR ID
//    [HttpGet("{id:guid}")]
//    public async Task<ActionResult<UsuarioEmpresaDTO>> ObtenerPorEmpresaUsuarioAsync(Guid id)
//    {
//        try
//        {
//            var empresaIdClaim = User.FindFirst("empresaId")?.Value;

//            if (string.IsNullOrEmpty(empresaIdClaim) || !Guid.TryParse(empresaIdClaim, out var empresaId))
//                return Unauthorized("Empresa no válida en el token");

//            var result = await _service.ObtenerPorEmpresaUsuarioAsync(empresaId, id);

//            if (result == null || !result.Any())
//                return NotFound();

//            return Ok(result);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, ex.Message);
//        }
//    }

//    [HttpPost]
//    public async Task<ActionResult<Guid>> CrearEmpresaUsuarioAsync([FromBody] CrearUsuarioEmpresaRequest request)
//    {
//        try
//        {
//            var empresaIdClaim = User.FindFirst("empresaId")?.Value;

//            if (string.IsNullOrEmpty(empresaIdClaim) || !Guid.TryParse(empresaIdClaim, out var empresaId))
//                return Unauthorized("Empresa no válida en el token");

//            var id = await _service.CrearAsync(empresaId, request.UsuarioId, request.Rol);

//            return CreatedAtAction(nameof(ObtenerPorEmpresaUsuarioAsync), new { id }, id);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, ex.Message);
//        }
//    }


//    //actualizar rol
//    [HttpPut("{usuarioId:guid}/empresas/{empresaId:guid}/rol")]
//    public async Task<IActionResult> ActualizarRolEmpresaUsuarioAsync(Guid usuarioId, Guid empresaId,
//       [FromBody] RolUsuarioEmpresaDTO rol)
//    {
//        try
//        {
//            var empresaIdClaim = User.FindFirst("empresaId")?.Value;

//            if (string.IsNullOrEmpty(empresaIdClaim) || !Guid.TryParse(empresaIdClaim, out var empresaIdToken))
//                return Unauthorized("Empresa no válida");

//            if (empresaIdToken != empresaId)
//                return Forbid("No puedes modificar otra empresa");

//            await _service.ActualizarRolAsync(usuarioId, empresaId, rol);

//            return NoContent();
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, ex.Message);
//        }
//    }

//    //eliminar usuario de empresa
//    [HttpDelete("{id:guid}")]
//    public async Task<IActionResult> EliminarEmpresaUsuarioAsync(Guid id)
//    {
//        try
//        {
//            var empresaIdClaim = User.FindFirst("empresaId")?.Value;

//            if (string.IsNullOrEmpty(empresaIdClaim))
//                return Unauthorized("Empresa no válida");

//            var result = await _service.EliminarAsync(id);

//            return Ok(result);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, ex.Message);
//        }
//    }


//    [HttpPost("empresas/seleccionar")]
//    public async Task<IActionResult> SeleccionarEmpresaAsync([FromBody] SeleccionarEmpresaRequest request)
//    {
//        Console.WriteLine($"[SeleccionarEmpresa] EmpresaId: {request.EmpresaId}");

//        try
//        {
//            // ?? obtener usuario autenticado desde Supabase JWT
//            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
//                ?? User.FindFirst("sub")?.Value;

//            if (string.IsNullOrEmpty(usuarioId) || !Guid.TryParse(usuarioId, out var userIdGuid))
//                return Unauthorized("Usuario inválido");

//            // ?? llamar al servicio (ya no retorna token)
//            await _service.SeleccionarEmpresaAsync(userIdGuid, request.EmpresaId);

//            // ? respuesta simple de confirmación
//            return Ok(new
//            {
//                message = "Empresa seleccionada correctamente"
//            });
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"[SeleccionarEmpresa ERROR] {ex.Message}");
//            return StatusCode(500, ex.Message);
//        }
//    }
//    [HttpPost("empresa/login")]
//    public async Task<IActionResult> LoginEmpresaAsync([FromBody] LoginEmpresaRequest request)
//    {
//        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

//        if (!Guid.TryParse(userId, out var usuarioId))
//            return Unauthorized("Usuario no válido");

//        var result = await _authServicio.LoginEmpresaAsync(usuarioId, request.EmpresaId);

//        return Ok(result);
//    }



//}

using inventarioWebAI.Aplicacion.DTOs.UsuarioEmpresa;
using inventarioWebAI.Aplicacion.Enums;
using inventarioWebAI.Aplicacion.Interfaces.IAuth;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inventarioWebAI.API.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize]
public class UsuarioEmpresaController : ControllerBase
{
    private readonly IUsuarioEmpresaServicio _service;
    private readonly IAuthServicio _authServicio;

    public UsuarioEmpresaController(
        IUsuarioEmpresaServicio service,
        IAuthServicio authServicio)
    {
        _service = service;
        _authServicio = authServicio;
    }

    // -------------------------
    // OBTENER RELACIONES
    // -------------------------
    [HttpGet("usuario/{usuarioId:guid}")]
    public async Task<IActionResult> ObtenerPorEmpresaUsuarioAsync(Guid usuarioId)
    {
        try
        {
            var result = await _service.ObtenerPorEmpresaUsuarioAsync(usuarioId);

            if (!result.Any())
                return NotFound();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // -------------------------
    // CREAR
    // -------------------------
    [HttpPost]
    public async Task<IActionResult> CrearEmpresaUsuarioAsync(
        [FromBody] CrearUsuarioEmpresaRequest request)
    {
        try
        {
            var id = await _service.CrearAsync(
                request.UsuarioId,
                request.Rol);
            return Ok(new
            {
                id
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // -------------------------
    // ACTUALIZAR ROL
    // -------------------------
    [HttpPut("{usuarioId:guid}/rol")]
    public async Task<IActionResult> ActualizarRolEmpresaUsuarioAsync(
        Guid usuarioId,
        [FromBody] RolUsuarioEmpresaDTO rol)
    {
        try
        {
            await _service.ActualizarRolAsync(usuarioId, rol);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // -------------------------
    // ELIMINAR
    // -------------------------
    [HttpDelete("{idRelacion:guid}")]
    public async Task<IActionResult> EliminarEmpresaUsuarioAsync(Guid idRelacion)
    {
        try
        {
            var result = await _service.EliminarAsync(idRelacion);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // -------------------------
    // SELECCIONAR EMPRESA
    // -------------------------
    [HttpPost("empresas/seleccionar")]
    public async Task<IActionResult> SeleccionarEmpresaAsync(
        [FromBody] SeleccionarEmpresaRequest request)
    {
        try
        {
            await _service.SeleccionarEmpresaAsync(request.EmpresaId);

            return Ok(new
            {
                message = "Empresa seleccionada correctamente"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // -------------------------
    // LOGIN EMPRESA
    // -------------------------
    [HttpPost("empresa/login")]
    public async Task<IActionResult> LoginEmpresaAsync(
       [FromBody] LoginEmpresaRequest request)
    {
        try
        {
            var result = await _authServicio.LoginEmpresaAsync(request.EmpresaId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
