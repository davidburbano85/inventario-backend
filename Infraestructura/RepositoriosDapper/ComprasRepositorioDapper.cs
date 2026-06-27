using Dapper;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Dominio.Entidades;
using inventarioWebAI.Infraestructura.AccesoDatos;

namespace inventarioWebAI.Infraestructura.RepositoriosDapper;

public class ComprasRepositorioDapper : IComprasRepositorio
{
    private readonly IDbConnectionFactory _db;

    public ComprasRepositorioDapper(IDbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<Guid> CrearCompraAsync(Compra compra)
    {
        using var conn = _db.CrearConexion();

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

        return await conn.ExecuteScalarAsync<Guid>(sql, compra);
    }

    public async Task<IEnumerable<Compra?>> ObtenerPorEmpresaAsync(Guid empresaId)
    {
        using var conn = _db.CrearConexion();

        var sql = @"
            SELECT *
            FROM compras
            WHERE empresa_id = @EmpresaId
              AND activo = true
            ORDER BY created_at DESC;";

        return await conn.QueryAsync<Compra>(sql, new { EmpresaId = empresaId });
    }

    public async Task<Compra?> ObtenerPorIdAsync(Guid id, Guid empresaId)
    {
        using var conn = _db.CrearConexion();

        var sql = @"
            SELECT *
            FROM compras
            WHERE id = @Id
              AND empresa_id = @EmpresaId
              AND activo = true;";

        return await conn.QueryFirstOrDefaultAsync<Compra>(sql, new
        {
            Id = id,
            EmpresaId = empresaId
        });
    }

    public async Task<bool> AnularCompraAsync(Guid id, Guid empresaId)
    {
        using var conn = _db.CrearConexion();

        var sql = @"
            UPDATE compras
            SET activo = false,
                updated_at = timezone('America/Bogota', now())
            WHERE id = @Id
              AND empresa_id = @EmpresaId
              AND activo = true;";

        var rows = await conn.ExecuteAsync(sql, new
        {
            Id = id,
            EmpresaId = empresaId
        });

        return rows > 0;
    }
}