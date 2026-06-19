//// Ubicación: /src/Aplicacion/Servicios/AlmacenServicio.cs

//using Dapper;
//using inventarioWebAI.Aplicacion.DTOs;
//using inventarioWebAI.Aplicacion.Interfaces;
//using inventarioWebAI.Infraestructura.conexion;

//namespace inventarioWebAI.Aplicacion.Servicios;

//// NUEVO: implementación de servicio de almacenes
//// POR QUÉ:
//// - Mantener consistencia con patrón existente (Servicio + Dapper)
//// - Gestionar multi-tenant correctamente
//public class AlmacenServicio : IAlmacenServicio
//{
//    private readonly DbConnectionFactory _db;

//    public AlmacenServicio(DbConnectionFactory db)
//    {
//        _db = db;
//    }

//    public async Task<IEnumerable<AlmacenDTO>> ObtenerPorEmpresa(Guid empresaId)
//    {
//        using var connection = _db.CrearConexion();

//        var sql = @"
//            SELECT
//                id,
//                nombre,
//                ubicacion
//            FROM almacenes
//            WHERE empresa_id = @EmpresaId
//            ORDER BY nombre;
//        ";

//        return await connection.QueryAsync<AlmacenDTO>(sql, new
//        {
//            EmpresaId = empresaId
//        });
//    }

//    public async Task<Guid> Crear(CrearAlmacenDTO dto)
//    {
//        using var connection = _db.CrearConexion();

//        // EXISTENTE: validaciones básicas
//        if (dto.EmpresaId == Guid.Empty)
//            throw new InvalidOperationException("EmpresaId es requerido.");

//        if (string.IsNullOrWhiteSpace(dto.Nombre))
//            throw new InvalidOperationException("El nombre del almacén es obligatorio.");

//        // NUEVO: validación multi-tenant (usuario → empresa)
//        // Gap detectado: CrearAlmacenDTO no tiene UsuarioId
//        // Se valida existencia de empresa como fallback
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

//        // NUEVO: normalización
//        var nombreNormalizado = dto.Nombre.Trim(); // evita duplicados con espacios
//        var ubicacionNormalizada = dto.Ubicacion?.Trim(); // opcional

//        // NUEVO: validación de nombre único por empresa (refuerza constraint DB)
//        var sqlValidarNombre = @"
//            SELECT COUNT(1)
//            FROM almacenes
//            WHERE empresa_id = @EmpresaId
//              AND nombre = @Nombre;
//        ";

//        var existe = await connection.ExecuteScalarAsync<int>(sqlValidarNombre, new
//        {
//            dto.EmpresaId,
//            Nombre = nombreNormalizado
//        });

//        if (existe > 0)
//            throw new InvalidOperationException("Ya existe un almacén con ese nombre en la empresa."); // NUEVO

//        var sql = @"
//            INSERT INTO almacenes (
//                empresa_id,
//                nombre,
//                ubicacion
//            )
//            VALUES (
//                @EmpresaId,
//                @Nombre,
//                @Ubicacion
//            )
//            RETURNING id;
//        ";

//        var id = await connection.ExecuteScalarAsync<Guid>(sql, new
//        {
//            dto.EmpresaId,
//            Nombre = nombreNormalizado,
//            Ubicacion = ubicacionNormalizada
//        });

//        return id;
//    }
//}

