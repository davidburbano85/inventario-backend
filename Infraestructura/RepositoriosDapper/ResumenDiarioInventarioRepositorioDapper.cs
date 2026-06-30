using System.Data;
using Dapper;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Infraestructura.RepositoriosDapper
{
    public class ResumenDiarioInventarioRepositorioDapper : IResumenDiarioInventarioRepositorio
    {
        public async Task<int> GenerarResumenDiarioAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            Guid empresaId,
            Guid almacenId,
            DateTime fecha)
        {
            var sql = @"
                INSERT INTO resumen_diario_inventario
                (
                    empresa_id,
                    almacen_id,
                    producto_id,
                    fecha,
                    total_ventas,
                    total_compras,
                    created_at,
                    updated_at,
                    activo
                )
                SELECT
                    m.empresa_id,
                    m.almacen_id,
                    m.producto_id,
                    DATE(m.created_at),
                    SUM(CASE WHEN m.tipo = 'salida' THEN m.cantidad ELSE 0 END),
                    SUM(CASE WHEN m.tipo = 'entrada' THEN m.cantidad ELSE 0 END),
                    timezone('America/Bogota', now()),
                    timezone('America/Bogota', now()),
                    true
                FROM movimientos_inventario m
                WHERE m.empresa_id = @EmpresaId
                  AND m.almacen_id = @AlmacenId
                  AND DATE(m.created_at) = @Fecha
                  AND m.activo = true
                GROUP BY m.empresa_id, m.almacen_id, m.producto_id, DATE(m.created_at)
                ON CONFLICT (empresa_id, almacen_id, producto_id, fecha)
                DO UPDATE SET
                    total_ventas = EXCLUDED.total_ventas,
                    total_compras = EXCLUDED.total_compras,
                    updated_at = timezone('America/Bogota', now());
            ";

            // batch por almacen + fecha usando movimientos como fuente única de verdad
            return await connection.ExecuteAsync(sql, new
            {
                EmpresaId = empresaId,
                AlmacenId = almacenId,
                Fecha = fecha
            }, transaction);
        }

        public async Task<int> GenerarResumenPorRangoAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            Guid empresaId,
            Guid almacenId,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            var sql = @"
                INSERT INTO resumen_diario_inventario
                (
                    empresa_id,
                    almacen_id,
                    producto_id,
                    fecha,
                    total_ventas,
                    total_compras,
                    created_at,
                    updated_at,
                    activo
                )
                SELECT
                    m.empresa_id,
                    m.almacen_id,
                    m.producto_id,
                    DATE(m.created_at),
                    SUM(CASE WHEN m.tipo = 'salida' THEN m.cantidad ELSE 0 END),
                    SUM(CASE WHEN m.tipo = 'entrada' THEN m.cantidad ELSE 0 END),
                    timezone('America/Bogota', now()),
                    timezone('America/Bogota', now()),
                    true
                FROM movimientos_inventario m
                WHERE m.empresa_id = @EmpresaId
                  AND m.almacen_id = @AlmacenId
                  AND DATE(m.created_at) BETWEEN @FechaInicio AND @FechaFin
                  AND m.activo = true
                GROUP BY m.empresa_id, m.almacen_id, m.producto_id, DATE(m.created_at)
                ON CONFLICT (empresa_id, almacen_id, producto_id, fecha)
                DO UPDATE SET
                    total_ventas = EXCLUDED.total_ventas,
                    total_compras = EXCLUDED.total_compras,
                    updated_at = timezone('America/Bogota', now());
            ";

            return await connection.ExecuteAsync(sql, new
            {
                EmpresaId = empresaId,
                AlmacenId = almacenId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            }, transaction);
        }

        public async Task<IEnumerable<ResumenDiarioInventario>> ObtenerPorRangoAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            Guid empresaId,
            Guid almacenId,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            var sql = @"
                SELECT
                    id AS Id,
                    empresa_id AS EmpresaId,
                    almacen_id AS AlmacenId,
                    producto_id AS ProductoId,
                    fecha AS Fecha,
                    total_ventas AS TotalVentas,
                    total_compras AS TotalCompras,
                    created_at AS CreatedAt,
                    updated_at AS UpdatedAt,
                    activo AS Activo
                FROM resumen_diario_inventario
                WHERE empresa_id = @EmpresaId
                  AND almacen_id = @AlmacenId
                  AND fecha BETWEEN @FechaInicio AND @FechaFin
                  AND activo = true
                ORDER BY fecha DESC;
            ";

            return await connection.QueryAsync<ResumenDiarioInventario>(sql, new
            {
                EmpresaId = empresaId,
                AlmacenId = almacenId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            }, transaction);
        }

        public async Task<ResumenDiarioInventario?> ObtenerPorProductoYFechaAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            Guid empresaId,
            Guid almacenId,
            Guid productoId,
            DateTime fecha)
        {
            var sql = @"
                SELECT
                    id AS Id,
                    empresa_id AS EmpresaId,
                    almacen_id AS AlmacenId,
                    producto_id AS ProductoId,
                    fecha AS Fecha,
                    total_ventas AS TotalVentas,
                    total_compras AS TotalCompras,
                    created_at AS CreatedAt,
                    updated_at AS UpdatedAt,
                    activo AS Activo
                FROM resumen_diario_inventario
                WHERE empresa_id = @EmpresaId
                  AND almacen_id = @AlmacenId
                  AND producto_id = @ProductoId
                  AND fecha = @Fecha
                  AND activo = true
                LIMIT 1;
            ";

            return await connection.QueryFirstOrDefaultAsync<ResumenDiarioInventario>(sql, new
            {
                EmpresaId = empresaId,
                AlmacenId = almacenId,
                ProductoId = productoId,
                Fecha = fecha
            }, transaction);
        }

        public async Task<bool> UpsertResumenAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            ResumenDiarioInventario resumen)
        {
            var sql = @"
                INSERT INTO resumen_diario_inventario
                (
                    empresa_id,
                    almacen_id,
                    producto_id,
                    fecha,
                    total_ventas,
                    total_compras,
                    created_at,
                    updated_at,
                    activo
                )
                VALUES
                (
                    @EmpresaId,
                    @AlmacenId,
                    @ProductoId,
                    @Fecha,
                    @TotalVentas,
                    @TotalCompras,
                    timezone('America/Bogota', now()),
                    timezone('America/Bogota', now()),
                    true
                )
                ON CONFLICT (empresa_id, almacen_id, producto_id, fecha)
                DO UPDATE SET
                    total_ventas = EXCLUDED.total_ventas,
                    total_compras = EXCLUDED.total_compras,
                    updated_at = timezone('America/Bogota', now());
            ";

            var rows = await connection.ExecuteAsync(sql, resumen, transaction);
            return rows > 0;
        }

        public async Task<bool> RecalcularProductoAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            Guid empresaId,
            Guid almacenId,
            Guid productoId,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            var sql = @"
                -- recalculo total desde movimientos_inventario (fuente única de verdad)
                DELETE FROM resumen_diario_inventario
                WHERE empresa_id = @EmpresaId
                  AND almacen_id = @AlmacenId
                  AND producto_id = @ProductoId
                  AND fecha BETWEEN @FechaInicio AND @FechaFin;

                INSERT INTO resumen_diario_inventario
                (
                    empresa_id,
                    almacen_id,
                    producto_id,
                    fecha,
                    total_ventas,
                    total_compras,
                    created_at,
                    updated_at,
                    activo
                )
                SELECT
                    m.empresa_id,
                    m.almacen_id,
                    m.producto_id,
                    DATE(m.created_at),
                    SUM(CASE WHEN m.tipo = 'salida' THEN m.cantidad ELSE 0 END),
                    SUM(CASE WHEN m.tipo = 'entrada' THEN m.cantidad ELSE 0 END),
                    timezone('America/Bogota', now()),
                    timezone('America/Bogota', now()),
                    true
                FROM movimientos_inventario m
                WHERE m.empresa_id = @EmpresaId
                  AND m.almacen_id = @AlmacenId
                  AND m.producto_id = @ProductoId
                  AND DATE(m.created_at) BETWEEN @FechaInicio AND @FechaFin
                  AND m.activo = true
                GROUP BY m.empresa_id, m.almacen_id, m.producto_id, DATE(m.created_at);
            ";

            await connection.ExecuteAsync(sql, new
            {
                EmpresaId = empresaId,
                AlmacenId = almacenId,
                ProductoId = productoId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            }, transaction);

            return true;
        }
    }
}