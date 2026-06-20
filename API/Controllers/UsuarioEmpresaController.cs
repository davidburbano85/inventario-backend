using inventarioWebAI.Aplicacion.DTOs;
using inventarioWebAI.Aplicacion.DTOs.UsuarioEmpresa;
using inventarioWebAI.Aplicacion.Enums;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace inventarioWebAI.API.Controllers;

[ApiController]
[Route("api/usuarios")]
//[Authorize]
public class UsuarioEmpresaController : ControllerBase
{
    private readonly IUsuarioEmpresaServicio _service;

   public UsuarioEmpresaController(IUsuarioEmpresaServicio service)
    {
        _service = service;
    }


    // OBTENER POR ID
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UsuarioEmpresaDTO>> ObtenerPorEmpresaUsuario(Guid empresaID, Guid id)
    {
        Console.WriteLine($"[UsuarioEmpresaController.ObtenerPorEmpresaUsuario] ENTRADA - ID: {id } EMPRESAID: { empresaID}");
        try
        {
            Console.WriteLine($"[UsuarioEmpresaController.ObtenerPorId] Llamando al servicio");
            var result = await _service.ObtenerPorEmpresaUsuarioAsync( empresaID,id);
            Console.WriteLine($"[UsuarioEmpresaController.ObtenerPorId] Servicio retornó: {(result == null ? "null" : $"{result.Count()} registros")}");
            if (result == null)
            {
                Console.WriteLine($"[UsuarioEmpresaController.ObtenerPorId] Retornando NotFound");
                return NotFound();
            }

            Console.WriteLine($"[UsuarioEmpresaController.ObtenerPorId] Retornando Ok con resultado");
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UsuarioEmpresaController.ObtenerPorId] EXCEPCIÓN: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"[UsuarioEmpresaController.ObtenerPorId] StackTrace: {ex.StackTrace}");
            return StatusCode(500, $"Error al obtener la relación usuario-empresa: {ex.Message}");
        }
    }
    // OBTENER POR EMPRESA
   
   

    // CREAR RELACION USUARIO-EMPRESA

    [HttpPost]
  //  [Authorize(Roles ="admin,  superadmin")]
    public async Task<ActionResult<Guid>> Crear([FromBody] CrearUsuarioEmpresaRequest request)
    {
        Console.WriteLine($"[UsuarioEmpresaController.Crear] ENTRADA - EmpresaId: {request.EmpresaId}, UsuarioId: {request.UsuarioId}, Rol: {request.Rol}");
        try
        {
            Console.WriteLine($"[UsuarioEmpresaController.Crear] Llamando al servicio");
            var id = await _service.CrearAsync(request.EmpresaId, request.UsuarioId, request.Rol);
            Console.WriteLine($"[UsuarioEmpresaController.Crear] Servicio retornó: {id}");

            Console.WriteLine($"[UsuarioEmpresaController.Crear] Retornando CreatedAtAction");
            return CreatedAtAction(nameof(ObtenerPorEmpresaUsuario), new { id }, id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UsuarioEmpresaController.Crear] EXCEPCIÓN: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"[UsuarioEmpresaController.Crear] StackTrace: {ex.StackTrace}");
            return StatusCode(500, $"Error al crear la relación usuario-empresa: {ex.Message}");
        }
    }

   
    //actualizar rol
    [HttpPut("{usuarioId:guid}/empresas/{empresaId:guid}/rol")]
    public async Task<IActionResult> ActualizarRol(Guid usuarioId, Guid empresaId,
      [FromBody] RolUsuarioEmpresaDTO rol)
    {
        Console.WriteLine($"[UsuarioEmpresaController.ActualizarRol] ENTRADA - UsuarioId: {usuarioId}, EmpresaId: {empresaId}, Rol: {rol}");
        try
        {
            Console.WriteLine($"[UsuarioEmpresaController.ActualizarRol] Llamando al servicio");
            await _service.ActualizarRolAsync(usuarioId, empresaId, rol);
            Console.WriteLine($"[UsuarioEmpresaController.ActualizarRol] Servicio completó exitosamente");

            Console.WriteLine($"[UsuarioEmpresaController.ActualizarRol] Retornando NoContent");
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UsuarioEmpresaController.ActualizarRol] EXCEPCIÓN: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"[UsuarioEmpresaController.ActualizarRol] StackTrace: {ex.StackTrace}");
            return StatusCode(500, $"Error al actualizar el rol: {ex.Message}");
        }
    }

    //eliminar usuario de empresa
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        Console.WriteLine($"[UsuarioEmpresaController.Eliminar] ENTRADA - ID: {id}");
        try
        {
            Console.WriteLine($"[UsuarioEmpresaController.Eliminar] Llamando al servicio");
            var result = await _service.EliminarAsync(id);
            Console.WriteLine($"[UsuarioEmpresaController.Eliminar] Servicio retornó: {result ?? "[NULL]"}");

            Console.WriteLine($"[UsuarioEmpresaController.Eliminar] Retornando Ok");
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UsuarioEmpresaController.Eliminar] EXCEPCIÓN: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"[UsuarioEmpresaController.Eliminar] StackTrace: {ex.StackTrace}");
            return StatusCode(500, $"Error al eliminar la relación usuario-empresa: {ex.Message}");
        }
    }



}


