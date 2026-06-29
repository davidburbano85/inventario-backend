using Dapper;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Dominio.Entidades;
using System.Data;

namespace inventarioWebAI.Infraestructura.RepositoriosDapper;

public class ComprasRepositorioDapper : IComprasRepositorio
{
    public async Task<Guid> CrearCompraAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Compra compra)
    {
        var sql = @"
            INSERT INTO compras
            (
                empresa_id,
                proveedor_id,
                total,
                created_at
            )
            VALUES
            (
                @EmpresaId,
                @ProveedorId,
                @Total,
                timezone('America/Bogota', now())
            )
            RETURNING id;";

        return await connection.ExecuteScalarAsync<Guid>(
            sql,
            new
            {
                compra.EmpresaId,
                compra.ProveedorId,
                compra.Total
            },
            transaction
        );
    }

    public async Task<IEnumerable<Compra>> ObtenerPorEmpresaAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId)
    {
        var sql = @"
            SELECT
                id,
                empresa_id,
                proveedor_id,
                total,
                created_at,
                updated_at,
                activo
            FROM compras
            WHERE empresa_id = @EmpresaId
              AND activo = true
            ORDER BY created_at DESC;";

        return await connection.QueryAsync<Compra>(
            sql,
            new { EmpresaId = empresaId },
            transaction
        );
    }

    public async Task<Compra?> ObtenerPorIdAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid id,
        Guid empresaId)
    {
        var sql = @"
            SELECT
                id,
                empresa_id,
                proveedor_id,
                total,
                created_at,
                updated_at,
                activo
            FROM compras
            WHERE id = @Id
              AND empresa_id = @EmpresaId
              AND activo = true;";

        return await connection.QueryFirstOrDefaultAsync<Compra>(
            sql,
            new
            {
                Id = id,
                EmpresaId = empresaId
            },
            transaction
        );
    }

    public async Task<bool> AnularCompraAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid id,
        Guid empresaId)
    {
        var sql = @"
            UPDATE compras
            SET activo = false,
                updated_at = timezone('America/Bogota', now())
            WHERE id = @Id
              AND empresa_id = @EmpresaId
              AND activo = true;";

        var rows = await connection.ExecuteAsync(
            sql,
            new
            {
                Id = id,
                EmpresaId = empresaId
            },
            transaction
        );

        return rows > 0;
    }

    public async Task<IEnumerable<CompraDetalle>> ObtenerPorCompraAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    Guid compraId,
    Guid empresaId)
    {
        var sql = @"
        SELECT
            id,
            empresa_id,
            compra_id,
            producto_id,
            cantidad,
            precio,
            created_at,
            updated_at,
            activo
        FROM compras_detalle
        WHERE compra_id = @CompraId
          AND empresa_id = @EmpresaId
          AND activo = true;";

        return await connection.QueryAsync<CompraDetalle>(
            sql,
            new
            {
                CompraId = compraId,
                EmpresaId = empresaId
            },
            transaction
        );
    }
}