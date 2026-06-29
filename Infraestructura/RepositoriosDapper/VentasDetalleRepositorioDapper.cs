using Dapper;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Dominio.Entidades;
using System.Data;

namespace inventarioWebAI.Infraestructura.RepositoriosDapper;

public class VentasDetalleRepositorioDapper : IVentasDetalleRepositorio
{
    public async Task InsertarAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid ventaId,
        Guid empresaId,
        IEnumerable<VentaDetalle> detalles)
    {
        var sql = @"
            INSERT INTO ventas_detalle
            (
                empresa_id,
                venta_id,
                producto_id,
                cantidad,
                precio,
                created_at
            )
            VALUES
            (
                @EmpresaId,
                @VentaId,
                @ProductoId,
                @Cantidad,
                @Precio,
                timezone('America/Bogota', now())
            );";

        foreach (var d in detalles)
        {
            await connection.ExecuteAsync(sql, new
            {
                EmpresaId = empresaId,
                VentaId = ventaId,
                d.ProductoId,
                d.Cantidad,
                d.Precio
            }, transaction);
        }
    }

    public async Task<IEnumerable<VentaDetalle>> ObtenerPorVentaAsync(
      IDbConnection connection,
      IDbTransaction transaction,
      Guid ventaId,
      Guid empresaId)
    {
        var sql = @"
        SELECT
            id,
            empresa_id        AS EmpresaId,
            venta_id          AS VentaId,
            producto_id       AS ProductoId,
            cantidad          AS Cantidad,
            precio            AS Precio,
            created_at        AS CreatedAt,
            updated_at        AS UpdatedAt,
            activo            AS Activo
        FROM ventas_detalle
        WHERE venta_id = @VentaId
          AND empresa_id = @EmpresaId
          AND activo = true;";

        var result = await connection.QueryAsync<VentaDetalle>(
            sql,
            new
            {
                VentaId = ventaId,
                EmpresaId = empresaId
            },
            transaction
        );

        return result;
    }


}