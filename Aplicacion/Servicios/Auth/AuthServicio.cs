using inventarioWebAI.Aplicacion.DTOs;
using inventarioWebAI.Aplicacion.DTOs.AuthDTO;
using inventarioWebAI.Aplicacion.Interfaces.Context;
using inventarioWebAI.Aplicacion.Interfaces.IAuth;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Infraestructura.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

namespace inventarioWebAI.Aplicacion.Servicios.Auth
{
    public class AuthServicio : IAuthServicio
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly TokenStore _tokenStore;
        private readonly IUsuarioEmpresaRepositorio _usuarioEmpresaRepositorio;
        private readonly IJwtServicio _jwtServicio;
        private readonly IUsuarioContext _usuarioContext;



        public AuthServicio(HttpClient httpClient, 
                            IConfiguration config, 
                            TokenStore tokenStore,
                            IUsuarioEmpresaRepositorio usuarioEmpresaRepositorio,
                            IJwtServicio jwtServicio, IUsuarioContext usuarioContext)
        {
            _httpClient = httpClient;
            _config = config;
            _tokenStore = tokenStore;
            _usuarioEmpresaRepositorio= usuarioEmpresaRepositorio;
            _jwtServicio = jwtServicio;
            _usuarioContext = usuarioContext;
        }

        public async Task<AuthRespuestasDto> LoginAsync(string email, string password)
        {
            var url = "https://egqgezxlgaajfrxmwvih.supabase.co/auth/v1/token?grant_type=password";
            var anonKey = _config.GetSection("Supabase")["AnonKey"];

            var request = new
            {
                email,
                password
            };

            var requestJson = JsonSerializer.Serialize(request);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
            httpRequest.Headers.Add("apikey", anonKey);
            httpRequest.Content = new StringContent(
                requestJson,
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(httpRequest);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(content);

            var json = JsonSerializer.Deserialize<JsonElement>(content);

            var accessToken = json.GetProperty("access_token").GetString();

            var userId = json.GetProperty("user")
                .GetProperty("id")
                .GetString();

            return new AuthRespuestasDto
            {
                AccessToken = accessToken,
                UserId = Guid.Parse(userId)
            };
        }
        public async Task<AuthRespuestasDto> SignupAsync(string email, string password)
        {
            Console.WriteLine("========== SIGNUP START ==========");

            var url = "https://egqgezxlgaajfrxmwvih.supabase.co/auth/v1/signup";
            var anonKey = _config.GetSection("Supabase")["AnonKey"];

            if (string.IsNullOrWhiteSpace(anonKey))
                throw new Exception("Supabase AnonKey no configurada");

            var request = new
            {
                email,
                password
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);

            httpRequest.Headers.Add("apikey", anonKey);
            httpRequest.Headers.Add("Authorization", $"Bearer {anonKey}");

            httpRequest.Content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(httpRequest);
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine(content);

            if (!response.IsSuccessStatusCode)
                throw new Exception(content);

            var json = JsonSerializer.Deserialize<JsonElement>(content);

            var accessToken = json.GetProperty("access_token").GetString();
            var refreshToken = json.GetProperty("refresh_token").GetString();

            // 🔥 AQUÍ ESTÁ EL CAMBIO REAL
            var userId = json.GetProperty("user").GetProperty("id").GetString();

            Console.WriteLine($"UserId: {userId}");

            Console.WriteLine("========== SIGNUP END ==========");

            return new AuthRespuestasDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = Guid.Parse(userId)
            };
        }
      
        public async Task<AuthRespuestasDto> LoginEmpresaAsync(Guid empresaId)
        {
            try
            {
                var userId = _usuarioContext.ObtenerAuthUserId();

                // 1. Validar relación usuario-empresa
                var relacion = await _usuarioEmpresaRepositorio
                    .ObtenerPorUsuarioYEmpresaAsync(userId, empresaId);

                if (relacion == null)
                    throw new UnauthorizedAccessException("El usuario no pertenece a esta empresa.");

                // 2. Desactivar todas las empresas del usuario
                await _usuarioEmpresaRepositorio.DesactivarTodasAsync(userId);

                // 3. Activar la empresa seleccionada
                await _usuarioEmpresaRepositorio.ActivarEmpresaAsync(userId, empresaId);

                // 4. Generar JWT interno de empresa
                var tokenInterno = _jwtServicio.generarToken(userId, empresaId);

                return new AuthRespuestasDto
                {
                    UserId = userId,
                    TokenInterno = tokenInterno,
                    AccessToken = null,
                    RefreshToken = null
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"[LoginEmpresaAsync] Unauthorized: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoginEmpresaAsync] ERROR GENERAL: {ex.Message}");
                throw new Exception("Error en LoginEmpresaAsync: " + ex.Message, ex);
            }
        }

        public async Task<AuthRespuestasDto> RefreshTokenAsync(string refreshToken)
        {
            var url = "https://egqgezxlgaajfrxmwvih.supabase.co/auth/v1/token?grant_type=refresh_token";

            var anonKey = _config.GetSection("Supabase")["AnonKey"];

            var request = new
            {
                refresh_token = refreshToken
            };
            Console.WriteLine("REFRESH TOKEN REQUEST:");
            Console.WriteLine(refreshToken);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);

            httpRequest.Headers.Add("apikey", anonKey);
            httpRequest.Headers.Add("Authorization", $"Bearer {anonKey}");

            httpRequest.Content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(httpRequest);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(content);

            var json = JsonSerializer.Deserialize<JsonElement>(content);

            return new AuthRespuestasDto
            {

                AccessToken = json.GetProperty("access_token").GetString(),
                RefreshToken = json.GetProperty("refresh_token").GetString()
            };
        }
       
    
    }
}