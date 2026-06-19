using Dapper;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Dominio.Entidades;
using inventarioWebAI.Dominio.Enums;
using inventarioWebAI.Infraestructura.AccesoDatos;
using inventarioWebAI.Infraestructura.Mapper;
using inventarioWebAI.Infraestructura.RepositoriosDapper.Models;

namespace inventarioWebAI.Infraestructura.RepositoriosDapper;

public class UsuarioEmpresaRepositorioDapper : IUsuarioEmpresaRepositorio
{
    private readonly IDbConnectionFactory _db;

    public UsuarioEmpresaRepositorioDapper(IDbConnectionFactory db)
    {
        _db = db;
    }

    // -------------------------
    // CREAR
    // -------------------------
    public async Task<Guid> CrearAsync(Guid empresaId, Guid usuarioId, RolUsuarioEmpresa rol)
    {
        Console.WriteLine("[CrearAsync] INICIO");

        try
        {
            using var conn = _db.CrearConexion();
            Console.WriteLine("[CrearAsync] Conexión abierta");

            var sql = @"
                INSERT INTO usuarios_empresas (empresa_id, usuario_id, rol, created_at)
                VALUES (@EmpresaId, @UsuarioId, @Rol, timezone('America/Bogota', now()))
                RETURNING id;";

            var rolDb = RolUsuarioEmpresaMapper.ToDb(rol);
            Console.WriteLine($"[CrearAsync] Rol DB: {rolDb}");

            var result = await conn.ExecuteScalarAsync<Guid>(sql, new
            {
                EmpresaId = empresaId,
                UsuarioId = usuarioId,
                Rol = rolDb
            });

            Console.WriteLine($"[CrearAsync] RESULTADO: {result}");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CrearAsync] ERROR: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }

    // -------------------------
    // OBTENER POR ID
    // -------------------------
    public async Task<UsuarioEmpresa?> ObtenerPorIdAsync(Guid id)
    {
        Console.WriteLine("[ObtenerPorId] INICIO");

        try
        {
            using var conn = _db.CrearConexion();
            Console.WriteLine("[ObtenerPorId] Conexión abierta");

            var sql = @"
                SELECT id, empresa_id, usuario_id, rol, created_at, updated_at
                FROM usuarios_empresas
                WHERE id = @Id;";

            Console.WriteLine("[ObtenerPorId] Ejecutando query");

            var x = await conn.QueryFirstOrDefaultAsync<UsuarioEmpresaDb>(sql, new { Id = id });

            Console.WriteLine(x == null
                ? "[ObtenerPorId] RESULTADO: NULL"
                : "[ObtenerPorId] RESULTADO: ENCONTRADO");

            if (x == null) return null;

            Console.WriteLine("[ObtenerPorId] Mapeando entidad");

            var entity = new UsuarioEmpresa
            {
                Id = x.Id,
                EmpresaId = x.Empresa_Id,
                UsuarioId = x.Usuario_Id,
                Rol = RolUsuarioEmpresaMapper.ToDomain(x.Rol),
                CreatedAt = x.Created_At,
                UpdatedAt = x.UpdatedAt
            };

            Console.WriteLine("[ObtenerPorId] FIN OK");

            return entity;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ObtenerPorId] ERROR: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }

