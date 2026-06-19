//// Ubicación: /src/Aplicacion/Servicios/InventarioServicio.cs

//using Dapper;
//using inventarioWebAI.Aplicacion.DTOs;
//using inventarioWebAI.Aplicacion.Interfaces;
//using inventarioWebAI.Infraestructura.conexion;
//using System.Data;

//namespace inventarioWebAI.Aplicacion.Servicios;

//public class InventarioServicio : IInventarioServicio
//{
//    private readonly DbConnectionFactory _db;

//    public InventarioServicio(DbConnectionFactory db)
//    {
//        _db = db;
//    }

//    public async Task<IEnumerable<MovimientoInventarioDTO>> ObtenerMovimientos(Guid empresaId)
//    {
//        using var connection = _db.CrearConexion();

//        var sql = @"
//            SELECT
//                id,
//                producto_id AS ProductoId,
//                almacen_id AS AlmacenId,
//                usuario_id AS UsuarioId,
//                tipo,
//                cantidad,
//                motivo,
//                created_at AS CreatedAt
//            FROM movimientos_inventario
//            WHERE empresa_id = @EmpresaId
//            ORDER BY created_at DESC
//        ";

//        return await connection.QueryAsync<MovimientoInventarioDTO>(sql, new
//        {
//            EmpresaId = empresaId
//        });
//    }

//    public async Task RegistrarMovimiento(CrearMovimientoDTO dto)
//    {
//        using var connection = _db.CrearConexion();
//        connection.Open();

//        using var transaction = connection.BeginTransaction();

//        try
//        {
//            // EXISTENTE: validaciones básicas
//            if (dto.EmpresaId == Guid.Empty)
//                throw new InvalidOperationException("EmpresaId es requerido.");

//            if (dto.ProductoId == Guid.Empty)
//                throw new InvalidOperationException("ProductoId es requerido.");

//            if (dto.AlmacenId == Guid.Empty)
//                throw new InvalidOperationException("AlmacenId es requerido.");

//            if (dto.UsuarioId == Guid.Empty)
//                throw new InvalidOperationException("UsuarioId es requerido.");

//            if (dto.Cantidad <= 0)
//                throw new InvalidOperationException("La cantidad debe ser mayor a 0.");

//            // NUEVO: validación de pertenencia usuario → empresa (multi-tenant real)
//            var sqlValidarUsuarioEmpresa = @"
//                SELECT pertenece_empresa(@EmpresaId);
//            ";

//            var pertenece = await connection.ExecuteScalarAsync<bool>(sqlValidarUsuarioEmpresa, new
//            {
//                dto.EmpresaId
//            }, transaction);

//            if (!pertenece)
//                throw new UnauthorizedAccessException("El usuario no pertenece a la empresa."); // NUEVO

//            // EXISTENTE: validar producto pertenece a empresa
//            var sqlValidarProducto = @"
//                SELECT COUNT(1)
//                FROM productos
//                WHERE id = @ProductoId
//                  AND empresa_id = @EmpresaId;
//            ";

//            var productoValido = await connection.ExecuteScalarAsync<int>(sqlValidarProducto, new
//            {
//                dto.ProductoId,
//                dto.EmpresaId
//            }, transaction);

//            if (productoValido == 0)
//                throw new InvalidOperationException("El producto no pertenece a la empresa.");

//            // EXISTENTE: validar almacén pertenece a empresa
//            var sqlValidarAlmacen = @"
//                SELECT COUNT(1)
//                FROM almacenes
//                WHERE id = @AlmacenId
//                  AND empresa_id = @EmpresaId;
//            ";

//            var almacenValido = await connection.ExecuteScalarAsync<int>(sqlValidarAlmacen, new
//            {
//                dto.AlmacenId,
//                dto.EmpresaId
//            }, transaction);

//            if (almacenValido == 0)
//                throw new InvalidOperationException("El almacén no pertenece a la empresa.");

//            // EXISTENTE: asegurar existencia en stock_actual
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

//            await connection.ExecuteAsync(sqlEnsureStock, new
//            {
//                dto.EmpresaId,
//                dto.ProductoId,
//                dto.AlmacenId
//            }, transaction);

//            // EXISTENTE: control de salida con bloqueo
//            if (dto.Tipo.ToString().ToLower() == "salida")
//            {
//                var sqlStock = @"
//                    SELECT cantidad
//                    FROM stock_actual
//                    WHERE empresa_id = @EmpresaId
//                      AND producto_id = @ProductoId
//                      AND almacen_id = @AlmacenId
//                    FOR UPDATE;
//                ";

//                var stockDisponible = await connection.ExecuteScalarAsync<decimal?>(sqlStock, new
//                {
//                    dto.EmpresaId,
//                    dto.ProductoId,
//                    dto.AlmacenId
//                }, transaction) ?? 0;

//                if (stockDisponible < dto.Cantidad)
//                    throw new InvalidOperationException("Stock insuficiente para realizar la salida.");
//            }

//            var sql = @"
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
//                    @Tipo,
//                    @Cantidad,
//                    @Motivo
//                );
//            ";

//            await connection.ExecuteAsync(sql, new
//            {
//                dto.EmpresaId,
//                dto.ProductoId,
//                dto.AlmacenId,
//                dto.UsuarioId,
//                Tipo = dto.Tipo.ToString().ToLower(),
//                dto.Cantidad,
//                dto.Motivo
//            }, transaction);

//            transaction.Commit();
//        }
//        catch
//        {
//            transaction.Rollback();
//            throw;
//        }
//    }
//}

