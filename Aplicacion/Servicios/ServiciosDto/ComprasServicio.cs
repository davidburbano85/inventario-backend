//// Ubicación: /src/Aplicacion/Servicios/ComprasServicio.cs

//using Dapper;
//using inventarioWebAI.Aplicacion.DTOs;
//using inventarioWebAI.Aplicacion.Interfaces;
//using inventarioWebAI.Infraestructura.conexion;
//using System.Data;

//namespace inventarioWebAI.Aplicacion.Servicios;

//public class ComprasServicio : IComprasServicio
//{
//    private readonly DbConnectionFactory _db;

//    public ComprasServicio(DbConnectionFactory db)
//    {
//        _db = db;
//    }

//    public async Task<IEnumerable<CompraDTO>> ObtenerPorEmpresa(Guid empresaId)
//    {
//        using var connection = _db.CrearConexion();

//        var sql = @"
//            SELECT
//                id,
//                empresa_id AS EmpresaId,
//                proveedor_id AS ProveedorId,
//                total,
//                created_at AS CreatedAt
//            FROM compras
//            WHERE empresa_id = @EmpresaId
//            ORDER BY created_at DESC
//        ";

//        return await connection.QueryAsync<CompraDTO>(sql, new { EmpresaId = empresaId });
//    }

//    public async Task<Guid> Crear(CrearCompraDTO dto)
//    {
//        using var connection = _db.CrearConexion();
//        connection.Open();

//        using var transaction = connection.BeginTransaction();

//        try
//        {
//            // EXISTENTE: validaciones básicas
//            if (dto.EmpresaId == Guid.Empty)
//                throw new InvalidOperationException("EmpresaId es requerido.");

//            if (dto.AlmacenId == Guid.Empty)
//                throw new InvalidOperationException("AlmacenId es requerido.");

//            if (dto.UsuarioId == Guid.Empty)
//                throw new InvalidOperationException("UsuarioId es requerido.");

//            if (!dto.Detalles.Any())
//                throw new InvalidOperationException("La compra debe tener al menos un detalle.");

//            foreach (var item in dto.Detalles)
//            {
//                if (item.Cantidad <= 0)
//                    throw new InvalidOperationException("La cantidad debe ser mayor a 0.");

//                if (item.Precio < 0)
//                    throw new InvalidOperationException("El precio no puede ser negativo.");
//            }

//            // NUEVO: validación multi-tenant usuario → empresa
//            var sqlValidarUsuarioEmpresa = @"
//                SELECT pertenece_empresa(@EmpresaId);
//            ";

//            var pertenece = await connection.ExecuteScalarAsync<bool>(sqlValidarUsuarioEmpresa, new
//            {
//                dto.EmpresaId
//            }, transaction);

//            if (!pertenece)
//                throw new UnauthorizedAccessException("El usuario no pertenece a la empresa."); // NUEVO

//            // EXISTENTE: validar almacén pertenece a empresa
//            var sqlValidarAlmacen = @"
//                SELECT COUNT(1)
//                FROM almacenes
//                WHERE id = @AlmacenId AND empresa_id = @EmpresaId;
//            ";

//            var almacenValido = await connection.ExecuteScalarAsync<int>(sqlValidarAlmacen, new
//            {
//                dto.AlmacenId,
//                dto.EmpresaId
//            }, transaction);

//            if (almacenValido == 0)
//                throw new InvalidOperationException("El almacén no pertenece a la empresa.");

//            // EXISTENTE: validación batch productos
//            var productoIds = dto.Detalles.Select(x => x.ProductoId).Distinct().ToList();

//            var sqlValidarProductos = @"
//                SELECT id
//                FROM productos
//                WHERE empresa_id = @EmpresaId
//                  AND id = ANY(@ProductoIds);
//            ";

//            var productosValidos = (await connection.QueryAsync<Guid>(sqlValidarProductos, new
//            {
//                dto.EmpresaId,
//                ProductoIds = productoIds
//            }, transaction)).ToHashSet();

//            foreach (var id in productoIds)
//            {
//                if (!productosValidos.Contains(id))
//                    throw new InvalidOperationException($"El producto {id} no pertenece a la empresa.");
//            }

//            var total = dto.Detalles.Sum(x => x.Cantidad * x.Precio);

//            var sqlCompra = @"
//                INSERT INTO compras (
//                    empresa_id,
//                    proveedor_id,
//                    total
//                )
//                VALUES (
//                    @EmpresaId,
//                    @ProveedorId,
//                    @Total
//                )
//                RETURNING id;
//            ";

//            var compraId = await connection.ExecuteScalarAsync<Guid>(sqlCompra, new
//            {
//                dto.EmpresaId,
//                dto.ProveedorId,
//                Total = total
//            }, transaction);

//            var sqlDetalle = @"
//                INSERT INTO compras_detalle (
//                    empresa_id,
//                    compra_id,
//                    producto_id,
//                    cantidad,
//                    precio
//                )
//                VALUES (
//                    @EmpresaId,
//                    @CompraId,
//                    @ProductoId,
//                    @Cantidad,
//                    @Precio
//                );
//            ";

//            var sqlEnsureStock = @"
//                INSERT INTO stock_actual (
//                    empresa_id,
//                    producto_id,
//                    almacen_id,
//                    cantidad
//                )
//                VALUES (
//                    @EmpresaId,
//                    @ProductoId,
//                    @AlmacenId,
//                    0
//                )
//                ON CONFLICT (empresa_id, producto_id, almacen_id) DO NOTHING;
//            ";

//            var sqlMovimiento = @"
//                INSERT INTO movimientos_inventario (
//                    empresa_id,
//                    producto_id,
//                    almacen_id,
//                    usuario_id,
//                    tipo,
//                    cantidad,
//                    motivo
//                )
//                VALUES (
//                    @EmpresaId,
//                    @ProductoId,
//                    @AlmacenId,
//                    @UsuarioId,
//                    'entrada',
//                    @Cantidad,
//                    @Motivo
//                );
//            ";

//            foreach (var item in dto.Detalles)
//            {
//                await connection.ExecuteAsync(sqlDetalle, new
//                {
//                    EmpresaId = dto.EmpresaId,
//                    CompraId = compraId,
//                    item.ProductoId,
//                    item.Cantidad,
//                    item.Precio
//                }, transaction);

//                await connection.ExecuteAsync(sqlEnsureStock, new
//                {
//                    dto.EmpresaId,
//                    item.ProductoId,
//                    dto.AlmacenId
//                }, transaction);

//                await connection.ExecuteAsync(sqlMovimiento, new
//                {
//                    dto.EmpresaId,
//                    item.ProductoId,
//                    dto.AlmacenId,
//                    dto.UsuarioId,
//                    item.Cantidad,
//                    Motivo = "Compra"
//                }, transaction);
//            }

//            transaction.Commit();

//            return compraId;
//        }
//        catch
//        {
//            transaction.Rollback();
//            throw;
//        }
//    }
//}

