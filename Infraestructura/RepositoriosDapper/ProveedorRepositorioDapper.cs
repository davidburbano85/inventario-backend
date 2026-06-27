using Dapper;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Dominio.Entidades;
using inventarioWebAI.Infraestructura.AccesoDatos;

namespace inventarioWebAI.Infraestructura.RepositoriosDapper;

public class ProveedorRepositorioDapper : IProveedorRepositorio
{
    private readonly IDbConnectionFactory _db;
    private readonly ILogger<ProveedorRepositorioDapper> _logger;

    public ProveedorRepositorioDapper(
        IDbConnectionFactory db,
        ILogger<ProveedorRepositorioDapper> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Guid> CrearAsync(Proveedor proveedor)
    {
        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                INSERT INTO proveedores
                (
                    empresa_id,
                    nombre,
                    contacto,
                    created_at,
                    updated_at,
                    activo
                )
                VALUES
                (
                    @EmpresaId,
                    @Nombre,
                    @Contacto,
                    timezone('America/Bogota', now()),
                    timezone('America/Bogota', now()),
                    true
                )
                RETURNING id;";

            return await conn.ExecuteScalarAsync<Guid>(sql, new
            {
                proveedor.EmpresaId,
                proveedor.Nombre,
                proveedor.Contacto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ProveedorRepositorio][CrearAsync] Error");
            throw;
        }
    }

    public async Task<IEnumerable<Proveedor?>> ObtenerPorEmpresaAsync(Guid empresaId)
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
                FROM proveedores
                WHERE empresa_id = @EmpresaId
                  AND activo = true;";

            return await conn.QueryAsync<Proveedor>(sql, new
            {
                EmpresaId = empresaId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ProveedorRepositorio][ObtenerPorEmpresaAsync] Error");
            throw;
        }
    }

    public async Task<Proveedor?> ObtenerPorIdAsync(Guid id, Guid empresaId)
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
                FROM proveedores
                WHERE id = @Id
                  AND empresa_id = @EmpresaId
                  AND activo = true;";

            return await conn.QueryFirstOrDefaultAsync<Proveedor>(sql, new
            {
                Id = id,
                EmpresaId = empresaId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ProveedorRepositorio][ObtenerPorIdAsync] Error");
            throw;
        }
    }

    public async Task<bool> ActualizarAsync(Proveedor proveedor)
    {
        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                UPDATE proveedores
                SET
                    nombre = @Nombre,
                    contacto = @Contacto,
                    updated_at = timezone('America/Bogota', now())
                WHERE id = @Id
                  AND empresa_id = @EmpresaId
                  AND activo = true
                RETURNING id;";

            var result = await conn.ExecuteScalarAsync<Guid?>(sql, new
            {
                proveedor.Id,
                proveedor.EmpresaId,
                proveedor.Nombre,
                proveedor.Contacto
            });

            return result.HasValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ProveedorRepositorio][ActualizarAsync] Error");
            throw;
        }
    }

    public async Task<bool> EliminarAsync(Guid id, Guid empresaId)
    {
        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                UPDATE proveedores
                SET
                    activo = false,
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ProveedorRepositorio][EliminarAsync] Error");
            throw;
        }
    }
}