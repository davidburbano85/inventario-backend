using Dapper;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Dominio.Entidades;
using inventarioWebAI.Infraestructura.AccesoDatos;

namespace inventarioWebAI.Infraestructura.RepositoriosDapper;

public class ClienteRepositorioDapper : IClienteRepositorio
{
    private readonly IDbConnectionFactory _db;

    public ClienteRepositorioDapper(IDbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<Guid> CrearAsync(Cliente cliente)
    {
        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                INSERT INTO clientes
                (
                    empresa_id,
                    nombre,
                    contacto,
                    created_at
                )
                VALUES
                (
                    @EmpresaId,
                    @Nombre,
                    @Contacto,
                    timezone('America/Bogota', now())
                )
                RETURNING id;";

            return await conn.ExecuteScalarAsync<Guid>(sql, cliente);
        }
        catch
        {
            throw;
        }
    }

    public async Task<IEnumerable<Cliente?>> ObtenerPorEmpresaAsync(Guid empresaId)
    {
        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                SELECT
                    id,
                    empresa_id,
                    nombre,
                    contacto,
                    created_at,
                    updated_at,
                    activo
                FROM clientes
                WHERE empresa_id = @EmpresaId
                  AND activo = true;";

            return await conn.QueryAsync<Cliente>(sql, new { EmpresaId = empresaId });
        }
        catch
        {
            throw;
        }
    }

    public async Task<Cliente?> ObtenerPorIdAsync(Guid id, Guid empresaId)
    {
        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                SELECT
                    id,
                    empresa_id,
                    nombre,
                    contacto,
                    created_at,
                    updated_at,
                    activo
                FROM clientes
                WHERE id = @Id
                  AND empresa_id = @EmpresaId
                  AND activo = true;";

            return await conn.QueryFirstOrDefaultAsync<Cliente>(sql, new
            {
                Id = id,
                EmpresaId = empresaId
            });
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> ActualizarAsync(Cliente cliente)
    {
        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                UPDATE clientes
                SET
                    nombre = @Nombre,
                    contacto = @Contacto,
                    updated_at = timezone('America/Bogota', now())
                WHERE id = @Id
                  AND empresa_id = @EmpresaId
                  AND activo = true
                RETURNING id;";

            var result = await conn.ExecuteScalarAsync<Guid?>(sql, cliente);

            return result.HasValue;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> EliminarAsync(Guid id, Guid empresaId)
    {
        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                UPDATE clientes
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
        catch
        {
            throw;
        }
    }
}