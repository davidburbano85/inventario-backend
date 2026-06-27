using Dapper;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Dominio.Entidades;
using inventarioWebAI.Infraestructura.AccesoDatos;

namespace inventarioWebAI.Infraestructura.RepositoriosDapper;

public class CategoriaRepositorioDapper : ICategoriaRepositorio
{
    private readonly IDbConnectionFactory _db;
    private readonly ILogger<CategoriaRepositorioDapper> _logger;

    public CategoriaRepositorioDapper(
        IDbConnectionFactory db,
        ILogger<CategoriaRepositorioDapper> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Guid> CrearCategoriaAsync(Categoria categoria)
    {
        _logger.LogInformation("[CrearCategoriaAsync] INICIO");

        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                INSERT INTO categorias
                (
                    empresa_id,
                    nombre,
                    created_at
                )
                VALUES
                (
                    @EmpresaId,
                    @Nombre,
                    timezone('America/Bogota', now())
                )
                RETURNING id;";

            var result = await conn.ExecuteScalarAsync<Guid>(
                sql,
                new
                {
                    categoria.EmpresaId,
                    categoria.Nombre
                });

            _logger.LogInformation("[CrearCategoriaAsync] OK {CategoriaId}", result);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CrearCategoriaAsync] ERROR");
            throw;
        }
    }

    public async Task<IEnumerable<Categoria?>> ObtenerCategoriasActivasPorEmpresaAsync(Guid empresaId)
    {
        _logger.LogInformation("[ObtenerCategoriasActivasPorEmpresaAsync] INICIO");

        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                SELECT
                    id,
                    empresa_id,
                    nombre,
                    created_at,
                    updated_at,
                    activo
                FROM categorias
                WHERE empresa_id = @EmpresaId
                  AND activo = true;";

            return await conn.QueryAsync<Categoria>(
                sql,
                new { EmpresaId = empresaId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ObtenerCategoriasActivasPorEmpresaAsync] ERROR");
            throw;
        }
    }

    public async Task<Categoria?> ObtenerCategoriaPorIdAsync(Guid id, Guid empresaId)
    {
        _logger.LogInformation("[ObtenerCategoriaPorIdAsync] INICIO");

        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                SELECT
                    id,
                    empresa_id,
                    nombre,
                    created_at,
                    updated_at,
                    activo
                FROM categorias
                WHERE id = @Id
                  AND empresa_id = @EmpresaId;";

            return await conn.QueryFirstOrDefaultAsync<Categoria>(
                sql,
                new { Id = id, EmpresaId = empresaId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ObtenerCategoriaPorIdAsync] ERROR");
            throw;
        }
    }

    public async Task<bool> ActualizarCategoriaAsync(Categoria categoria)
    {
        _logger.LogInformation("[ActualizarCategoriaAsync] INICIO");

        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                UPDATE categorias
                SET
                    nombre = @Nombre,
                    updated_at = timezone('America/Bogota', now())
                WHERE id = @Id
                  AND empresa_id = @EmpresaId
                  AND activo = true
                RETURNING id;";

            var result = await conn.ExecuteScalarAsync<Guid?>(
                sql,
                new
                {
                    categoria.Id,
                    categoria.EmpresaId,
                    categoria.Nombre
                });

            return result.HasValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ActualizarCategoriaAsync] ERROR");
            throw;
        }
    }

    public async Task<bool> EliminarCategoriaAsync(Guid id, Guid empresaId)
    {
        _logger.LogInformation("[EliminarCategoriaAsync] INICIO");

        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                DELETE FROM categorias
                WHERE id = @Id
                  AND empresa_id = @EmpresaId;";

            var rows = await conn.ExecuteAsync(sql, new
            {
                Id = id,
                EmpresaId = empresaId
            });

            return rows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EliminarCategoriaAsync] ERROR");
            throw;
        }
    }
}