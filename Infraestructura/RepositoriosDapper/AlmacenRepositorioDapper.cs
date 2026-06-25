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

        //public async Task<Almacen?> ObtenerAlmacenPorIdAsync(Guid id, Guid empresaId)
        //{
        //    Console.WriteLine("[ObtenerAlmacenPorIdAsync] INICIO");

        //    try
        //    {
        //        using var conn = _db.CrearConexion();

        //        var sql = @"
        //            SELECT 
        //                id,
        //                empresa_id,
        //                nombre,
        //                ubicacion,
        //                created_at,
        //                updated_at,
        //                activo
        //            FROM almacenes
        //            WHERE id = @Id
        //              AND empresa_id = @EmpresaId
        //              AND activo = true;";

        //        var row = await conn.QueryFirstOrDefaultAsync<Almacen>(
        //            sql,
        //            new
        //            {
        //                Id = id,
        //                EmpresaId = empresaId
        //            });

        //        return row;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"[ObtenerAlmacenPorIdAsync] ERROR: {ex.GetType().Name} - {ex.Message}");
        //        Console.WriteLine(ex.StackTrace);
        //        throw;
        //    }
        //}

        public async Task<Almacen?> ObtenerAlmacenActivoPorEmpresaAsync(Guid empresaId)
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
                LIMIT 1;";

                var almacen = await conn.QueryFirstOrDefaultAsync<Almacen>(
                    sql,
                    new
                    {
                        EmpresaId = empresaId
                    });

                return almacen;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ObtenerAlmacenActivoPorEmpresaAsync] ERROR: {ex.GetType().Name} - {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }




    }
}

