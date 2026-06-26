using Dapper;
using inventarioWebAI.Aplicacion.DTOs.Almacen;
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

       

        public async Task<IEnumerable< Almacen?>> ObtenerAlmacenesActivosPorEmpresaAsync(Guid empresaId)
        {
            Console.WriteLine("[ObtenerAlmacenActivoPorEmpresaAsync] INICIO");

            try
            {
                using var conn = _db.CrearConexion();

                var sql = @"
                SELECT
                    id,
                    empresa_id,
                    nombre,
                    ubicacion,
                    created_at,
                    updated_at,
                    activo
                FROM almacenes
                WHERE empresa_id = @EmpresaId
                  AND activo = true
                ;";

                return await conn.QueryAsync<Almacen>(
                     sql,
                     new
                     {
                         EmpresaId = empresaId
                     });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ObtenerAlmacenActivoPorEmpresaAsync] ERROR: {ex.GetType().Name} - {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public async Task<bool> ActualizarAlmacenAsync(Almacen almacen)
        {
            Console.WriteLine("[ActualizarAlmacenAsync] INICIO");

            try
            {
                using var conn = _db.CrearConexion();

                var sql = @"
                    UPDATE almacenes
                    SET
                        nombre = @Nombre,
                        ubicacion = @Ubicacion,
                        updated_at = timezone('America/Bogota', now())
                    WHERE id = @Id
                      AND empresa_id = @EmpresaId
                      AND activo = true
                    RETURNING id;";

                var result = await conn.ExecuteScalarAsync<Guid?>(
                    sql,
                    new
                    {
                        almacen.Id,
                        almacen.EmpresaId,
                        almacen.Nombre,
                        almacen.Ubicacion
                    });

                var actualizado = result.HasValue;

                Console.WriteLine($"[ActualizarAlmacenAsync] RESULTADO: {actualizado}");

                return actualizado;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"[ActualizarAlmacenAsync] ERROR: {ex.GetType().Name} - {ex.Message}");

                Console.WriteLine(ex.StackTrace);

                throw;
            }
        }


        public async Task<Almacen?> ObtenerAlmacenPorIdAsync(Guid id, Guid empresaId)
        {
            Console.WriteLine("[ObtenerAlmacenPorIdAsync] INICIO");

            try
            {
                using var conn = _db.CrearConexion();

                var sql = @"
            SELECT
                id,
                empresa_id,
                nombre,
                ubicacion,
                created_at,
                updated_at,
                activo
            FROM almacenes
            WHERE id = @Id
              AND empresa_id = @EmpresaId
                            ;";

                var almacen = await conn.QueryFirstOrDefaultAsync<Almacen>(
                    sql,
                    new
                    {
                        Id = id,
                        EmpresaId = empresaId
                    });

                Console.WriteLine("[ObtenerAlmacenPorIdAsync] RESULTADO OK");

                return almacen;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ObtenerAlmacenPorIdAsync] ERROR: {ex.GetType().Name} - {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public async Task<bool> EliminarAlmacenAsync(Guid id, Guid empresaId)
        {
            Console.WriteLine("[EliminarAlmacenAsync] INICIO");

            try
            {
                using var conn = _db.CrearConexion();

                var sql = @"
                DELETE FROM almacenes
                WHERE id = @Id
                  AND empresa_id = @EmpresaId;";

                var filas = await conn.ExecuteAsync(sql, new
                {
                    Id = id,
                    EmpresaId = empresaId
                });

                Console.WriteLine($"[EliminarAlmacenAsync] Filas afectadas: {filas}");

                return filas > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EliminarAlmacenAsync] ERROR: {ex.GetType().Name} - {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}

