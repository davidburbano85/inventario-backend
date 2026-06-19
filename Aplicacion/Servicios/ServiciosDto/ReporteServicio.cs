//// Ubicación: /src/Aplicacion/Servicios/ReporteServicio.cs

//using Dapper;
//using inventarioWebAI.Aplicacion.DTOs;
//using inventarioWebAI.Aplicacion.Interfaces;
//using inventarioWebAI.Infraestructura.conexion;

//namespace inventarioWebAI.Aplicacion.Servicios;

//public class ReporteServicio : IReporteServicio
//{
//    private readonly DbConnectionFactory _db;

//    public ReporteServicio(DbConnectionFactory db)
//    {
//        _db = db;
//    }

//    public async Task<decimal> ObtenerVentasPorPeriodo(Guid empresaId, DateTime desde, DateTime hasta)
//    {
//        using var connection = _db.CrearConexion();

//        var sql = @"
//            SELECT COALESCE(SUM(total),0) FROM ventas
//            WHERE empresa_id = @EmpresaId
//              AND created_at >= @Desde
//              AND created_at <= @Hasta;
//        ";

//        var total = await connection.ExecuteScalarAsync<decimal>(sql, new { EmpresaId = empresaId, Desde = desde, Hasta = hasta });
//        return total;
//    }

//    public async Task<IEnumerable<ProductoDTO>> ObtenerTopProductos(Guid empresaId, DateTime desde, DateTime hasta, int topN = 10)
//    {
//        using var connection = _db.CrearConexion();

//        var sql = @"
//            SELECT p.id, p.categoria_id AS CategoriaId, p.nombre AS Nombre, p.codigo_sku AS CodigoSku, p.precio_venta AS PrecioVenta, p.precio_compra AS PrecioCompra, p.activo AS Activo
//            FROM ventas_detalle vd
//            INNER JOIN productos p ON p.id = vd.producto_id
//            INNER JOIN ventas v ON v.id = vd.venta_id
//            WHERE v.empresa_id = @EmpresaId
//              AND v.created_at >= @Desde
//              AND v.created_at <= @Hasta
//            GROUP BY p.id, p.categoria_id, p.nombre, p.codigo_sku, p.precio_venta, p.precio_compra, p.activo
//            ORDER BY SUM(vd.cantidad) DESC
//            LIMIT @TopN;
//        ";

//        return await connection.QueryAsync<ProductoDTO>(sql, new { EmpresaId = empresaId, Desde = desde, Hasta = hasta, TopN = topN });
//    }

//    public async Task<decimal> ObtenerStockValorizado(Guid empresaId)
//    {
//        using var connection = _db.CrearConexion();

//        var sql = @"
//            SELECT COALESCE(SUM(sa.cantidad * COALESCE(p.precio_compra,0)),0)
//            FROM stock_actual sa
//            INNER JOIN productos p ON p.id = sa.producto_id
//            WHERE sa.empresa_id = @EmpresaId;
//        ";

//        var total = await connection.ExecuteScalarAsync<decimal>(sql, new { EmpresaId = empresaId });
//        return total;
//    }

//    public async Task<IEnumerable<ResumenDiarioInventarioDTO>> ObtenerResumenDiario(Guid empresaId, DateTime fecha)
//    {
//        using var connection = _db.CrearConexion();

//        var sql = @"
//            SELECT producto_id AS ProductoId, DATE(fecha) AS Fecha, total_ventas AS TotalVentas, total_compras AS TotalCompras
//            FROM resumen_diario_inventario
//            WHERE empresa_id = @EmpresaId
//              AND fecha = @Fecha;
//        ";

//        return await connection.QueryAsync<ResumenDiarioInventarioDTO>(sql, new { EmpresaId = empresaId, Fecha = fecha.Date });
//    }
//}
