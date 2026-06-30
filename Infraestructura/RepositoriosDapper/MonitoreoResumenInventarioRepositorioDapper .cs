using System.Data;
using Dapper;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Infraestructura.RepositoriosDapper
{
    public class MonitoreoResumenInventarioRepositorioDapper
        : IMonitoreoResumenInventarioRepositorio
    {
        public async Task<Guid> CrearAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            MonitoreoResumenInventario entidad)
        {
            var sql = @"
                INSERT INTO monitoreo_resumen_inventario
                (
                    empresa_id,
                    almacen_id,
                    tipo_proceso,
                    fecha_desde,
                    fecha_hasta,
                    estado,
                    registros_procesados,
                    mensaje
                )
                VALUES
                (
                    @EmpresaId,
                    @AlmacenId,
                    @TipoProceso,
                    @FechaDesde,
                    @FechaHasta,
                    @Estado,
                    @RegistrosProcesados,
                    @Mensaje
                )
                RETURNING id;
            ";

            return await connection.ExecuteScalarAsync<Guid>(sql, entidad, transaction);
        }

        public async Task<bool> MarcarExitosoAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            Guid id,
            int registrosProcesados)
        {
            var sql = @"
                UPDATE monitoreo_resumen_inventario
                SET estado = 'exitoso',
                    registros_procesados = @RegistrosProcesados,
                    fecha_fin = timezone('America/Bogota', now())
                WHERE id = @Id;
            ";

            var rows = await connection.ExecuteAsync(sql, new { Id = id, RegistrosProcesados = registrosProcesados }, transaction);
            return rows > 0;
        }

        public async Task<bool> MarcarFallidoAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            Guid id,
            string mensaje)
        {
            var sql = @"
                UPDATE monitoreo_resumen_inventario
                SET estado = 'fallido',
                    mensaje = @Mensaje,
                    fecha_fin = timezone('America/Bogota', now())
                WHERE id = @Id;
            ";

            var rows = await connection.ExecuteAsync(sql, new { Id = id, Mensaje = mensaje }, transaction);
            return rows > 0;
        }

        public async Task<IEnumerable<MonitoreoResumenInventario>> ObtenerUltimosAsync(
            IDbConnection connection,
            int limite)
        {
            var sql = @"
                SELECT *
                FROM monitoreo_resumen_inventario
                ORDER BY creado_en DESC
                LIMIT @Limite;
            ";

            return await connection.QueryAsync<MonitoreoResumenInventario>(sql, new { Limite = limite });
        }

        public async Task<MonitoreoResumenInventario?> ObtenerUltimoFallidoAsync(
            IDbConnection connection)
        {
            var sql = @"
                SELECT *
                FROM monitoreo_resumen_inventario
                WHERE estado = 'fallido'
                ORDER BY creado_en DESC
                LIMIT 1;
            ";

            return await connection.QueryFirstOrDefaultAsync<MonitoreoResumenInventario>(sql);
        }
    }
}