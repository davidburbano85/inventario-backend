using inventarioWebAI.Aplicacion.DTOs.Almacen;
using inventarioWebAI.Aplicacion.Interfaces.Context;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Dominio.Entidades;
using Microsoft.Extensions.Caching.Memory;

namespace inventarioWebAI.Aplicacion.Servicios;

public class AlmacenServicio : IAlmacenServicio
{
    private readonly IAlmacenRepositorio _almacenRepositorio;
    private readonly IUsuarioEmpresaContextService _contextService;
    private readonly IUsuarioContext _usuarioContext;
    private readonly ILogger<AlmacenServicio> _logger;
    private readonly IMemoryCache _cache;

    public AlmacenServicio(
        IAlmacenRepositorio almacenRepositorio,
        IUsuarioEmpresaContextService contextService,
        IUsuarioContext usuarioContext,
        ILogger<AlmacenServicio> logger,
        IMemoryCache cache)
    {
        _almacenRepositorio = almacenRepositorio;
        _contextService = contextService;
        _usuarioContext = usuarioContext;
        _logger = logger;
        _cache = cache;
    }

    // =====================================================
    // CREAR ALMACÉN
    // =====================================================
    public async Task<Guid> CrearAlmacenAsync(Guid usuarioId, string nombre, string ubicacion)
    {
        if (usuarioId == Guid.Empty)
            throw new InvalidOperationException("Usuario inválido.");

        if (string.IsNullOrWhiteSpace(nombre))
            throw new InvalidOperationException("Nombre obligatorio.");

        if (string.IsNullOrWhiteSpace(ubicacion))
            throw new InvalidOperationException("Ubicación obligatoria.");

        var ctx = await _contextService.GetAsync();

        if (!ctx.EsSuperAdmin)
            throw new UnauthorizedAccessException("Solo SuperAdmin puede crear almacenes.");

        var almacen = new Almacen
        {
            EmpresaId = ctx.EmpresaId,
            Nombre = nombre.Trim(),
            Ubicacion = ubicacion.Trim()
        };

        return await _almacenRepositorio.CrearAlmacenAsync(almacen);
    }

    // =====================================================
    // OBTENER ALMACENES
    // =====================================================
    public async Task<IEnumerable<AlmacenDTO>> ObtenerAlmacenesActivosPorEmpresaAsync()
    {
        var ctx = await _contextService.GetAsync();

        var almacenes = await _almacenRepositorio
            .ObtenerAlmacenesActivosPorEmpresaAsync(ctx.EmpresaId);

        return almacenes.Select(a => new AlmacenDTO
        {
            Id = a.Id,
            EmpresaId = a.EmpresaId,
            Nombre = a.Nombre,
            Ubicacion = a.Ubicacion,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt,
            Activo = a.Activo
        });
    }

    // =====================================================
    // ACTUALIZAR ALMACÉN
    // =====================================================
    public async Task<bool> ActualizarAlmacenAsync(Guid almacenId, string nombre, string ubicacion)
    {
        if (almacenId == Guid.Empty)
            throw new InvalidOperationException("ID inválido.");

        if (string.IsNullOrWhiteSpace(nombre))
            throw new InvalidOperationException("Nombre obligatorio.");

        if (string.IsNullOrWhiteSpace(ubicacion))
            throw new InvalidOperationException("Ubicación obligatoria.");

        var ctx = await _contextService.GetAsync();

        if (!ctx.EsAdmin && !ctx.EsSuperAdmin)
            throw new UnauthorizedAccessException("Sin permisos.");

        var almacenExistente = await _almacenRepositorio
            .ObtenerAlmacenPorIdAsync(almacenId, ctx.EmpresaId);

        if (almacenExistente == null)
            throw new InvalidOperationException("No existe almacén.");

        var almacen = new Almacen
        {
            Id = almacenId,
            EmpresaId = ctx.EmpresaId,
            Nombre = nombre.Trim(),
            Ubicacion = ubicacion.Trim()
        };

        return await _almacenRepositorio.ActualizarAlmacenAsync(almacen);
    }

    // =====================================================
    // OBTENER POR ID
    // =====================================================
    public async Task<AlmacenDTO?> ObtenerAlmacenPorIdAsync(Guid almacenId)
    {
        if (almacenId == Guid.Empty)
            throw new InvalidOperationException("ID inválido.");

        var ctx = await _contextService.GetAsync();

        if (!ctx.EsAdmin && !ctx.EsSuperAdmin)
            throw new UnauthorizedAccessException("Sin permisos.");

        var almacen = await _almacenRepositorio
            .ObtenerAlmacenPorIdAsync(almacenId, ctx.EmpresaId);

        if (almacen == null)
            return null;

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

    // =====================================================
    // ELIMINAR
    // =====================================================
    public async Task<bool> EliminarAlmacenAsync(Guid almacenId)
    {
        if (almacenId == Guid.Empty)
            throw new InvalidOperationException("ID inválido.");

        var ctx = await _contextService.GetAsync();

        if (!ctx.EsAdmin && !ctx.EsSuperAdmin)
            throw new UnauthorizedAccessException("Sin permisos.");

        var almacen = await _almacenRepositorio
            .ObtenerAlmacenPorIdAsync(almacenId, ctx.EmpresaId);

        if (almacen == null)
            return false;

        return await _almacenRepositorio
            .EliminarAlmacenAsync(almacenId, ctx.EmpresaId);
    }

    // =====================================================
    // SELECCIONAR ALMACÉN
    // =====================================================
    public async Task SeleccionarAlmacenAsync(Guid almacenId)
    {
        var ctx = await _contextService.GetAsync();

        var almacen = await _almacenRepositorio
            .ObtenerAlmacenPorIdAsync(almacenId, ctx.EmpresaId);

        if (almacen == null)
            throw new InvalidOperationException("No pertenece a empresa.");

        var key = $"almacen:{ctx.UsuarioId}";

        _cache.Set(key, almacenId, TimeSpan.FromHours(8));

        _logger.LogInformation("Almacén guardado: {UsuarioId} -> {AlmacenId}",
            ctx.UsuarioId, almacenId);
    }

    // =====================================================
    // OBTENER ALMACÉN ACTIVO
    // =====================================================
    public Task<Guid> ObtenerAlmacenActivoAsync()
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();
        var key = $"almacen:{usuarioId}";

        if (!_cache.TryGetValue(key, out Guid almacenId))
            throw new InvalidOperationException("No hay almacén seleccionado.");

        return Task.FromResult(almacenId);
    }
}