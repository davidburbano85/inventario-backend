//// Ubicación: /src/Aplicacion/Servicios/StockServicio.cs

//using Dapper;
//using inventarioWebAI.Aplicacion.DTOs;
//using inventarioWebAI.Aplicacion.Interfaces;
//using inventarioWebAI.Infraestructura.conexion;

//namespace inventarioWebAI.Aplicacion.Servicios;

//public class StockServicio : IStockServicio
//{
//    private readonly DbConnectionFactory _db;

//    public StockServicio(DbConnectionFactory db)
//    {
//        _db = db;
//    }

//    // Crea fila inicial en stock_actual si no existe, o actualiza cantidad si existe
//    public async Task<Guid> CrearStockInicial(Guid empresaId, Guid productoId, Guid almacenId, decimal cantidad)
//    {
//        using var connection = _db.CrearConexion();

//        if (empresaId == Guid.Empty) throw new InvalidOperationException("EmpresaId es requerido.");
//        if (productoId == Guid.Empty) throw new InvalidOperationException("ProductoId es requerido.");
//        if (almacenId == Guid.Empty) throw new InvalidOperationException("AlmacenId es requerido.");

//        var sqlExiste = @"
//            SELECT id
//            FROM stock_actual
//            WHERE empresa_id = @EmpresaId
//              AND producto_id = @ProductoId
//              AND almacen_id = @AlmacenId;
//        ";

//        var existenteId = await connection.ExecuteScalarAsync<Guid?>(sqlExiste, new
//        {
//            EmpresaId = empresaId,
//            ProductoId = productoId,
//            AlmacenId = almacenId
//        });

//        if (existenteId is not null)
//        {
//            var sqlUpdate = @"
//                UPDATE stock_actual
//                SET cantidad = @Cantidad, updated_at = now()
//                WHERE id = @Id
//                RETURNING id;
//            ";

//            var id = await connection.ExecuteScalarAsync<Guid>(sqlUpdate, new
//            {
//                Cantidad = cantidad,
//                Id = existenteId.Value
//            });

//            return id;
//        }

//        var sqlInsert = @"
//            INSERT INTO stock_actual (
//                empresa_id,
//                producto_id,
//                almacen_id,
//                cantidad
//            ) VALUES (
//                @EmpresaId,
//                @ProductoId,
//                @AlmacenId,
//                @Cantidad
//            ) RETURNING id;
//        ";

//        var newId = await connection.ExecuteScalarAsync<Guid>(sqlInsert, new
//        {
//            EmpresaId = empresaId,
//            ProductoId = productoId,
//            AlmacenId = almacenId,
//            Cantidad = cantidad
//        });

//        return newId;
//    }

//    // Ajuste manual: crea un movimiento_inventario; triggers deben actualizar stock_actual
//    public async Task<Guid> AjustarStockManual(CrearMovimientoDTO dto)
//    {
//        using var connection = _db.CrearConexion();

//        if (dto.EmpresaId == Guid.Empty) throw new InvalidOperationException("EmpresaId es requerido.");
//        if (dto.ProductoId == Guid.Empty) throw new InvalidOperationException("ProductoId es requerido.");
//        if (dto.AlmacenId == Guid.Empty) throw new InvalidOperationException("AlmacenId es requerido.");
//        if (dto.Cantidad <= 0) throw new InvalidOperationException("Cantidad debe ser mayor a cero.");

//        var sql = @"
//            INSERT INTO movimientos_inventario (
//                empresa_id,
//                producto_id,
//                almacen_id,
//                usuario_id,
//                tipo,
//                cantidad,
//                motivo
//            ) VALUES (
//                @EmpresaId,
//                @ProductoId,
//                @AlmacenId,
//                @UsuarioId,
//                @Tipo,
//                @Cantidad,
//                @Motivo
//            ) RETURNING id;
//        ";

//        var usuarioParam = dto.UsuarioId == Guid.Empty ? (object)DBNull.Value : dto.UsuarioId;
//        var id = await connection.ExecuteScalarAsync<Guid>(sql, new
//        {
//            dto.EmpresaId,
//            dto.ProductoId,
//            dto.AlmacenId,
//            UsuarioId = usuarioParam,
//            Tipo = dto.Tipo.ToString(),
//            dto.Cantidad,
//            dto.Motivo
//        });

//        return id;
//    }
//}
