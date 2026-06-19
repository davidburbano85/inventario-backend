//// Ubicación: /src/Aplicacion/Servicios/ProductoServicio.cs

//using Dapper;
//using inventarioWebAI.Aplicacion.DTOs;
//using inventarioWebAI.Aplicacion.Interfaces;
//using inventarioWebAI.Infraestructura.conexion;

//namespace inventarioWebAI.Aplicacion.Servicios;

//public class ProductoServicio : IProductoServicio
//{
//    private readonly DbConnectionFactory _db;

//    public ProductoServicio(DbConnectionFactory db)
//    {
//        _db = db;
//    }

//    public async Task<IEnumerable<ProductoDTO>> ObtenerPorEmpresa(Guid empresaId)
//    {
//        using var connection = _db.CrearConexion();

//        var sql = @" 
//            SELECT 
//                id,
//                nombre,
//                codigo_sku AS CodigoSku,
//                precio_venta AS PrecioVenta,
//                precio_compra AS PrecioCompra,
//                activo
//            FROM productos
//            WHERE empresa_id = @EmpresaId
//        ";

//        return await connection.QueryAsync<ProductoDTO>(sql, new
//        {
//            EmpresaId = empresaId
//        });
//    }

//    public async Task<ProductoDTO?> ObtenerPorId(Guid empresaId, Guid productoId)
//    {
//        using var connection = _db.CrearConexion();

//        var sql = @"
//            SELECT 
//                id,
//                nombre,
//                codigo_sku AS CodigoSku,
//                precio_venta AS PrecioVenta,
//                precio_compra AS PrecioCompra,
//                activo
//            FROM productos
//            WHERE empresa_id = @EmpresaId AND id=@ProductoId
//        ";

//        return await connection.QueryFirstOrDefaultAsync<ProductoDTO>(sql, new
//        {
//            EmpresaId = empresaId,
//            ProductoId = productoId
//        });
//    }

//    public async Task<Guid> Crear(CrearProductoDTO dto)
//    {
//        using var connection = _db.CrearConexion();

//        // EXISTENTE: validaciones básicas
//        if (dto.EmpresaId == Guid.Empty)
//            throw new InvalidOperationException("EmpresaId es requerido.");

//        if (string.IsNullOrWhiteSpace(dto.Nombre))
//            throw new InvalidOperationException("El nombre del producto es obligatorio.");

//        if (dto.PrecioVenta < 0)
//            throw new InvalidOperationException("El precio de venta no puede ser negativo.");

//        if (dto.PrecioCompra.HasValue && dto.PrecioCompra < 0)
//            throw new InvalidOperationException("El precio de compra no puede ser negativo.");

//        // NUEVO: validación multi-tenant usuario → empresa
//        // Gap detectado: CrearProductoDTO no contiene UsuarioId → no se puede validar correctamente el usuario
//        // Se usa validación DB indirecta como fallback (RLS también protege)
//        var sqlValidarEmpresa = @"
//            SELECT 1
//            FROM empresas
//            WHERE id = @EmpresaId;
//        ";

//        var empresaExiste = await connection.ExecuteScalarAsync<int?>(sqlValidarEmpresa, new
//        {
//            dto.EmpresaId
//        });

//        if (empresaExiste is null)
//            throw new InvalidOperationException("La empresa no existe."); // NUEVO

//        // EXISTENTE: normalización
//        var nombreNormalizado = dto.Nombre.Trim();
//        var skuNormalizado = dto.CodigoSku?.Trim();

//        // EXISTENTE: validación de SKU
//        if (!string.IsNullOrWhiteSpace(skuNormalizado))
//        {
//            var sqlValidarSku = @"
//                SELECT COUNT(1)
//                FROM productos
//                WHERE empresa_id = @EmpresaId
//                  AND codigo_sku = @CodigoSku;
//            ";

//            var existe = await connection.ExecuteScalarAsync<int>(sqlValidarSku, new
//            {
//                dto.EmpresaId,
//                CodigoSku = skuNormalizado
//            });

//            if (existe > 0)
//                throw new InvalidOperationException("El código SKU ya existe para esta empresa.");
//        }

//        var sql = @"
//            INSERT INTO productos (
//                empresa_id,
//                categoria_id,
//                nombre,
//                codigo_sku,
//                precio_venta,
//                precio_compra
//            )
//            VALUES (
//                @EmpresaId,
//                @CategoriaId,
//                @Nombre,
//                @CodigoSku,
//                @PrecioVenta,
//                @PrecioCompra
//            )
//            RETURNING id;
//        ";

//        var id = await connection.ExecuteScalarAsync<Guid>(sql, new
//        {
//            dto.EmpresaId,
//            dto.CategoriaId,
//            Nombre = nombreNormalizado,
//            CodigoSku = skuNormalizado,
//            dto.PrecioVenta,
//            dto.PrecioCompra
//        });

//        return id;
//    }
//}
