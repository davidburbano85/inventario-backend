//public class ApiKeyMiddleware
//{
//    private readonly RequestDelegate _next;
//    private readonly IConfiguration _config;

//    private const string HEADER_API_KEY = "X-API-KEY";

//    public ApiKeyMiddleware(RequestDelegate next, IConfiguration config)
//    {
//        _next = next;
//        _config = config;
//    }

//    public async Task InvokeAsync(HttpContext context)
//    {
//        // No aplicar validación en rutas públicas
//        if (!context.Request.Path.StartsWithSegments("/public"))
//        {
//            await _next(context);
//            return;
//        }

//        var clientApiKey = context.Request.Headers[HEADER_API_KEY].FirstOrDefault();// obtenemos la API key enviada por el cliente en el header "X-API-KEY"
//        var validApiKey = _config["Supabase:AnonKey"]; // obtenemos la API key válida desde la configuración (appsettings.json)]


//        if (string.IsNullOrWhiteSpace(clientApiKey) ||
//            !string.Equals(clientApiKey, validApiKey, StringComparison.Ordinal))
//        {
//            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//            await context.Response.WriteAsync("Unauthorized: Invalid or missing API key.");
//            return;
//        }

//        await _next(context);
//    }
//}