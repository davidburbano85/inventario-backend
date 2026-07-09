using Dapper;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Dominio.Entidades;
using System.Data;
using inventarioWebAI.Infraestructura.Mapper.MapperDominio;
using inventarioWebAI.Dominio.Enums;

namespace inventarioWebAI.Infraestructura.RepositoriosDapper;

public class MovimientosInventarioRepositorioDapper : IMovimientosInventarioRepositorio
{
    public async Task<Guid> InsertarAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        MovimientoInventario movimiento)
    {
        var sql = @"
            INSERT INTO movimientos_inventario
            (
                empresa_id,
                producto_id,
                almacen_id,
                usuario_id,
                tipo,
                cantidad,
                motivo,
                factura,
                created_at
            )
            VALUES
            (
                @EmpresaId,
                @ProductoId,
                @AlmacenId,
                @UsuarioId,
                @Tipo,
                @Cantidad,
                @Motivo,
                @Factura,
                timezone('America/Bogota', now())
            )
            RETURNING id;";

        return await connection.ExecuteScalarAsync<Guid>(
            sql,
            new
            {
                movimiento.EmpresaId,
                movimiento.ProductoId,
                movimiento.AlmacenId,
                movimiento.UsuarioId,

                // CRÍTICO:
                // Se usa mapper de dominio para garantizar consistencia BD ↔ dominio
                Tipo = TipoMovimientoInventarioMapper.ToDb(movimiento.Tipo),

                movimiento.Cantidad,
                movimiento.Motivo
            },
            transaction
        );
    }

    public async Task<IEnumerable<MovimientoInventario>> ObtenerPorEmpresaAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId)
    {
        var sql = @"
            SELECT
                id,
                empresa_id,
                producto_id,
                almacen_id,
                usuario_id,
                tipo,
                cantidad,
                motivo,
                created_at,
                updated_at
            FROM movimientos_inventario
            WHERE empresa_id = @EmpresaId
              AND activo = true
            ORDER BY created_at DESC;";

        var result = await connection.QueryAsync<dynamic>(
            sql,
            new { EmpresaId = empresaId },
            transaction
        );

        return result.Select(MapToDomain);
    }

    public async Task<IEnumerable<MovimientoInventario>> ObtenerPorProductoAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId,
        Guid productoId)
    {
        var sql = @"
            SELECT *
            FROM movimientos_inventario
            WHERE empresa_id = @EmpresaId
              AND producto_id = @ProductoId
              AND activo = true
            ORDER BY created_at DESC;";

        var result = await connection.QueryAsync<dynamic>(
            sql,
            new { EmpresaId = empresaId, ProductoId = productoId },
            transaction
        );

        return result.Select(MapToDomain);
    }

    public async Task<IEnumerable<MovimientoInventario>> ObtenerPorAlmacenAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId,
        Guid almacenId)
    {
        var sql = @"
            SELECT *
            FROM movimientos_inventario
            WHERE empresa_id = @EmpresaId
              AND almacen_id = @AlmacenId
              AND activo = true
            ORDER BY created_at DESC;";

        var result = await connection.QueryAsync<dynamic>(
            sql,
            new { EmpresaId = empresaId, AlmacenId = almacenId },
            transaction
        );

        return result.Select(MapToDomain);
    }

    public async Task<IEnumerable<MovimientoInventario>> FiltrarAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId,
        Guid? productoId = null,
        Guid? almacenId = null,
        string? tipo = null)
    {
        var sql = @"
            SELECT *
            FROM movimientos_inventario
            WHERE empresa_id = @EmpresaId
              AND (@ProductoId IS NULL OR producto_id = @ProductoId)
              AND (@AlmacenId IS NULL OR almacen_id = @AlmacenId)
              AND (@Tipo IS NULL OR tipo = @Tipo)
              AND activo = true
            ORDER BY created_at DESC;";

        var result = await connection.QueryAsync<dynamic>(
            sql,
            new
            {
                EmpresaId = empresaId,
                ProductoId = productoId,
                AlmacenId = almacenId,
                Tipo = tipo
            },
            transaction
        );

        return result.Select(MapToDomain);
    }

    // =========================================================
    // CRÍTICO: Mapper manual de BD → Domain
    // Evita acoplar Dapper directamente al dominio
    // =========================================================
    private static MovimientoInventario MapToDomain(dynamic x)
    {
        return new MovimientoInventario
        {
            Id = x.id,
            EmpresaId = x.empresa_id,
            ProductoId = x.producto_id,
            AlmacenId = x.almacen_id,
            UsuarioId = x.usuario_id,
            Tipo = TipoMovimientoInventarioMapper.ToDomain(x.tipo),
            Cantidad = x.cantidad,
            Motivo = x.motivo,
            Factura = x.factura,
            CreatedAt = x.created_at,
            UpdatedAt = x.updated_at
        };
    }
    public async Task<MovimientoInventario?> EncontrarPorFacturaAsync(
      IDbConnection connection,
      IDbTransaction transaction,
      Guid empresaId,
      string factura)
    {
        var sql = @"
        SELECT *
        FROM movimientos_inventario
        WHERE empresa_id = @EmpresaId
          AND factura = @Factura
          AND activo = true
        LIMIT 1;";


        var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
            sql,
            new
            {
                EmpresaId = empresaId,
                Factura = factura
            },
            transaction
        );


        return result == null ? null : MapToDomain(result);
    }

}