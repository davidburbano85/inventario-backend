//// Ubicación: /src/Aplicacion/Servicios/LogSistemaServicio.cs

//using Dapper;
//using inventarioWebAI.Aplicacion.DTOs;
//using inventarioWebAI.Aplicacion.Interfaces;
//using inventarioWebAI.Infraestructura.conexion;

//namespace inventarioWebAI.Aplicacion.Servicios;

//public class LogSistemaServicio : ILogSistemaServicio
//{
//    private readonly DbConnectionFactory _db;

//    public LogSistemaServicio(DbConnectionFactory db)
//    {
//        _db = db;
//    }

//    public async Task<Guid> Crear(LogSistemaDTO dto)
//    {
//        using var connection = _db.CrearConexion();

//        // FIX: agregar validación EmpresaId (existe en DB pero NO en DTO → gap)
//        // Gap detectado: DTO no contiene EmpresaId pero DB lo requiere

//        if (dto.UsuarioId == Guid.Empty)
//            throw new InvalidOperationException("UsuarioId es requerido."); // EXISTENTE

//        if (string.IsNullOrWhiteSpace(dto.Accion))
//            throw new InvalidOperationException("La acción es obligatoria."); // EXISTENTE

//        var sql = @"
//            INSERT INTO logs_sistema ( -- MODIFICADO: nombre correcto de tabla
//                empresa_id,
//                usuario_id,
//                accion,
//                detalle
//            )
//            VALUES (
//                @EmpresaId,
//                @UsuarioId,
//                @Accion,
//                @Detalle
//            )
//            RETURNING id;
//        ";

//        // FIX: EmpresaId ahora requerido → se fuerza desde DTO extendido
//        var id = await connection.ExecuteScalarAsync<Guid>(sql, new
//        {
//            EmpresaId = dto.EmpresaId, // NUEVO: requiere agregar en DTO
//            dto.UsuarioId,
//            dto.Accion,
//            dto.Detalle
//        });

//        return id;
//    }

//    public async Task<IEnumerable<LogSistemaDTO>> ObtenerPorEmpresa(Guid empresaId)
//    {
//        using var connection = _db.CrearConexion();

//        var sql = @"
//            SELECT
//                id,
//                empresa_id AS EmpresaId, -- NUEVO: mapear correctamente
//                usuario_id AS UsuarioId,
//                accion AS Accion,
//                detalle AS Detalle,
//                created_at AS CreatedAt
//            FROM logs_sistema -- MODIFICADO
//            WHERE empresa_id = @EmpresaId
//            ORDER BY created_at DESC;
//        ";

//        return await connection.QueryAsync<LogSistemaDTO>(sql, new
//        {
//            EmpresaId = empresaId
//        });
//    }
//}