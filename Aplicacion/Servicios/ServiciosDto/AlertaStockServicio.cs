//// Ubicación: /src/Aplicacion/Servicios/AlertaStockServicio.cs

//using Dapper;
//using inventarioWebAI.Aplicacion.DTOs;
//using inventarioWebAI.Aplicacion.Interfaces;
//using inventarioWebAI.Infraestructura.Conexion;

//namespace inventarioWebAI.Aplicacion.Servicios;

//public class AlertaStockServicio : IAlertaStockServicio
//{
//    private readonly DbConnectionFactory _db;

//    public AlertaStockServicio(DbConnectionFactory db)
//    {
//        _db = db;
//    }

//    public async Task<IEnumerable<StockActualDTO>> ObtenerProductosBajoUmbral(Guid empresaId, decimal umbral)
//    {
//        using var connection = _db.CrearConexion();

//        var sql = @"
//            SELECT producto_id AS ProductoId, almacen_id AS AlmacenId, cantidad
//            FROM stock_actual
//            WHERE empresa_id = @EmpresaId
//              AND cantidad < @Umbral
//            ORDER BY cantidad ASC;
//        ";

//        return await connection.QueryAsync<StockActualDTO>(sql, new { EmpresaId = empresaId, Umbral = umbral });
//    }

//    public async Task<Guid> RegistrarAlerta(Guid empresaId, Guid productoId, decimal umbral, string nota)
//    {
//        using var connection = _db.CrearConexion();

//        var sql = @"
//            INSERT INTO logs_sistema (empresa_id, accion, detalle)
//            VALUES (@EmpresaId, @Accion, @Detalle)
//            RETURNING id;
//        ";

//        var detalle = $"Alerta stock producto {productoId} umbral {umbral}: {nota}";

//        var id = await connection.ExecuteScalarAsync<Guid>(sql, new { EmpresaId = empresaId, Accion = "AlertaStock", Detalle = detalle });
//        return id;
//    }

//    public async Task MarcarAlertaAtendida(Guid alertaId, Guid usuarioId)
//    {
//        using var connection = _db.CrearConexion();

//        var sql = @"
//            UPDATE logs_sistema
//            SET detalle = detalle || ' | Atendida por ' || @UsuarioId
//            WHERE id = @Id;
//        ";

//        await connection.ExecuteAsync(sql, new { Id = alertaId, UsuarioId = usuarioId });
//    }
//}
