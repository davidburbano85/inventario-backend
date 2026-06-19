//// Ubicación: /src/Aplicacion/Servicios/CategoriaServicio.cs

//using Dapper;
//using inventarioWebAI.Aplicacion.DTOs;
//using inventarioWebAI.Aplicacion.Interfaces;
//using inventarioWebAI.Infraestructura.conexion;

//namespace inventarioWebAI.Aplicacion.Servicios;

//// NUEVO: implementación de servicio de categorías
//// POR QUÉ:
//// - Mantener consistencia con patrón existente
//// - Soportar multi-tenant
//public class CategoriaServicio : ICategoriaServicio
//{
//    private readonly DbConnectionFactory _db;

//    public CategoriaServicio(DbConnectionFactory db)
//    {
//        _db = db;
//    }

//    public async Task<IEnumerable<CategoriaDTO>> ObtenerPorEmpresa(Guid empresaId)
//    {
//        using var connection = _db.CrearConexion();

//        var sql = @"
//            SELECT
//                id,
//                nombre
//            FROM categorias
//            WHERE empresa_id = @EmpresaId
//            ORDER BY nombre;
//        ";

//        return await connection.QueryAsync<CategoriaDTO>(sql, new
//        {
//            EmpresaId = empresaId
//        });
//    }

//    public async Task<Guid> Crear(CrearCategoriaDTO dto)
//    {
//        using var connection = _db.CrearConexion();

//        // EXISTENTE: validaciones básicas
//        if (dto.EmpresaId == Guid.Empty)
//            throw new InvalidOperationException("EmpresaId es requerido.");

//        if (string.IsNullOrWhiteSpace(dto.Nombre))
//            throw new InvalidOperationException("El nombre de la categoría es obligatorio.");

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

//        // NUEVO: validación de nombre único por empresa
//        var sqlValidarNombre = @"
//            SELECT COUNT(1)
//            FROM categorias
//            WHERE empresa_id = @EmpresaId
//              AND nombre = @Nombre;
//        ";

//        var existe = await connection.ExecuteScalarAsync<int>(sqlValidarNombre, new
//        {
//            dto.EmpresaId,
//            Nombre = nombreNormalizado
//        });

//        if (existe > 0)
//            throw new InvalidOperationException("Ya existe una categoría con ese nombre en la empresa."); // NUEVO

//        var sql = @"
//            INSERT INTO categorias (
//                empresa_id,
//                nombre
//            )
//            VALUES (
//                @EmpresaId,
//                @Nombre
//            )
//            RETURNING id;
//        ";

//        var id = await connection.ExecuteScalarAsync<Guid>(sql, new
//        {
//            dto.EmpresaId,
//            Nombre = nombreNormalizado
//        });

//        return id;
//    }
//}