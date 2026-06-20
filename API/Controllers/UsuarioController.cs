using inventarioWebAI.Aplicacion.DTOs.Usuario;
using inventarioWebAI.Aplicacion.Interfaces.IAuth;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            if (dto == null)
                return BadRequest("El cuerpo de la solicitud no puede ser nulo.");

            if (!telefono.Equals(dto.Telefono, StringComparison.OrdinalIgnoreCase))
                return BadRequest("El teléfono de la ruta no coincide con el del cuerpo.");

            var usuarioActual = await _usuarioServicio.ObtenerPorTelefonoAsync(telefono);
            Console.WriteLine($"Usuario encontrado para teléfono {telefono}: {System.Text.Json.JsonSerializer.Serialize(usuarioActual)}");
            if (usuarioActual == null)
                return NotFound($"No se encontró un usuario con el teléfono {telefono}.");
            var resultado = await _usuarioServicio.ActualizarAsync(dto);
            Console.WriteLine("Resultado de la actualización: " + System.Text.Json.JsonSerializer.Serialize(resultado));
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



    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> ActualizarUsuarioAutenticado([FromBody] UsuarioDTO dto)
    {
        try
        {
            Console.WriteLine("===== INICIO ACTUALIZAR USUARIO =====");

            Console.WriteLine($"DTO recibido: {System.Text.Json.JsonSerializer.Serialize(dto)}");

            var authUserIdString =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            Console.WriteLine($"AUTH USER ID STRING: {authUserIdString}");

            if (string.IsNullOrWhiteSpace(authUserIdString))
            {
                Console.WriteLine("NO SE ENCONTRÓ USER ID EN TOKEN");
                return Unauthorized("Token inválido");
            }

            if (!Guid.TryParse(authUserIdString, out var authUserId))
            {
                Console.WriteLine($"ERROR PARSE GUID: {authUserIdString}");
                return Unauthorized("ID inválido");
            }

            Console.WriteLine($"GUID PARSEADO: {authUserId}");

            var resultado = await _usuarioServicio.ActualizarPorAuthAsync(authUserId, dto);

            Console.WriteLine($"RESULTADO: {resultado}");

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            return StatusCode(500, ex.Message);
        }
    }

}

