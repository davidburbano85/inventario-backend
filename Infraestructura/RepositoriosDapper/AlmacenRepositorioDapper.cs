using Dapper;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Dominio.Entidades;
using inventarioWebAI.Infraestructura.AccesoDatos;
using System.Data;

namespace inventarioWebAI.Infraestructura.RepositoriosDapper;

public class AlmacenRepositorioDapper : IAlmacenRepositorio
{
    private readonly IDbConnectionFactory _db;

    public AlmacenRepositorioDapper(IDbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<Guid> CrearAlmacenAsync(Almacen almacen)
    {
        using var connection = _db.CrearConexion();

        var sql = @"
            INSERT INTO almacenes (empresa_id, nombre, ubicacion, activo)
            VALUES (@EmpresaId, @Nombre, @Ubicacion, true)
            RETURNING id;";

        return await connection.ExecuteScalarAsync<Guid>(sql, almacen);
    }


    public async Task<IEnumerable<Almacen>> ObtenerAlmacenesActivosPorEmpresaAsync(Guid empresaId)
    {
        using var connection = _db.CrearConexion();

        var sql = @"
        SELECT
            id          AS Id,
            empresa_id  AS EmpresaId,
            nombre      AS Nombre,
            activo      AS Activo,
            ubicacion   AS Ubicacion,
            created_at  AS CreatedAt,
            updated_at  AS UpdatedAt
        FROM almacenes
        WHERE empresa_id = @EmpresaId
          AND activo = true;";

        return await connection.QueryAsync<Almacen>(
            sql,
            new { EmpresaId = empresaId }
        );
    }


    public async Task<bool> ActualizarAlmacenAsync(Almacen almacen)
    {
        using var connection = _db.CrearConexion();

        var sql = @"
            UPDATE almacenes
            SET nombre = @Nombre,
                ubicacion = @Ubicacion,
                updated_at = timezone('America/Bogota', now())
            WHERE id = @Id
              AND empresa_id = @EmpresaId;";

        var rows = await connection.ExecuteAsync(sql, almacen);
        return rows > 0;
    }

    public async Task<Almacen?> ObtenerAlmacenPorIdAsync(Guid id, Guid empresaId)
    {
        using var connection = _db.CrearConexion();

        var sql = @"
        SELECT
            id          AS Id,
            empresa_id  AS EmpresaId,
            nombre      AS Nombre,
            activo      AS Activo,
            ubicacion   AS Ubicacion,
            created_at  AS CreatedAt,
            updated_at  AS UpdatedAt
        FROM almacenes
        WHERE id = @Id
          AND empresa_id = @EmpresaId
          AND activo = true;";

        return await connection.QueryFirstOrDefaultAsync<Almacen>(
            sql,
            new { Id = id, EmpresaId = empresaId }
        );
    }
    public async Task<bool> EliminarAlmacenAsync(Guid id, Guid empresaId)
    {
        using var connection = _db.CrearConexion();

        var sql = @"
            UPDATE almacenes
            SET activo = false
            WHERE id = @Id
              AND empresa_id = @EmpresaId;";

        var rows = await connection.ExecuteAsync(sql, new { Id = id, EmpresaId = empresaId });
        return rows > 0;
    }
}