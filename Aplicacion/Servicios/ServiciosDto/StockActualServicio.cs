//// Ubicación: /src/Aplicacion/Servicios/StockActualServicio.cs

//using Dapper;
//using inventarioWebAI.Aplicacion.DTOs;
//using inventarioWebAI.Aplicacion.Interfaces;
//using inventarioWebAI.Infraestructura.conexion;

//namespace inventarioWebAI.Aplicacion.Servicios;

//// NUEVO: servicio de solo lectura para stock_actual
//// POR QUÉ:
//// - Evitar modificar InventarioServicio (ya cerrado)
//// - Separar queries (read-model) de comandos (write-model)
//public class StockActualServicio : IStockActualServicio
//{
//    private readonly DbConnectionFactory _db;

//    public StockActualServicio(DbConnectionFactory db)
//    {
//        _db = db;
//    }

//    public async Task<IEnumerable<StockActualDTO>> ObtenerPorEmpresa(Guid empresaId)
//    {
//        using var connection = _db.CrearConexion();

//        var sql = @"
//            SELECT
//                producto_id AS ProductoId,
//                almacen_id AS AlmacenId,
//                cantidad,
//                updated_at AS UpdatedAt
//            FROM stock_actual
//            WHERE empresa_id = @EmpresaId
//            ORDER BY producto_id, almacen_id;
//        ";

//        return await connection.QueryAsync<StockActualDTO>(sql, new
//        {
//            EmpresaId = empresaId
//        });
//    }

//    public async Task<IEnumerable<StockActualDTO>> ObtenerPorProducto(Guid empresaId, Guid productoId)
//    {
//        using var connection = _db.CrearConexion();

//        var sql = @"
//            SELECT
//                producto_id AS ProductoId,
//                almacen_id AS AlmacenId,
//                cantidad,
//                updated_at AS UpdatedAt
//            FROM stock_actual
//            WHERE empresa_id = @EmpresaId
//              AND producto_id = @ProductoId
//            ORDER BY almacen_id;
//        ";

//        return await connection.QueryAsync<StockActualDTO>(sql, new
//        {
//            EmpresaId = empresaId,
//            ProductoId = productoId
//        });
//    }
//}