    // -------------------------
    // POR EMPRESA
    // -------------------------
    public async Task<IEnumerable<UsuarioEmpresa>> ObtenerPorEmpresaUsuarioAsync(
     Guid? empresaId = null,
     Guid? usuarioId = null)
    {
        Console.WriteLine("[ObtenerPorEmpresaUsuario] INICIO");

        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
            SELECT id, empresa_id, usuario_id, rol, created_at, updated_at
            FROM usuarios_empresas
            WHERE (@EmpresaId IS NULL OR empresa_id = @EmpresaId)
              AND (@UsuarioId IS NULL OR usuario_id = @UsuarioId);";

            var rows = (await conn.QueryAsync<UsuarioEmpresaDb>(sql, new
            {
                EmpresaId = empresaId,
                UsuarioId = usuarioId
            })).ToList();

            Console.WriteLine($"[ObtenerPorEmpresaUsuario] FILAS: {rows.Count}");

            return rows.Select(x => new UsuarioEmpresa
            {
                Id = x.Id,
                EmpresaId = x.Empresa_Id,
                UsuarioId = x.Usuario_Id,
                Rol = RolUsuarioEmpresaMapper.ToDomain(x.Rol),
                CreatedAt = x.Created_At,
                UpdatedAt = x.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ObtenerPorEmpresaUsuario] ERROR: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }
    // -------------------------
    // ELIMINAR
    // -------------------------
    public async Task<bool> EliminarAsync(Guid id)
    {
        Console.WriteLine("[Eliminar] INICIO");

        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"DELETE FROM usuarios_empresas WHERE id = @Id";

            Console.WriteLine("[Eliminar] Ejecutando DELETE");

            var rows = await conn.ExecuteAsync(sql, new { Id = id });

            Console.WriteLine($"[Eliminar] FILAS AFECTADAS: {rows}");

            return rows > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Eliminar] ERROR: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }

    // -------------------------
    // NOMBRE USUARIO
    // -------------------------
    public async Task<string?> ObtenerNombreUsuarioAsync(Guid id)
    {

        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                SELECT u.nombre
                FROM usuarios u
                INNER JOIN usuarios_empresas ue ON ue.usuario_id = u.id
                WHERE ue.id = @Id;";

            var result = await conn.ExecuteScalarAsync<string?>(sql, new { Id = id });


            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ObtenerNombreUsuario] ERROR: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }

    // -------------------------
    // ACTUALIZAR ROL
    // -------------------------
    public async Task<bool> ActualizarRolAsync(Guid usuarioId, Guid empresaId, RolUsuarioEmpresa rol)
    {

        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                UPDATE usuarios_empresas
                SET rol = @Rol,
                    updated_at = timezone('America/Bogota', now())
                WHERE usuario_id = @UsuarioId AND empresa_id = @EmpresaId;";

            var rolDb = RolUsuarioEmpresaMapper.ToDb(rol);


            var rows = await conn.ExecuteAsync(sql, new
            {
                Rol = rolDb,
                UsuarioId = usuarioId,
                EmpresaId = empresaId
            });


            return rows > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ActualizarRol] ERROR: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }

    // -------------------------
    // CONTAR SUPERADMINS
    // -------------------------
    public async Task<int> ContarAdminsPorEmpresaAsync(Guid empresaId)
    {

        try
        {
            using var conn = _db.CrearConexion();

            var sql = @"
                SELECT COUNT(1)
                FROM usuarios_empresas
                WHERE empresa_id = @EmpresaId AND rol = @Rol;";

            var rolDb = RolUsuarioEmpresaMapper.ToDb(RolUsuarioEmpresa.SuperAdmin);


            var result = await conn.ExecuteScalarAsync<int>(sql, new
            {
                EmpresaId = empresaId,
                Rol = rolDb
            });


            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ContarAdmins] ERROR: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }

  

    public  async Task<UsuarioEmpresa> ObtenerPorUsuarioYEmpresaAsync(Guid usuarioId, Guid empresaId)
    {
        try
        {
            using var conn = _db.CrearConexion();


            var sql = @"
            SELECT
                id,
                empresa_id,
                usuario_id,
                rol,
                created_at,
                updated_at
            FROM usuarios_empresas
            WHERE usuario_id = @UsuarioId
              AND empresa_id = @EmpresaId
            LIMIT 1;";

            var row = await conn.QueryFirstOrDefaultAsync<UsuarioEmpresaDb>(
                sql,
                new
                {
                    UsuarioId = usuarioId,
                    EmpresaId = empresaId
                });

            if (row == null)
                return null;

            return new UsuarioEmpresa
            {
                Id = row.Id,
                EmpresaId = row.Empresa_Id,
                UsuarioId = row.Usuario_Id,
                Rol = RolUsuarioEmpresaMapper.ToDomain(row.Rol),
                CreatedAt = row.Created_At,
                UpdatedAt = row.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[ObtenerPorUsuarioYEmpresaAsync] ERROR: {ex.GetType().Name} - {ex.Message}");

            Console.WriteLine(ex.StackTrace);

            throw;
        }
    }
}