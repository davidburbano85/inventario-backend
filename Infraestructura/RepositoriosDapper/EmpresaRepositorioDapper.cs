


using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Dominio.Entidades;
using System.Data.Common;
using Dapper;
using inventarioWebAI.Infraestructura.Conexion;
using static System.Runtime.InteropServices.JavaScript.JSType;
using inventarioWebAI.Infraestructura.AccesoDatos;

namespace inventarioWebAI.Infraestructura.RepositoriosDapper
{


    public class EmpresaRepositorioDapper : IEmpresaRepositorio
    {
        private readonly IDbConnectionFactory _db;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public EmpresaRepositorioDapper(IDbConnectionFactory db,
                                        IConfiguration config,
                                        IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _config = config;
            _httpClientFactory = httpClientFactory;
        }


        public async Task<Guid> CrearEmpresaAsync(Empresa empresa)
        {
            using var connection = _db.CrearConexion();

            var sql = @"
            INSERT INTO empresas (nombre)
            VALUES (@Nombre)
            RETURNING id; ";

            return await connection.ExecuteScalarAsync<Guid>(sql, empresa);
        }

        public async Task<Empresa?> ObtenerEmpresaPorIdAsync(Guid empresaId)
        {
            using var connection = _db.CrearConexion();
            var sql = @"
                SELECT
                    id,
                    nombre,
                    created_at AS CreatedAt
                FROM empresas
                WHERE id = @EmpresaId;
            ";
            return await connection.QueryFirstOrDefaultAsync<Empresa>(sql, new
            {
                EmpresaId = empresaId
            });
        }


        public async Task<IEnumerable<Empresa>> ObtenerEmpresaPorUsuarioAsync(Guid usuarioId)
        {
            using var connection = _db.CrearConexion();

            var sql = @"
                SELECT
                    e.id,
                    e.nombre,
                    e.created_at AS CreatedAt
                FROM empresas e
                INNER JOIN usuarios_empresas ue ON ue.empresa_id = e.id
                WHERE ue.usuario_id = @UsuarioId;
            ";

            return await connection.QueryAsync<Empresa>(sql, new
            {
                UsuarioId = usuarioId
            });
        }

        public async Task<Empresa> ActualizarEmpresaAsync( Empresa empresa)
        {
            using var connection = _db.CrearConexion();

            var sql = @"
                UPDATE empresas
                SET nombre = @Nombre,
                    updated_at = now()
                WHERE id = @Id
                RETURNING id, nombre, updated_at;
            ";

            return await connection.QueryFirstAsync<Empresa>(sql, empresa);
        }

        public async Task<Empresa> EliminarEmpresaAsync(Guid empresaId)
        {
           using var connection = _db.CrearConexion();
            var sql = @"
                DELETE FROM empresas
                WHERE id = @EmpresaId
                RETURNING id, nombre, updated_at;
            ";
            return await connection.QueryFirstAsync<Empresa>(sql, new
            {
                EmpresaId = empresaId

            });
        }

       
    
    
    
    }
}
