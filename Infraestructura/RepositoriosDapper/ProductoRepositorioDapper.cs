using Dapper;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Dominio.Entidades;
using inventarioWebAI.Infraestructura.AccesoDatos;

namespace inventarioWebAI.Infraestructura.RepositoriosDapper;

public class ProductoRepositorioDapper : IProductoRepositorio
{
    private readonly IDbConnectionFactory _db;
    private readonly ILogger<ProductoRepositorioDapper> _logger;

    public ProductoRepositorioDapper(
        IDbConnectionFactory db,
        ILogger<ProductoRepositorioDapper> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Guid> CrearProductoAsync(Producto producto)
    {
        _logger.LogInformation("[CrearProductoAsync] INICIO");

        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                INSERT INTO productos
                (
                    empresa_id,
                    categoria_id,
                    nombre,
                    codigo_sku,
                    precio_venta,
                    precio_compra,
                    created_at
                )
                VALUES
                (
                    @EmpresaId,
                    @CategoriaId,
                    @Nombre,
                    @CodigoSku,
                    @PrecioVenta,
                    @PrecioCompra,
                    timezone('America/Bogota', now())
                )
                RETURNING id;";

            var result = await conn.ExecuteScalarAsync<Guid>(
                sql,
                new
                {
                    producto.EmpresaId,
                    producto.CategoriaId,
                    producto.Nombre,
                    producto.CodigoSku,
                    producto.PrecioVenta,
                    producto.PrecioCompra
                });

            _logger.LogInformation("[CrearProductoAsync] OK {ProductoId}", result);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CrearProductoAsync] ERROR");
            throw;
        }
    }

    public async Task<IEnumerable<Producto?>> ObtenerProductosActivosPorEmpresaAsync(Guid empresaId)
    {
        _logger.LogInformation("[ObtenerProductosActivosPorEmpresaAsync] INICIO");

        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                SELECT
                    id,
                    empresa_id,
                    categoria_id,
                    nombre,
                    codigo_sku,
                    precio_venta,
                    precio_compra,
                    created_at,
                    updated_at,
                    activo
                FROM productos
                WHERE empresa_id = @EmpresaId
                  AND activo = true;";

            return await conn.QueryAsync<Producto>(
                sql,
                new { EmpresaId = empresaId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ObtenerProductosActivosPorEmpresaAsync] ERROR");
            throw;
        }
    }

    public async Task<IEnumerable<Producto?>> ObtenerProductosPorCategoriaAsync(Guid empresaId, Guid categoriaId)
    {
        _logger.LogInformation("[ObtenerProductosPorCategoriaAsync] INICIO");

        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                SELECT
                    id,
                    empresa_id,
                    categoria_id,
                    nombre,
                    codigo_sku,
                    precio_venta,
                    precio_compra,
                    created_at,
                    updated_at,
                    activo
                FROM productos
                WHERE empresa_id = @EmpresaId
                  AND categoria_id = @CategoriaId
                  AND activo = true;";

            return await conn.QueryAsync<Producto>(
                sql,
                new { EmpresaId = empresaId, CategoriaId = categoriaId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ObtenerProductosPorCategoriaAsync] ERROR");
            throw;
        }
    }

    public async Task<Producto?> ObtenerProductoPorIdAsync(Guid id, Guid empresaId)
    {
        _logger.LogInformation("[ObtenerProductoPorIdAsync] INICIO");

        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                SELECT
                    id,
                    empresa_id,
                    categoria_id,
                    nombre,
                    codigo_sku,
                    precio_venta,
                    precio_compra,
                    created_at,
                    updated_at,
                    activo
                FROM productos
                WHERE id = @Id
                  AND empresa_id = @EmpresaId;";

            return await conn.QueryFirstOrDefaultAsync<Producto>(
                sql,
                new { Id = id, EmpresaId = empresaId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ObtenerProductoPorIdAsync] ERROR");
            throw;
        }
    }

    public async Task<bool> ActualizarProductoAsync(Producto producto)
    {
        _logger.LogInformation("[ActualizarProductoAsync] INICIO");

        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                UPDATE productos
                SET
                    categoria_id = @CategoriaId,
                    nombre = @Nombre,
                    codigo_sku = @CodigoSku,
                    precio_venta = @PrecioVenta,
                    precio_compra = @PrecioCompra,
                    updated_at = timezone('America/Bogota', now())
                WHERE id = @Id
                  AND empresa_id = @EmpresaId
                  AND activo = true
                RETURNING id;";

            var result = await conn.ExecuteScalarAsync<Guid?>(
                sql,
                new
                {
                    producto.Id,
                    producto.EmpresaId,
                    producto.CategoriaId,
                    producto.Nombre,
                    producto.CodigoSku,
                    producto.PrecioVenta,
                    producto.PrecioCompra
                });

            return result.HasValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ActualizarProductoAsync] ERROR");
            throw;
        }
    }

    public async Task<bool> EliminarProductoAsync(Guid id, Guid empresaId)
    {
        _logger.LogInformation("[EliminarProductoAsync] INICIO");

        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                UPDATE productos
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EliminarProductoAsync] ERROR");
            throw;
        }
    }
}