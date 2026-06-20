using inventarioWebAI.Aplicacion.DTOs;
using inventarioWebAI.Aplicacion.DTOs.AuthDTO;
using inventarioWebAI.Aplicacion.Interfaces.IAuth;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Infraestructura.Auth;
using System.Text;
using System.Text.Json;

namespace inventarioWebAI.Aplicacion.Servicios.Auth
{
    public class AuthServicio : IAuthServicio
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly TokenStore _tokenStore;

        public AuthServicio(HttpClient httpClient, IConfiguration config, TokenStore tokenStore)
        {
            _httpClient = httpClient;
            _config = config;
            _tokenStore = tokenStore;
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
            {
                throw new Exception(content);
            }

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