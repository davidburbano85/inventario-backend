using inventarioWebAI.Aplicacion.DTOs.Almacen;
using inventarioWebAI.Aplicacion.Interfaces.Context;
using inventarioWebAI.Aplicacion.Interfaces.IPermmisoServicios;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Servicios;

public class AlmacenServicio : IAlmacenServicio
{
    private readonly IAlmacenRepositorio _almacenRepositorio;
    private readonly IPermisoServicio _permisoServicio;
    private readonly IUsuarioContext _usuarioContext;
    private readonly IUsuarioEmpresaRepositorio _usuarioEmpresaRepositorio;
    private readonly IEmpresaRepositorio _empresaRepositorio;

    public AlmacenServicio(
        IAlmacenRepositorio almacenRepositorio,
        IPermisoServicio permisoServicio,
        IUsuarioContext usuarioContext,
        IUsuarioEmpresaRepositorio usuarioEmpresaRepositorio,
        IEmpresaRepositorio empresaRepositorio)
    {
        _almacenRepositorio = almacenRepositorio;
        _permisoServicio = permisoServicio;
        _usuarioContext = usuarioContext;
        _usuarioEmpresaRepositorio = usuarioEmpresaRepositorio;
        _empresaRepositorio = empresaRepositorio;
    }

    // ==============================
    // CREAR ALMACÉN
    // ==============================
    public async Task<Guid> CrearAlmacenAsync(Guid usuarioId, string nombre, string ubicacion)
    {
        if (usuarioId == Guid.Empty)
            throw new InvalidOperationException("El ID del usuario es inválido.");

        if (string.IsNullOrWhiteSpace(nombre))
            throw new InvalidOperationException("El nombre del almacén es obligatorio.");

        if (string.IsNullOrWhiteSpace(ubicacion))
            throw new InvalidOperationException("La ubicación del almacén es obligatoria.");

        // 1. Validar empresa activa del usuario
        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null)
            throw new InvalidOperationException("El usuario no tiene una empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        // 2. Validar que la empresa exista (consistencia de datos)
        var empresa = await _empresaRepositorio.ObtenerEmpresaPorIdAsync(empresaId);

        if (empresa == null)
            throw new InvalidOperationException("La empresa no existe.");

        // 3. Validar permisos (solo SuperAdmin)
        var esSuperAdmin = await _permisoServicio.EsSuperAdminAsync(usuarioId, empresaId);

        if (!esSuperAdmin)
            throw new UnauthorizedAccessException("Solo un SuperAdmin puede crear almacenes.");

        // 4. Crear entidad
        var almacen = new Almacen
        {
            EmpresaId = empresaId,
            Nombre = nombre.Trim(),
            Ubicacion = ubicacion.Trim()
        };

        // 5. Persistir
        var id = await _almacenRepositorio.CrearAlmacenAsync(almacen);

        return id;
    }



    // ==============================
    // OBTENER ALMACENES
    // ==============================
    public async Task<IEnumerable<AlmacenDTO>> ObtenerAlmacenesActivosPorEmpresaAsync()
    {
        try
        {
            var usuarioId = _usuarioContext.ObtenerAuthUserId();

            if (usuarioId == Guid.Empty)
                throw new InvalidOperationException("UsuarioId inválido.");

            var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

            if (usuarioEmpresa == null || usuarioEmpresa.EmpresaId == Guid.Empty || !usuarioEmpresa.Activo)
                throw new InvalidOperationException("No hay empresa activa para el usuario.");

            var empresaId = usuarioEmpresa.EmpresaId;

            await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

            var almacenes = await _almacenRepositorio.ObtenerAlmacenesActivosPorEmpresaAsync(empresaId);

            if (almacenes is null)
                return null;

            return almacenes.Select(almacen => new AlmacenDTO
            {
                Id = almacen.Id,
                EmpresaId = almacen.EmpresaId,
                Nombre = almacen.Nombre,
                Ubicacion = almacen.Ubicacion,
                CreatedAt = almacen.CreatedAt,
                Activo = almacen.Activo
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ALMACEN SERVICE] ObtenerAlmacenActivoPorEmpresaAsync ERROR: {ex.Message}");
            throw;
        }
    }


    // ==============================
    // ACTUALIZAR ALMACÉN
    // ==============================
    public async Task<bool> ActualizarAlmacenAsync(Guid almacenId, string nombre, string ubicacion)
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId(); // 🔥 ahora SIEMPRE desde contexto

        if (almacenId == Guid.Empty)
            throw new InvalidOperationException("El ID del almacén es inválido.");

        if (string.IsNullOrWhiteSpace(nombre))
            throw new InvalidOperationException("El nombre del almacén es obligatorio.");

        if (string.IsNullOrWhiteSpace(ubicacion))
            throw new InvalidOperationException("La ubicación del almacén es obligatoria.");

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null || !usuarioEmpresa.Activo)
            throw new InvalidOperationException("El usuario no tiene una empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        var empresa = await _empresaRepositorio.ObtenerEmpresaPorIdAsync(empresaId);

        if (empresa == null)
            throw new InvalidOperationException("La empresa no existe.");

        var esSuperAdmin = await _permisoServicio.EsSuperAdminAsync(usuarioId, empresaId);
        var esAdmin = await _permisoServicio.EsAdminAsync(usuarioId, empresaId);

        if (!esSuperAdmin && !esAdmin)
            throw new UnauthorizedAccessException("No tienes permisos para actualizar almacenes.");

        var almacenExistente = await _almacenRepositorio.ObtenerAlmacenPorIdAsync(almacenId, empresaId);

        if (almacenExistente == null)
            throw new InvalidOperationException("El almacén no existe o no pertenece a la empresa.");

        var almacen = new Almacen
        {
            Id = almacenId,
            EmpresaId = empresaId,
            Nombre = nombre.Trim(),
            Ubicacion = ubicacion.Trim()
        };

        return await _almacenRepositorio.ActualizarAlmacenAsync(almacen);
    }

