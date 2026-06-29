using Dapper;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using System.Data;

namespace inventarioWebAI.Infraestructura.RepositoriosDapper;

public class StockRepositorioDapper : IStockRepositorio
{
    public async Task<decimal?> ObtenerCantidadAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId,
        Guid productoId,
        Guid almacenId)
    {
        var sql = @"
            SELECT cantidad
            FROM stock_actual
            WHERE empresa_id = @EmpresaId
              AND producto_id = @ProductoId
              AND almacen_id = @AlmacenId
              AND activo = true;";

        return await connection.ExecuteScalarAsync<decimal?>(
            sql,
            new
            {
                EmpresaId = empresaId,
                ProductoId = productoId,
                AlmacenId = almacenId,

            },
            transaction
        );
    }

    public async Task CrearRegistroAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId,
        Guid productoId,
        Guid almacenId,
        decimal cantidadInicial)
    {
        var sql = @"
            INSERT INTO stock_actual
            (
                empresa_id,
                producto_id,
                almacen_id,
                cantidad,
                updated_at
            )
            VALUES
            (
                @EmpresaId,
                @ProductoId,
                @AlmacenId,
                @Cantidad,
                timezone('America/Bogota', now())
            );";

        await connection.ExecuteAsync(
            sql,
            new
            {
                EmpresaId = empresaId,
                ProductoId = productoId,
                AlmacenId = almacenId,
                Cantidad = cantidadInicial
            },
            transaction
        );
    }

    public async Task IncrementarStockAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId,
        Guid productoId,
        Guid almacenId,
        decimal cantidad)
    {
        var sql = @"
            UPDATE stock_actual
            SET
                cantidad = cantidad + @Cantidad,
                updated_at = timezone('America/Bogota', now())
            WHERE empresa_id = @EmpresaId
              AND producto_id = @ProductoId
              AND almacen_id = @AlmacenId
              AND activo = true;";

        await connection.ExecuteAsync(
            sql,
            new
            {
                EmpresaId = empresaId,
                ProductoId = productoId,
                AlmacenId = almacenId,
                Cantidad = cantidad
            },
            transaction
        );
    }

    public async Task DisminuirStockAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId,
        Guid productoId,
        Guid almacenId,
        decimal cantidad)
    {
        var sql = @"
            UPDATE stock_actual
            SET
                cantidad = cantidad - @Cantidad,
                updated_at = timezone('America/Bogota', now())
            WHERE empresa_id = @EmpresaId
              AND producto_id = @ProductoId
              AND almacen_id = @AlmacenId
              AND activo = true;";

        await connection.ExecuteAsync(
            sql,
            new
            {
                EmpresaId = empresaId,
                ProductoId = productoId,
                AlmacenId = almacenId,
                Cantidad = cantidad
            },
            transaction
        );
    }
}