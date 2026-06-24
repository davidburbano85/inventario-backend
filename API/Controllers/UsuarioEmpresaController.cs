using inventarioWebAI.Aplicacion.DTOs;
using inventarioWebAI.Aplicacion.DTOs.UsuarioEmpresa;
using inventarioWebAI.Aplicacion.Enums;
using inventarioWebAI.Aplicacion.Interfaces.IAuth;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace inventarioWebAI.API.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize]
public class UsuarioEmpresaController : ControllerBase
{
    private readonly IUsuarioEmpresaServicio _service;
    private readonly IAuthServicio _authServicio;

   public UsuarioEmpresaController(IUsuarioEmpresaServicio service,
                                    IAuthServicio authServicio)
    {
        _service = service;
        _authServicio = authServicio;
    }


    // OBTENER POR ID
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UsuarioEmpresaDTO>> ObtenerPorEmpresaUsuario(Guid id)
    {
        try
        {
            var empresaIdClaim = User.FindFirst("empresaId")?.Value;

            if (string.IsNullOrEmpty(empresaIdClaim) || !Guid.TryParse(empresaIdClaim, out var empresaId))
                return Unauthorized("Empresa no válida en el token");

            var result = await _service.ObtenerPorEmpresaUsuarioAsync(empresaId, id);

            if (result == null || !result.Any())
                return NotFound();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Crear([FromBody] CrearUsuarioEmpresaRequest request)
    {
        try
        {
            var empresaIdClaim = User.FindFirst("empresaId")?.Value;

            if (string.IsNullOrEmpty(empresaIdClaim) || !Guid.TryParse(empresaIdClaim, out var empresaId))
                return Unauthorized("Empresa no válida en el token");

            var id = await _service.CrearAsync(empresaId, request.UsuarioId, request.Rol);

            return CreatedAtAction(nameof(ObtenerPorEmpresaUsuario), new { id }, id);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    //actualizar rol
    [HttpPut("{usuarioId:guid}/empresas/{empresaId:guid}/rol")]
    public async Task<IActionResult> ActualizarRol(Guid usuarioId, Guid empresaId,
       [FromBody] RolUsuarioEmpresaDTO rol)
    {
        try
        {
            var empresaIdClaim = User.FindFirst("empresaId")?.Value;

            if (string.IsNullOrEmpty(empresaIdClaim) || !Guid.TryParse(empresaIdClaim, out var empresaIdToken))
                return Unauthorized("Empresa no válida");

            if (empresaIdToken != empresaId)
                return Forbid("No puedes modificar otra empresa");

            await _service.ActualizarRolAsync(usuarioId, empresaId, rol);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    //eliminar usuario de empresa
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        try
        {
            var empresaIdClaim = User.FindFirst("empresaId")?.Value;

            if (string.IsNullOrEmpty(empresaIdClaim))
                return Unauthorized("Empresa no válida");

            var result = await _service.EliminarAsync(id);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    [HttpPost("empresas/seleccionar")]
    public async Task<IActionResult> SeleccionarEmpresa([FromBody]SeleccionarEmpresaRequest request)
    {
        Console.WriteLine($"[SeleccionarEmpresa] EmpresaId: {request.EmpresaId}");
        try
        {
            //obtener usuario autenticado de supa jwt
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(usuarioId) || !Guid.TryParse(usuarioId, out var userIdGuid))
                return Unauthorized("Usuario invalido");

            //llamar al servicio
            var token = await _service.SeleccionarEmpresaAsync(userIdGuid, request.EmpresaId);
            //responder nuevo jwt de empresa
            return Ok(new
            {
                empresaToken = token,
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);

        }

    }


  [HttpPost("empresa/login")]
    public async Task<IActionResult> LoginEmpresa([FromBody] LoginEmpresaRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userId, out var usuarioId))
            return Unauthorized("Usuario no válido");

        var result = await _authServicio.LoginEmpresaAsync(usuarioId, request.EmpresaId);

        return Ok(result);
    }



}


