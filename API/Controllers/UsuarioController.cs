using inventarioWebAI.Aplicacion.DTOs.Usuario;
using inventarioWebAI.Aplicacion.Interfaces.IAuth;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inventarioWebAI.API.Controllers;
[ApiController]
[Route("api/usuario")]
//[Authorize]

public class UsuarioController : ControllerBase
{
    private readonly IUsuarioServicio _usuarioServicio;
    public UsuarioController(IUsuarioServicio usuarioServicio)
    {
        _usuarioServicio = usuarioServicio;
    }
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        try
        {
            var usuarios = await _usuarioServicio.ListarAsync();
            return Ok(usuarios);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al listar los usuarios: {ex.Message}");
        }
    }

    [HttpGet("{idUsuario}")]
    public async Task<IActionResult> ObtenerPorId(Guid idUsuario)
    {
        try
        {
            var usuario = await _usuarioServicio.ObtenerPorIdAsync(idUsuario);
            if (usuario == null)
                return NotFound($"No se encontró un usuario con el ID {idUsuario}.");
            return Ok(usuario);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al obtener el usuario: {ex.Message}");
        }
    }

    [HttpPut("{telefono}")]
    public async Task<IActionResult> ActualizarTelefono(string telefono, [FromBody] UsuarioDTO dto)
    {
        try
        {
            var usuarioActualizado = await _usuarioServicio.ObtenerPorTelefonoAsync(telefono);
            if (usuarioActualizado == null)
                return NotFound($"No se encontró un usuario con el teléfono {telefono}.");
            var resultado = await _usuarioServicio.ActualizarAsync(dto);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al actualizar el usuario: {ex.Message}");
        }
    }


    [HttpDelete("{idUsuario:guid}")]
    public async Task<IActionResult> EliminarUsuarioCompletamente(Guid idUsuario)
    {
        try
        {
            var resultado = await _usuarioServicio.EliminarAsync(idUsuario);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al eliminar el usuario: {ex.Message}");
        }
    }

    [HttpPut("me")]//796acbd0-a56f-4f0a-a299-10e58e1e4fc8
    public async Task<IActionResult> ActualizarUsuarioAutenticado([FromBody] UsuarioDTO dto)
    {
        try
        {


            foreach (var c in User.Claims)
            {
                Console.WriteLine($"{c.Type} -> {c.Value}");
            }

            var authUserIdString =
           User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            Console.WriteLine($"AUTH USER ID STRING controller: {authUserIdString}");



            var claimSub = User.Claims.FirstOrDefault(c =>
                    c.Type == "sub" ||
                    c.Type == System.Security.Claims.ClaimTypes.NameIdentifier
                );






            if (string.IsNullOrEmpty(authUserIdString))
            {
                return Unauthorized("No se encontró el ID del usuario en el token");
            }

            if (!Guid.TryParse(authUserIdString, out var authUserId))
            {

                return Unauthorized("El ID del usuario no es un GUID válido");
            }



            var resultado = await _usuarioServicio.ActualizarPorAuthAsync(authUserId, dto);



            return Ok(resultado);
        }
        catch (Exception ex)
        {


            return StatusCode(500, $"Error al actualizar el usuario autenticado: {ex.Message}");
        }
    }





}

