//// Ubicación: /src/Aplicacion/Servicios/ClienteServicio.cs

//using Dapper;
//using inventarioWebAI.Aplicacion.DTOs;
//using inventarioWebAI.Aplicacion.Interfaces;
//using inventarioWebAI.Infraestructura.conexion;

//namespace inventarioWebAI.Aplicacion.Servicios;

//// NUEVO: implementación de servicio de clientes
//// POR QUÉ:
//// - Mantener consistencia con patrón existente
//// - Multi-tenant basado en empresa_id
//public class ClienteServicio : IClienteServicio
//{
//    private readonly DbConnectionFactory _db;

//    public ClienteServicio(DbConnectionFactory db)
//    {
//        _db = db;
//    }

//    public async Task<IEnumerable<ClienteDTO>> ObtenerPorEmpresa(Guid empresaId)
//    {
//        using var connection = _db.CrearConexion();

//        var sql = @"
//            SELECT
//                id,
//                nombre,
//                contacto
//            FROM clientes
//            WHERE empresa_id = @EmpresaId
//            ORDER BY nombre;
//        ";

//        return await connection.QueryAsync<ClienteDTO>(sql, new
//        {
//            EmpresaId = empresaId
//        });
//    }

//    public async Task<Guid> Crear(CrearClienteDTO dto)
//    {
//        using var connection = _db.CrearConexion();

//        // EXISTENTE: validaciones básicas
//        if (dto.EmpresaId == Guid.Empty)
//            throw new InvalidOperationException("EmpresaId es requerido.");

//        if (string.IsNullOrWhiteSpace(dto.Nombre))
//            throw new InvalidOperationException("El nombre del cliente es obligatorio.");

//        // NUEVO: validación multi-tenant (fallback)
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
//        var nombreNormalizado = dto.Nombre.Trim();
//        var contactoNormalizado = dto.Contacto?.Trim();

//        // NUEVO: validación de nombre único por empresa
//        var sqlValidarNombre = @"
//            SELECT COUNT(1)
//            FROM clientes
//            WHERE empresa_id = @EmpresaId
//              AND nombre = @Nombre;
//        ";

//        var existe = await connection.ExecuteScalarAsync<int>(sqlValidarNombre, new
//        {
//            dto.EmpresaId,
//            Nombre = nombreNormalizado
//        });

//        if (existe > 0)
//            throw new InvalidOperationException("Ya existe un cliente con ese nombre en la empresa."); // NUEVO

//        var sql = @"
//            INSERT INTO clientes (
//                empresa_id,
//                nombre,
//                contacto
//            )
//            VALUES (
//                @EmpresaId,
//                @Nombre,
//                @Contacto
//            )
//            RETURNING id;
//        ";

//        var id = await connection.ExecuteScalarAsync<Guid>(sql, new
//        {
//            dto.EmpresaId,
//            Nombre = nombreNormalizado,
//            Contacto = contactoNormalizado
//        });

//        return id;
//    }
//}