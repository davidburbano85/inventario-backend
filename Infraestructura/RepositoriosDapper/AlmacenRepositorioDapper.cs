using Dapper;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Dominio.Entidades;
using inventarioWebAI.Dominio.Enums;
using inventarioWebAI.Infraestructura.AccesoDatos;
using inventarioWebAI.Infraestructura.Mapper;

namespace inventarioWebAI.Infraestructura.RepositoriosDapper
{
    public class AlmacenRepositorioDapper: IAlmacenRepositorio
    {
            private readonly IDbConnectionFactory _db;
            private readonly IConfiguration _config;
            private readonly IHttpClientFactory _httpClientFactory;
    
            public AlmacenRepositorioDapper(IDbConnectionFactory db,
                                            IConfiguration config,
                                            IHttpClientFactory httpClientFactory)
            {
                _db = db;
                _config = config;
                _httpClientFactory = httpClientFactory;
            }

        public async Task<Guid> CrearAlmacenAsync(Almacen almacen)
        {
            Console.WriteLine("[CrearAlmacenAsync] INICIO");

            try
            {
                using var conn = _db.CrearConexion();

                var sql = @"
                    INSERT INTO almacenes
                    (
                        empresa_id,
                        nombre,
                        ubicacion,
                        created_at
                    )
                    VALUES
                    (
                        @EmpresaId,
                        @Nombre,
                        @Ubicacion,
                        timezone('America/Bogota', now())
                    )
                    RETURNING id;";

                var result = await conn.ExecuteScalarAsync<Guid>(
                    sql,
                    new
                    {
                        almacen.EmpresaId,
                        almacen.Nombre,
                        almacen.Ubicacion
                    });

                Console.WriteLine($"[CrearAlmacenAsync] RESULTADO: {result}");

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"[CrearAlmacenAsync] ERROR: {ex.GetType().Name} - {ex.Message}");

                Console.WriteLine(ex.StackTrace);

                throw;
            }
        }

        public async Task<IEnumerable<Almacen>> ObtenerAlmacenPorEmpresaIdAsync(Guid empresaId)
        {
            try
            {
                using var connection = _db.CrearConexion();

                var sql = @"
            SELECT
                id,
                empresa_id AS EmpresaId,
                nombre,
                ubicacion,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM almacenes
            WHERE empresa_id = @EmpresaId;
        ";

                var almacenes = await connection.QueryAsync<Almacen>(sql, new
                {
                    EmpresaId = empresaId
                });

                return almacenes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ALMACEN] ObtenerPorEmpresa ERROR: {ex.GetType().Name} - {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }





        public Task<IEnumerable<Almacen>> ObtenerPorEmpresaAsync(Guid empresaId)
        {
            throw new NotImplementedException();
        }


    }
}

