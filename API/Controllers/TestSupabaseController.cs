//using Microsoft.AspNetCore.Mvc;
//using System.Text;
//using System.Text.Json;
//using System.Net.Http;

//[ApiController]
//[Route("test-supabase")]
//public class TestSupabaseController : ControllerBase
//{
//    private readonly IHttpClientFactory _httpFactory;

//    public TestSupabaseController(IHttpClientFactory httpFactory)
//    {
//        _httpFactory = httpFactory;
//    }

//    // GET: test-supabase/auth?email=...&password=...
//    // Nota: por simplicidad este endpoint acepta credenciales por query string.
//    // En producción usa POST y HTTPS, y nunca expongas credenciales en logs.
//    [HttpGet("auth")]
//    public async Task<IActionResult> SignIn([FromQuery] string? email, [FromQuery] string? password)
//    {
//        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
//            return BadRequest(new { message = "Email y password son requeridos como query params." });

//        try
//        {
//            var client = _httpFactory.CreateClient("Supabase");

//            var payload = new
//            {
//                grant_type = "password",
//                email,
//                password
//            };

//            var json = JsonSerializer.Serialize(payload);
//            using var content = new StringContent(json, Encoding.UTF8, "application/json");

//            // POST al endpoint de token de Supabase
//            var resp = await client.PostAsync("/auth/v1/token?grant_type=password", content);
//            var body = await resp.Content.ReadAsStringAsync();

//            if (resp.IsSuccessStatusCode)
//            {
//                // devolver el JSON recibido (contiene access_token, refresh_token, etc.)
//                var doc = JsonDocument.Parse(body);
//                return Ok(new { status = (int)resp.StatusCode, token = doc.RootElement });
//            }

//            return BadRequest(new { status = (int)resp.StatusCode, error = body });
//        }
//        catch (Exception ex)
//        {
//            return BadRequest(new { message = "Error al autenticar en Supabase", error = ex.Message });
//        }
//    }
//    //el siguiente get realiza varias pruebas a diferentes endpoints
//    //de supabase para diagnosticar el error 404 que se esta presentando,
//    //devolviendo el status, reason phrase, un preview del body y las cabeceras de cada intento
//    [HttpGet]
//    public async Task<IActionResult> TestSupabase()
//    {
//        try
//        {
//            var client = _httpFactory.CreateClient("Supabase");
//            // Probar varios endpoints comunes de Supabase para diagnosticar 404
//            var endpoints = new[] { "/rest/v1", "/auth/v1", "/storage/v1", "/" };
//            var results = new List<object>();

//            foreach (var ep in endpoints)
//            {
//                HttpResponseMessage resp;
//                try
//                {
//                    resp = await client.GetAsync(ep);
//                }
//                catch (Exception ex)
//                {
//                    results.Add(new { endpoint = ep, error = ex.Message });
//                    continue;
//                }

//                var body = string.Empty;
//                if (resp.Content != null)
//                {
//                    var content = await resp.Content.ReadAsStringAsync();
//                    body = content.Length > 1024 ? content.Substring(0, 1024) + "..." : content;
//                }

//                // obtener algunas cabeceras relevantes
//                var headers = resp.Headers.ToDictionary(h => h.Key, h => string.Join(',', h.Value));

//                results.Add(new
//                {
//                    endpoint = ep,
//                    status = (int)resp.StatusCode,
//                    reason = resp.ReasonPhrase,
//                    bodyPreview = body,
//                    headers
//                });
//            }

//            return Ok(new { project = client.BaseAddress?.ToString(), attempts = results });
//        }
//        catch (Exception ex)
//        {
//            return BadRequest(new
//            {
//                message = "Error al conectar con Supabase",
//                error = ex.Message
//            });
//        }
//    }
//}
