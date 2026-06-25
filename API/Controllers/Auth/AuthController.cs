using inventarioWebAI.Aplicacion.DTOs;
using inventarioWebAI.Aplicacion.DTOs.AuthDTO;
using inventarioWebAI.Aplicacion.Interfaces.IAuth;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Dominio.Entidades;
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
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    public AuthController(IUsuarioServicio usuarioServicio, 
                                    IAuthServicio authServicio,
                                    HttpClient httpClient,
                                    IConfiguration configuration,
                                    IUsuarioRepositorio usuarioRepositorio)
    {
        _usuarioServicio = usuarioServicio;
        _authServicio = authServicio;
        _httpClient = httpClient;
        _config = configuration;
        _usuarioRepositorio = usuarioRepositorio;
    }
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
    public async Task<IActionResult> crear([FromBody] LoginDto dto)
    {
        Console.WriteLine("===== SIGNUP START =====");

        try
        {
            // 1. Crear usuario en Supabase Auth
            var result = await _authServicio.SignupAsync(dto.Email, dto.Password);

            Console.WriteLine($"UserId Auth: {result.UserId}");

            if (result.UserId == Guid.Empty)
            {
                Console.WriteLine("ERROR: UserId vacío");
                return BadRequest("No se pudo obtener el UserId de Supabase");
            }

            // 2. Crear usuario en tu tabla local (SIN auth_user_id)
            var usuario = new Usuario
            {
                Id = result.UserId,   // 🔥 MISMO ID DE SUPABASE
                Nombre = dto.Email,
                Telefono = "",
                CreatedAt = DateTime.UtcNow
            };

            var creado = await _usuarioRepositorio.CrearAsync(usuario);

            if (creado!=0)
            {
                Console.WriteLine("ERROR: No se pudo insertar en tabla usuarios");
                return StatusCode(500, "Error creando usuario en base de datos");
            }

            Console.WriteLine("===== SIGNUP END OK =====");

            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine("===== SIGNUP ERROR =====");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);

            return StatusCode(500, $"Error en signup: {ex.Message}");
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

