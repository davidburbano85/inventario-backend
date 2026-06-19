using inventarioWebAI.Aplicacion.DTOs;
using inventarioWebAI.Aplicacion.DTOs.AuthDTO;
using inventarioWebAI.Aplicacion.Interfaces.IAuth;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Infraestructura.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace inventarioWebAI.API.Controllers.Auth;

[ApiController]
[Route("api/auth")]

public class AuthController : ControllerBase
{
    private readonly IUsuarioServicio _usuarioServicio;
    private readonly IAuthServicio _authServicio;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public AuthController(IUsuarioServicio usuarioServicio, 
                                    IAuthServicio authServicio,
                                    HttpClient httpClient,
                                    IConfiguration configuration)
    {
        _usuarioServicio = usuarioServicio;
        _authServicio = authServicio;
        _httpClient = httpClient;
        _config = configuration;    }
    // =========================
    // LOGIN
    // =========================

    [HttpPost("/api/auth/login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            Console.WriteLine("========== LOGIN ==========");
            Console.WriteLine($"Email recibido: {dto.Email}");

            var result = await _authServicio.LoginAsync(dto.Email, dto.Password);

            Console.WriteLine("========== RESPUESTA LOGIN ==========");


            // 🔥 LOG DEL USER ID
            if (result.UserId!=Guid.Empty)
            {
                Console.WriteLine($"AUTH USER ID: {result.UserId}");
            }
            else
            {
                Console.WriteLine("NO SE RECIBIÓ USER ID EN EL RESULTADO");
            }

            Console.WriteLine("=====================================");

            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR EN LOGIN:");
            Console.WriteLine(ex.Message);

            return StatusCode(500, $"Error al iniciar sesión: {ex.Message}");
        }
    }
    // =========================
    // SIGNUP
    // =========================
    [HttpPost("signup")]
    public async Task<IActionResult> crear([FromBody] LoginDto dto)// aunque el dto se llama login, lo usaremos para el signup,
                                                                    // ya que solo tiene email y password, lo que es suficiente
                                                                    // para el registro
    {
        Console.WriteLine("entro al servicio");                                                         // usuario en nuestra base de datos

        try
        {
            var result = await _authServicio.SignupAsync(dto.Email, dto.Password);// este método se encargará de registrar el
            Console.WriteLine("salio del servicio");                                                          // usuario en supabase auth y luego crear el
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex}");
            return BadRequest(ex.Message);
        }
    }



    // =========================
    // REFRESCAR TOKEN
    // =========================
    [HttpPost("refresh-token")]

   
    public async Task<IActionResult> RefreshToken([FromBody] AuthRespuestasDto dto)
    //creamos este endpoint para refrescar el token de acceso utilizando el refresh
    //token que se obtiene al iniciar sesión o registrarse. El cliente enviará el
    //refresh token authResponseDto, y el servidor validará el refresh token y,
    //si es válido, generará un nuevo token de acceso y un nuevo refresh token,
    //que se devolverán al cliente. Esto permite mantener la sesión del usuario
    //activa sin necesidad de que vuelva a iniciar sesión cada vez que el token
    //de acceso expire.

    {
        try
        {
            var result = await _authServicio.RefreshTokenAsync(dto.RefreshToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    // =========================
    // OBTENER USUARIO POR AUTHUSERID
    // =========================


    [HttpGet("auth/{authUserId:guid}")]
    public async Task<IActionResult> ObtenerPorAuthUserId(Guid authUserId)
    {
        try
        {
            var usuario = await _usuarioServicio.ObtenerPorAuthUserIdAsync(authUserId);
            if (usuario == null)
                return NotFound($"No se encontró un usuario con el AuthUserId {authUserId}.");
            return Ok(usuario);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al obtener el usuario por AuthUserId: {ex.Message}");
        }
    }





  
}