    public async Task<AlmacenDTO?> ObtenerAlmacenPorIdAsync(Guid almacenId)
    {
        if (almacenId == Guid.Empty)
            throw new InvalidOperationException("El ID del almacén es inválido.");

        // 1. Obtener usuario autenticado desde JWT
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        if (usuarioId == Guid.Empty)
            throw new InvalidOperationException("Usuario no válido.");

        // 2. Obtener empresa activa del usuario (multitenant context)
        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null || !usuarioEmpresa.Activo)
            throw new InvalidOperationException("No hay empresa activa para el usuario.");

        var empresaId = usuarioEmpresa.EmpresaId;

        // 3. Validar que la empresa exista
        var empresa = await _empresaRepositorio.ObtenerEmpresaPorIdAsync(empresaId);

        if (empresa == null)
            throw new InvalidOperationException("La empresa no existe.");

        // 4. Validar permisos (Admin o SuperAdmin)
        var esSuperAdmin = await _permisoServicio.EsSuperAdminAsync(usuarioId, empresaId);
        var esAdmin = await _permisoServicio.EsAdminAsync(usuarioId, empresaId);

        if (!esSuperAdmin && !esAdmin)
            throw new UnauthorizedAccessException("No tienes permisos para ver almacenes.");

        // 5. Obtener almacén por ID dentro de la empresa
        var almacen = await _almacenRepositorio.ObtenerAlmacenPorIdAsync(almacenId, empresaId);

        if (almacen == null)
            return null;

        // 6. Mapear a DTO
        return new AlmacenDTO
        {
            Id = almacen.Id,
            EmpresaId = almacen.EmpresaId,
            Nombre = almacen.Nombre,
            Ubicacion = almacen.Ubicacion,
            CreatedAt = almacen.CreatedAt,
            Activo = almacen.Activo
        };
    }

    public async Task<bool> EliminarAlmacenAsync(Guid almacenId)
    {
        if (almacenId == Guid.Empty)
            throw new InvalidOperationException("El ID del almacén es inválido.");

        // 1. Obtener usuario autenticado
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        if (usuarioId == Guid.Empty)
            throw new InvalidOperationException("Usuario no válido.");

        // 2. Obtener empresa activa
        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null || !usuarioEmpresa.Activo)
            throw new InvalidOperationException("No hay empresa activa para el usuario.");

        var empresaId = usuarioEmpresa.EmpresaId;

        // 3. Validar empresa
        var empresa = await _empresaRepositorio.ObtenerEmpresaPorIdAsync(empresaId);

        if (empresa == null)
            throw new InvalidOperationException("La empresa no existe.");

        // 4. Validar permisos
        var esSuperAdmin = await _permisoServicio.EsSuperAdminAsync(usuarioId, empresaId);
        var esAdmin = await _permisoServicio.EsAdminAsync(usuarioId, empresaId);

        if (!esSuperAdmin && !esAdmin)
            throw new UnauthorizedAccessException("No tienes permisos para eliminar almacenes.");

        // 5. Verificar que el almacén exista y pertenezca a la empresa
        var almacen = await _almacenRepositorio.ObtenerAlmacenPorIdAsync(almacenId, empresaId);

        if (almacen == null)
            return false;

        // 6. Eliminar
        return await _almacenRepositorio.EliminarAlmacenAsync(almacenId, empresaId);
    }
}