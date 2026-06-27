// Ubicación: /src/Infraestructura/RepositoriosDapper/CompraDetalleRepositorioDapper.cs

using Dapper;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Dominio.Entidades;
using inventarioWebAI.Infraestructura.AccesoDatos;
using System.Data;

namespace inventarioWebAI.Infraestructura.RepositoriosDapper;

public class CompraDetalleRepositorioDapper : ICompraDetalleRepositorio
{
    private readonly IDbConnectionFactory _db;

    public CompraDetalleRepositorioDapper(IDbConnectionFactory db)
    {
        _db = db;
    }

    public async Task InsertarAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid compraId,
        Guid empresaId,
        IEnumerable<CompraDetalle> detalles)
    {
        var sql = @"
            INSERT INTO compras_detalle
            (
                empresa_id,
                compra_id,
                producto_id,
                cantidad,
                precio,
                created_at
            )
            VALUES
            (
                @EmpresaId,
                @CompraId,
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
                CompraId = compraId,
                d.ProductoId,
                d.Cantidad,
                d.Precio
            }, transaction);
        }
    }

    public async Task<IEnumerable<CompraDetalle>> ObtenerPorCompraAsync(
        Guid compraId,
        Guid empresaId)
    {
        using var conn = _db.CrearConexion();

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

        return await conn.QueryAsync<CompraDetalle>(sql, new
        {
            CompraId = compraId,
            EmpresaId = empresaId
        });
    }
}