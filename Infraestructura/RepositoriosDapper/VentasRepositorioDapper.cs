using Dapper;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Dominio.Entidades;
using System.Data;

namespace inventarioWebAI.Infraestructura.RepositoriosDapper;

public class VentasRepositorioDapper : IVentasRepositorio
{
    public async Task<Guid> CrearVentaAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Venta venta)
    {
        var sql = @"
            INSERT INTO ventas
            (
                empresa_id,
                cliente_id,
                total,
                created_at
            )
            VALUES
            (
                @EmpresaId,
                @ClienteId,
                @Total,
                timezone('America/Bogota', now())
            )
            RETURNING id;";

        return await connection.ExecuteScalarAsync<Guid>(
            sql,
            venta,
            transaction
        );
    }

    public async Task<IEnumerable<Venta>> ObtenerPorEmpresaAsync(
     IDbConnection connection,
     IDbTransaction transaction,
     Guid empresaId)
    {
        var sql = @"
        SELECT
            id          AS Id,
            empresa_id  AS EmpresaId,
            cliente_id  AS ClienteId,
            total       AS Total,
            created_at  AS CreatedAt,
            updated_at  AS UpdatedAt
        FROM ventas
        WHERE empresa_id = @EmpresaId
          AND activo = true
        ORDER BY created_at DESC;";

        return await connection.QueryAsync<Venta>(
            sql,
            new { EmpresaId = empresaId },
            transaction
        );
    }
    public async Task<Venta?> ObtenerPorIdAsync(
     IDbConnection connection,
     IDbTransaction transaction,
     Guid id,
     Guid empresaId)
    {
        var sqlVenta = @"
        SELECT
            id          AS Id,
            empresa_id  AS EmpresaId,
            cliente_id  AS ClienteId,
            total       AS Total,
            created_at  AS CreatedAt,
            updated_at  AS UpdatedAt
        FROM ventas
        WHERE id = @Id
          AND empresa_id = @EmpresaId
          AND activo = true;";

        var venta = await connection.QueryFirstOrDefaultAsync<Venta>(
            sqlVenta,
            new
            {
                Id = id,
                EmpresaId = empresaId
            },
            transaction
        );

        if (venta == null)
            return null;

        var sqlDetalles = @"
           SELECT
                id          AS Id,
                empresa_id  AS EmpresaId,
                venta_id    AS VentaId,
                producto_id AS ProductoId,
                cantidad    AS Cantidad,
                precio      AS Precio,
                created_at  AS CreatedAt,
                updated_at  AS UpdatedAt,
                activo      AS Activo
            FROM ventas_detalle
            WHERE venta_id = @VentaId
              AND empresa_id = @EmpresaId
              AND activo = true;";

        var detalles = await connection.QueryAsync<VentaDetalle>(
            sqlDetalles,
            new
            {
                VentaId = id,
                EmpresaId = empresaId
            },
            transaction
        );

        venta.Detalles = detalles.ToList();

        return venta;
    }
    public async Task<bool> AnularVentaAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid id,
        Guid empresaId)
    {
        var sql = @"
            UPDATE ventas
            SET activo = false,
                updated_at = timezone('America/Bogota', now())
            WHERE id = @Id
              AND empresa_id = @EmpresaId
              AND activo = true;";

        var rows = await connection.ExecuteAsync(
            sql,
            new { Id = id, EmpresaId = empresaId },
            transaction
        );

        return rows > 0;
    }
}