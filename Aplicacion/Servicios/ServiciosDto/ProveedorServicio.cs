using inventarioWebAI.Aplicacion.DTOs.Proveedor;
using inventarioWebAI.Aplicacion.Interfaces.Context;
using inventarioWebAI.Aplicacion.Interfaces.IPermmisoServicios;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Servicios;

public class ProveedorServicio : IProveedorServicio
{
    private readonly IProveedorRepositorio _proveedorRepositorio;
    private readonly IUsuarioContext _usuarioContext;
    private readonly IUsuarioEmpresaRepositorio _usuarioEmpresaRepositorio;
    private readonly IEmpresaRepositorio _empresaRepositorio;
    private readonly IPermisoServicio _permisoServicio;

    public ProveedorServicio(
        IProveedorRepositorio proveedorRepositorio,
        IUsuarioContext usuarioContext,
        IUsuarioEmpresaRepositorio usuarioEmpresaRepositorio,
        IEmpresaRepositorio empresaRepositorio,
        IPermisoServicio permisoServicio)
    {
        _proveedorRepositorio = proveedorRepositorio;
        _usuarioContext = usuarioContext;
        _usuarioEmpresaRepositorio = usuarioEmpresaRepositorio;
        _empresaRepositorio = empresaRepositorio;
        _permisoServicio = permisoServicio;
    }

    public async Task<Guid> CrearAsync(string nombre, string? contacto)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new InvalidOperationException("El nombre es obligatorio.");

        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        if (usuarioId == Guid.Empty)
            throw new InvalidOperationException("Usuario inválido.");

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null || !usuarioEmpresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var proveedor = new Proveedor
        {
            EmpresaId = empresaId,
            Nombre = nombre.Trim(),
            Contacto = contacto
        };

        return await _proveedorRepositorio.CrearAsync(proveedor);
    }

    public async Task<IEnumerable<ProveedorDTO>> ObtenerPorEmpresaAsync()
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        if (usuarioId == Guid.Empty)
            throw new InvalidOperationException("Usuario inválido.");

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null || !usuarioEmpresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var proveedores = await _proveedorRepositorio.ObtenerPorEmpresaAsync(empresaId);

        if (proveedores == null)
            return Enumerable.Empty<ProveedorDTO>();

        return proveedores.Select(p => new ProveedorDTO
        {
            Id = p!.Id,
            EmpresaId = p.EmpresaId,
            Nombre = p.Nombre,
            Contacto = p.Contacto,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            Activo = p.Activo
        });
    }

    public async Task<ProveedorDTO?> ObtenerPorIdAsync(Guid proveedorId)
    {
        if (proveedorId == Guid.Empty)
            throw new InvalidOperationException("Proveedor inválido.");

        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null || !usuarioEmpresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var proveedor = await _proveedorRepositorio.ObtenerPorIdAsync(proveedorId, empresaId);

        if (proveedor == null)
            return null;

        return new ProveedorDTO
        {
            Id = proveedor.Id,
            EmpresaId = proveedor.EmpresaId,
            Nombre = proveedor.Nombre,
            Contacto = proveedor.Contacto,
            CreatedAt = proveedor.CreatedAt,
            UpdatedAt = proveedor.UpdatedAt,
            Activo = proveedor.Activo
        };
    }

    public async Task<bool> ActualizarAsync(Guid proveedorId, string nombre, string? contacto)
    {
        if (proveedorId == Guid.Empty)
            throw new InvalidOperationException("Proveedor inválido.");

        if (string.IsNullOrWhiteSpace(nombre))
            throw new InvalidOperationException("El nombre es obligatorio.");

        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null || !usuarioEmpresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var proveedorExistente = await _proveedorRepositorio.ObtenerPorIdAsync(proveedorId, empresaId);

        if (proveedorExistente == null)
            throw new InvalidOperationException("El proveedor no existe.");

        var proveedor = new Proveedor
        {
            Id = proveedorId,
            EmpresaId = empresaId,
            Nombre = nombre.Trim(),
            Contacto = contacto
        };

        return await _proveedorRepositorio.ActualizarAsync(proveedor);
    }

    public async Task<bool> EliminarAsync(Guid proveedorId)
    {
        if (proveedorId == Guid.Empty)
            throw new InvalidOperationException("Proveedor inválido.");

        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null || !usuarioEmpresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var proveedor = await _proveedorRepositorio.ObtenerPorIdAsync(proveedorId, empresaId);

        if (proveedor == null)
            return false;

        return await _proveedorRepositorio.EliminarAsync(proveedorId, empresaId);
    }
}