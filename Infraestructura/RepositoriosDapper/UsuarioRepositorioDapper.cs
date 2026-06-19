using Dapper;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Dominio.Entidades;
using inventarioWebAI.Infraestructura.AccesoDatos;
using inventarioWebAI.Infraestructura.Conexion;
using static System.Net.WebRequestMethods;

namespace inventarioWebAI.Infraestructura.RepositoriosDapper
{
    public class UsuarioRepositorioDapper : IUsuarioRepositorio
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UsuarioRepositorioDapper> _logger;
        public UsuarioRepositorioDapper(IDbConnectionFactory dbConnectionFactory,
                                        IConfiguration config,
                                        IHttpClientFactory httpClientFactory,
                                        ILogger<UsuarioRepositorioDapper> logger    )
        {
            _dbConnectionFactory = dbConnectionFactory;
            _config = config;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        public async Task<int> CrearAsync(Usuario usuario)
        {
            using var conexion = _dbConnectionFactory.CrearConexion();

            const string sql = @"
            INSERT INTO usuarios
                    (auth_user_id, nombre, telefono, created_at)
                VALUES
                    (@AuthUserId, @Nombre, @Telefono, timezone('America/Bogota', now()))
                RETURNING id;
            ";

            var id = await conexion.ExecuteScalarAsync<int>(
                sql,
                usuario
               
            );

            return id;
        }


        public async Task<Usuario?> ObtenerPorIdAsync(Guid idUsuario)
        {
            const string sql = @"
                SELECT
                        id AS Id,
                        nombre AS Nombre,
                        telefono AS Telefono,
                        created_at AS CreatedAt
                    FROM usuarios
                    WHERE id = @idUsuario;
                ";

            try
            {
                _logger.LogInformation(
                    "Abriendo conexión para usuario {IdUsuario}",
                    idUsuario);

                using var conexion = _dbConnectionFactory.CrearConexion();

                var inicio = DateTime.UtcNow;

                var usuario = await conexion.QueryFirstOrDefaultAsync<Usuario>(
                    sql,
                    new { idUsuario }
                );

                var duracion = DateTime.UtcNow - inicio;

                _logger.LogInformation(
                    "Consulta finalizada en {Milisegundos} ms para {IdUsuario}",
                    duracion.TotalMilliseconds,
                    idUsuario);

                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error consultando usuario {IdUsuario}",
                    idUsuario);

                throw;
            }
        }
        public async Task<bool> ExisteTelefonoAsync(string telefono)
        {
            using var conexion = _dbConnectionFactory.CrearConexion();
            string sql = @"SELECT COUNT(1) FROM Usuarios WHERE telefono = @Telefono";
            var cantidad = await conexion.ExecuteScalarAsync<int>(sql, new { telefono });
            return cantidad > 0;
        }

        public Task<IEnumerable<Usuario>> ListarAsync()
        {
            var conexion = _dbConnectionFactory.CrearConexion();
            string sql = @"SELECT
                        id AS Id,
                        nombre AS Nombre,
                        telefono AS Telefono, 
                        created_at AS CreatedAt
                         FROM Usuarios";
            return conexion.QueryAsync<Usuario>(sql);
        }

        public async Task<Usuario?> ObtenerPorAuthUserIdAsync(Guid authUserId)
        {
            using var conexion = _dbConnectionFactory.CrearConexion();
            var sql = @"SELECT
                        id AS Id,
                        nombre AS Nombre,
                        telefono AS Telefono, 
                        created_at AS CreatedAt
                         FROM Usuarios
                         WHERE id = @Id";

            return await conexion.QueryFirstOrDefaultAsync<Usuario>(sql, new { Id = authUserId });
        }

        public async Task<Usuario?> ObtenerPorTelefonoAsync(string telefono)
        {
            using var conexion = _dbConnectionFactory.CrearConexion();
            string sql = @" 
              
                SELECT 
                        id AS Id,
                        nombre AS Nombre, 
                        telefono AS Telefono,
                        created_at AS CreatedAt
                        FROM usuario
                        WHERE telefono=@Telefono         
                ";
            return await conexion.QueryFirstOrDefaultAsync<Usuario>(sql, new { telefono });
            // queryFirstOrDefaultAsync 
        }

        public async Task<bool> ActualizarAsync(Usuario usuario)
        {
            using var conexion = _dbConnectionFactory.CrearConexion();
            string sql = @"
                UPDATE usuario
                SET
                    nombre = @Nombre,
                    telefono =@Telefono,
                    ceated_at = @CreatedAt
                WHERE id=@Id
            ";
            var filas = await conexion.ExecuteAsync(sql, usuario);
            return filas > 0;
        }

        public async Task<bool> EliminarAsync(Guid idUsuario)
        {

            var supabaseUrl = "https://egqgezxlgaajfrxmwvih.supabase.co";
            var url = $"{supabaseUrl}/auth/v1/admin/users/{idUsuario}";
            var serviceRoleKey = Environment.GetEnvironmentVariable("SUPABASE_SERVICE_ROLE_KEY");
            if (string.IsNullOrWhiteSpace(serviceRoleKey))
            {
                throw new InvalidOperationException("La variable de entorno SUPABASE_SERVICE_ROLE_KEY no está configurada.");
            }
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            request.Headers.Add("apikey", serviceRoleKey);
            request.Headers.Add("Authorization", $"Bearer {serviceRoleKey}");
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> ActualizarPorAuthAsync(Guid authId, Usuario usuario)
        {
            using var conexion = _dbConnectionFactory.CrearConexion();
            string sql = @"
                UPDATE usuarios
                SET
                    nombre = @Nombre,
                    telefono =@Telefono,
                    created_at = @CreatedAt
                WHERE id=@Id
            ";
            var filas = await conexion.ExecuteAsync(sql, new { Nombre = usuario.Nombre, Telefono = usuario.Telefono, CreatedAt = DateTime.UtcNow, Id = authId });
            return filas > 0;
        }
    }
}
