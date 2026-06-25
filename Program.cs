// interfaces del proyecto
using inventarioWebAI.Aplicacion.Interfaces.Context;
using inventarioWebAI.Aplicacion.Interfaces.IAuth;
using inventarioWebAI.Aplicacion.Interfaces.IPermmisoServicios;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Aplicacion.Servicios;
using inventarioWebAI.Aplicacion.Servicios.Auth;
using inventarioWebAI.Aplicacion.Servicios.PermisoServicios;
using inventarioWebAI.Aplicacion.Servicios.ServiciosDto;
using inventarioWebAI.Infraestructura.AccesoDatos;
using inventarioWebAI.Infraestructura.Auth;
using inventarioWebAI.Infraestructura.Conexion;
using inventarioWebAI.Infraestructura.Context;
using inventarioWebAI.Infraestructura.RepositoriosDapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

IdentityModelEventSource.ShowPII = true; // Habilita la visualización de información detallada
                                          // en los logs de validación de tokens JWT
                                          // (útil para depuración, pero no recomendado en producción)

var builder = WebApplication.CreateBuilder(args);

//=============================
//logger=============================
builder.Logging.ClearProviders(); // Limpia los proveedores de logging predeterminados
builder.Logging.AddConsole(); // Agrega el proveedor de logging para la consola
builder.Logging.AddDebug(); // Agrega el proveedor de logging para el depurador
// ===========================
// OBTENER CONFIGURACIÓN SUPABASE
// ===========================
var supabaseIssuer = "https://egqgezxlgaajfrxmwvih.supabase.co/auth/v1"; 
var supabaseAudience = builder.Configuration["Supabase:Audience"]; // Obtiene la audiencia de Supabase desde la configuración


// ===========================
// INYECCIÓN DEPENDENCIAS
// ===========================

// ============================Registra la implementación de ======================================
// =============================IDbConnectionFactory usando NpgsqlConnectionFactory ===============
builder.Services.AddScoped<IDbConnectionFactory, NpgsqlConnectionFactory>(); 
//=============================auth=============================
builder.Services.AddHttpClient<IAuthServicio, AuthServicio>(); // Registra el servicio de autenticación con HttpClient para llamadas HTTP a Supabase

// ============================ Permisos =======================
builder.Services.AddScoped<IPermisoServicio, PermisoServicio>();
//============================ Usuario ============================
builder.Services.AddScoped<IUsuarioRepositorio,UsuarioRepositorioDapper>();
builder.Services.AddScoped<IUsuarioServicio, UsuarioServicio>();

//=============================UsuarioEmpresa =====================
builder.Services.AddScoped<IUsuarioEmpresaRepositorio, UsuarioEmpresaRepositorioDapper>();
builder.Services.AddScoped<IUsuarioEmpresaServicio, UsuarioEmpresaServicio>();


builder.Services.AddHttpContextAccessor();// Registra el servicio para acceder al contexto HTTP, necesario para obtener información del usuario autenticado
builder.Services.AddScoped<IUsuarioContext,UsuarioContextService>();// Registra el servicio para obtener información del usuario autenticado a través del contexto HTTP

// =========================== empresa ============================
builder.Services.AddScoped<IEmpresaRepositorio, EmpresaRepositorioDapper>();
builder.Services.AddScoped<IEmpresaServicio, EmpresaServicio>();
// ============================= jwtServicio ======================
builder.Services.AddScoped<IJwtServicio, JwtServicio>();
// ========================= Almacen ==================================
builder.Services.AddScoped<IAlmacenRepositorio, AlmacenRepositorioDapper>();
builder.Services.AddScoped<IAlmacenServicio, AlmacenServicio>();

builder.Logging.ClearProviders();// Limpia los proveedores de logging predeterminados
builder.Logging.AddConsole();// Agrega el proveedor de logging para la consola
builder.Logging.SetMinimumLevel(LogLevel.Debug);// Establece el nivel mínimo de logging en Debug para obtener información detallada durante el desarrollo


