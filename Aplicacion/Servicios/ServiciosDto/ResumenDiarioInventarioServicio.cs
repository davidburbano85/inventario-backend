//// Ubicación: /src/Aplicacion/Servicios/ResumenDiarioInventarioServicio.cs

//using Dapper;
//using inventarioWebAI.Aplicacion.DTOs;
//using inventarioWebAI.Aplicacion.Interfaces;
//using inventarioWebAI.Infraestructura.conexion;

//namespace inventarioWebAI.Aplicacion.Servicios;

//// NUEVO: servicio de resumen diario de inventario (read-model)
//// POR QUÉ:
//// - Proveer vista agregada por día sin tocar lógica de movimientos
//// - Preparado para reportes / dashboards
//public class ResumenDiarioInventarioServicio : IResumenDiarioInventarioServicio
//{
//    private readonly DbConnectionFactory _db;

//    public ResumenDiarioInventarioServicio(DbConnectionFactory db)
//    {
//        _db = db;
//    }

//    // NUEVO: obtiene resumen diario por empresa
//    public async Task<IEnumerable<ResumenDiarioInventarioDTO>> ObtenerPorEmpresa(Guid empresaId)
//    {
//        using var connection = _db.CrearConexion();

//        var sql = @"
//            SELECT
//                producto_id AS ProductoId,
//                almacen_id AS AlmacenId,
//                DATE(created_at) AS Fecha, -- NUEVO: agrupación diaria
//                SUM(CASE WHEN tipo = 'entrada' THEN cantidad ELSE 0 END) AS TotalEntradas,
//                SUM(CASE WHEN tipo = 'salida' THEN cantidad ELSE 0 END) AS TotalSalidas
//            FROM movimientos_inventario
//            WHERE empresa_id = @EmpresaId
//            GROUP BY producto_id, almacen_id, DATE(created_at)
//            ORDER BY Fecha DESC;
//        ";

//        return await connection.QueryAsync<ResumenDiarioInventarioDTO>(sql, new
//        {
//            EmpresaId = empresaId
//        });
//    }

//    // NUEVO: resumen por producto específico
//    public async Task<IEnumerable<ResumenDiarioInventarioDTO>> ObtenerPorProducto(Guid empresaId, Guid productoId)
//    {
//        using var connection = _db.CrearConexion();

//        var sql = @"
//            SELECT
//                producto_id AS ProductoId,
//                almacen_id AS AlmacenId,
//                DATE(created_at) AS Fecha,
//                SUM(CASE WHEN tipo = 'entrada' THEN cantidad ELSE 0 END) AS TotalEntradas,
//                SUM(CASE WHEN tipo = 'salida' THEN cantidad ELSE 0 END) AS TotalSalidas
//            FROM movimientos_inventario
//            WHERE empresa_id = @EmpresaId
//              AND producto_id = @ProductoId
//            GROUP BY producto_id, almacen_id, DATE(created_at)
//            ORDER BY Fecha DESC;
//        ";

//        return await connection.QueryAsync<ResumenDiarioInventarioDTO>(sql, new
//        {
//            EmpresaId = empresaId,
//            ProductoId = productoId
//        });
//    }
//}