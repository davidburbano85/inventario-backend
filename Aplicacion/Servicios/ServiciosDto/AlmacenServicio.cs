using inventarioWebAI.Aplicacion.DTOs.Almacen;
using inventarioWebAI.Aplicacion.Interfaces;
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
    public async Task<IEnumerable<AlmacenDTO>> ObtenerAlmacenPorEmpresaAsync()
    {
        try
        {
            var empresaId = _usuarioContext.ObtenerEmpresaId();
            var usuarioId = _usuarioContext.ObtenerAuthUserId();

            if (empresaId == Guid.Empty)
                throw new InvalidOperationException("EmpresaId inválido.");

            if (usuarioId == Guid.Empty)
                throw new InvalidOperationException("UsuarioId inválido.");

            // 🔐 PERMISOS
            await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

            var almacenes = await _almacenRepositorio
                .ObtenerAlmacenPorEmpresaIdAsync(empresaId);

            return almacenes.Select(a => new AlmacenDTO
            {
                Id = a.Id,
                EmpresaId = a.EmpresaId,
                Nombre = a.Nombre,
                Ubicacion = a.Ubicacion,
                CreatedAt = a.CreatedAt
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ALMACEN SERVICE] Obtener ERROR: {ex.Message}");
            throw;
        }
    }
}