builder.Services.AddSingleton<TokenStore>();// Registra la clase TokenStore como un servicio singleton para almacenar el token de acceso y su expiración en memoria durante la ejecución de la aplicación
/*
=========================================
CONFIGURACIÓN AUTENTICACIÓN JWT SUPABASE
=========================================
*/

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options=>
{
    options.Authority = supabaseIssuer; // Establece la URL de Supabase como autoridad para la validación de tokens
    options.Audience = supabaseAudience; // Establece la audiencia esperada para los tokens JWT
    options.RequireHttpsMetadata = false; // Deshabilita la exigencia de HTTPS para el endpoint de metadatos (útil para desarrollo local)
    options.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuer = true, // Habilita la validación del emisor del token
        ValidIssuer = supabaseIssuer, // Establece el emisor válido (la URL de Supabase)
        ValidateAudience = true, // Habilita la validación de la audiencia del token
        ValidAudience = supabaseAudience, // Establece la audiencia válida (configurada en Supabase)
        ValidateLifetime = true, // Habilita la validación de la vida útil del token
        ValidateIssuerSigningKey = true, // Habilita la validación de la firma del token
        ClockSkew = TimeSpan.FromSeconds(30), // Permite un margen de tiempo para la expiración del token (útil para evitar problemas de sincronización de reloj)
        NameClaimType = "sub",
        RoleClaimType = "role"
    };
    options.Events = new JwtBearerEvents
    {
        // =================== EVENTOS JWT SUPABASE =================
     

        // ==========================================
        // 🔥 CUANDO LLEGA EL TOKEN
        // ==========================================

        OnMessageReceived = context =>
        {

            var header = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(header))
            {
                var token = header.Replace("Bearer ", ""); // Extrae el token JWT del encabezado Authorization
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token); // Intenta leer el token JWT para verificar su formato

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al leer el token JWT: {ex.Message}"); // Log de error si el token no se puede leer
                }
            }
            Console.WriteLine($"TOKEN VALIDADO: {context.Principal?.Identity?.IsAuthenticated}");

            return Task.CompletedTask;
        },
        // ==========================================
        // 🔥 CUANDO .NET DESCARGA METADATA OPENID
        // ==========================================
        OnAuthenticationFailed = context =>
            {
                Console.WriteLine("\n===== JWT AUTHENTICATION FAILED =====");
                Console.WriteLine($"Error de autenticación: {context.Exception.Message}"); // Log del error de autenticación para depuración
                Console.WriteLine("===== FIN AUTHENTICATION FAILED =====\n");
                return Task.CompletedTask;
            },
        // ==========================================
        // 🔥 TOKEN VALIDADO
        // ==========================================
        OnTokenValidated = context =>
            {
                Console.WriteLine("\n===== JWT TOKEN VALIDATED =====");
                Console.WriteLine("Token JWT validado correctamente."); // Log de éxito al validar el token
                Console.WriteLine("===== FIN TOKEN VALIDATED =====\n");
                return Task.CompletedTask;
            },
        // ==========================================
        // 🔥 CUANDO FALLA AUTORIZACIÓN
        // ==========================================
        OnChallenge = context =>
            {
                Console.WriteLine("\n===== JWT CHALLENGE OCCURRED =====");
                Console.WriteLine($"Error de desafío: {context.Error}, Descripción: {context.ErrorDescription}"); // Log del error de desafío para depuración
                return Task.CompletedTask;
            },
        // ==========================================
        // 🔥 CUANDO USER NO TIENE PERMISOS
        // ==========================================
        OnForbidden = context =>
            {
                Console.WriteLine("\n===== JWT FORBIDDEN =====");
                Console.WriteLine("Acceso prohibido: el usuario no tiene permisos para acceder al recurso."); // Log de acceso prohibido para depuración
                return Task.CompletedTask;

            }

    };
    Console.WriteLine("===== FIN CONFIG JWT =====");
});



// ===========================
// AUTORIZACIÓN
// ===========================
builder.Services.AddAuthorization();
// ===========================
// CONFIGURACIÓN CORS
// Permite que el frontend Angular
// pueda llamar a esta API desde otro dominio
builder.Services.AddCors(options => 
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",
            "http://127.0.0.1:4200") // Permite solicitudes desde el frontend Angular en localhost
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>// Configura Swagger para incluir la seguridad JWT en la documentación de la API
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme// Define el esquema de seguridad para JWT Bearer
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Escribe: Bearer {tu token}"
    });
    

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement// Agrega el requisito de seguridad para que Swagger sepa que las rutas protegidas requieren un token JWT
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("FrontendPolicy"); // Aplica la política CORS definida para permitir solicitudes desde el frontend Angular

//app.UseMiddleware<ApiKeyMiddleware>(); // Agrega el middleware de validación de API key para rutas públicas (si es necesario)

app.UseAuthentication();


app.UseAuthorization();

app.MapControllers();

app.Run();