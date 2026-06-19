//// Ubicación: /src/Aplicacion/Servicios/VentasServicio.cs

//using Dapper;
//using inventarioWebAI.Aplicacion.DTOs;
//using inventarioWebAI.Aplicacion.Interfaces;
//using inventarioWebAI.Infraestructura.conexion;
//using System.Data;

//namespace inventarioWebAI.Aplicacion.Servicios;

//public class VentasServicio : IVentasServicio
//{
//    private readonly DbConnectionFactory _db;

//    public VentasServicio(DbConnectionFactory db)
//    {
//        _db = db;
//    }

//    public async Task<IEnumerable<VentaDTO>> ObtenerPorEmpresa(Guid empresaId)
//    {
//        using var connection = _db.CrearConexion();

//        var sql = @"
//            SELECT
//                id,
//                empresa_id AS EmpresaId,
//                cliente_id AS ClienteId,
//                total,
//                created_at AS CreatedAt
//            FROM ventas
//            WHERE empresa_id = @EmpresaId
//            ORDER BY created_at DESC
//        ";

//        return await connection.QueryAsync<VentaDTO>(sql, new { EmpresaId = empresaId });
//    }

//    public async Task<Guid> Crear(CrearVentaDTO dto)
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
//                throw new InvalidOperationException("La venta debe tener al menos un detalle.");

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

//            // MODIFICADO: validación de stock por producto + almacén
//            foreach (var item in dto.Detalles)
//            {
//                var sqlStock = @"
//                    SELECT COALESCE(cantidad,0)
//                    FROM stock_actual
//                    WHERE empresa_id = @EmpresaId
//                      AND producto_id = @ProductoId
//                      AND almacen_id = @AlmacenId;
//                ";

//                var stockDisponible = await connection.ExecuteScalarAsync<decimal?>(sqlStock, new
//                {
//                    dto.EmpresaId,
//                    item.ProductoId,
//                    dto.AlmacenId
//                }, transaction) ?? 0;

//                if (stockDisponible < item.Cantidad)
//                    throw new InvalidOperationException($"Stock insuficiente para el producto {item.ProductoId} en el almacén {dto.AlmacenId}.");
//            }

//            var total = dto.Detalles.Sum(x => x.Cantidad * x.Precio);

//            var sqlVenta = @"
//                INSERT INTO ventas (
//                    empresa_id,
//                    cliente_id,
//                    total
//                )
//                VALUES (
//                    @EmpresaId,
//                    @ClienteId,
//                    @Total
//                )
//                RETURNING id;
//            ";

//            var ventaId = await connection.ExecuteScalarAsync<Guid>(sqlVenta, new
//            {
//                dto.EmpresaId,
//                dto.ClienteId,
//                Total = total
//            }, transaction);

//            var sqlDetalle = @"
//                INSERT INTO ventas_detalle (
//                    empresa_id,
//                    venta_id,
//                    producto_id,
//                    cantidad,
//                    precio
//                )
//                VALUES (
//                    @EmpresaId,
//                    @VentaId,
//                    @ProductoId,
//                    @Cantidad,
//                    @Precio
//                );
//            ";

//            foreach (var item in dto.Detalles)
//            {
//                await connection.ExecuteAsync(sqlDetalle, new
//                {
//                    EmpresaId = dto.EmpresaId,
//                    VentaId = ventaId,
//                    item.ProductoId,
//                    item.Cantidad,
//                    item.Precio
//                }, transaction);
//            }

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
//                    'salida',
//                    @Cantidad,
//                    @Motivo
//                );
//            ";

//            foreach (var item in dto.Detalles)
//            {
//                await connection.ExecuteAsync(sqlMovimiento, new
//                {
//                    EmpresaId = dto.EmpresaId,
//                    item.ProductoId,
//                    AlmacenId = dto.AlmacenId,
//                    UsuarioId = dto.UsuarioId,
//                    item.Cantidad,
//                    Motivo = $"Venta {ventaId}"
//                }, transaction);
//            }

//            transaction.Commit();

//            return ventaId;
//        }
//        catch
//        {
//            transaction.Rollback();
//            throw;
//        }
//    }
//}

