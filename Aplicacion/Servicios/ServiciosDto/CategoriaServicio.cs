using inventarioWebAI.Aplicacion.DTOs.Categoria;
using inventarioWebAI.Aplicacion.Interfaces.Context;
using inventarioWebAI.Aplicacion.Interfaces.IPermmisoServicios;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Servicios;

public class CategoriaServicio : ICategoriaServicio
{
    private readonly ICategoriaRepositorio _categoriaRepositorio;
    private readonly IPermisoServicio _permisoServicio;
    private readonly IUsuarioContext _usuarioContext;
    private readonly IUsuarioEmpresaRepositorio _usuarioEmpresaRepositorio;
    private readonly IEmpresaRepositorio _empresaRepositorio;

    public CategoriaServicio(
        ICategoriaRepositorio categoriaRepositorio,
        IPermisoServicio permisoServicio,
        IUsuarioContext usuarioContext,
        IUsuarioEmpresaRepositorio usuarioEmpresaRepositorio,
        IEmpresaRepositorio empresaRepositorio)
    {
        _categoriaRepositorio = categoriaRepositorio;
        _permisoServicio = permisoServicio;
        _usuarioContext = usuarioContext;
        _usuarioEmpresaRepositorio = usuarioEmpresaRepositorio;
        _empresaRepositorio = empresaRepositorio;
    }

    public async Task<Guid> CrearCategoriaAsync(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new InvalidOperationException("El nombre de la categoría es obligatorio.");

        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        if (usuarioId == Guid.Empty)
            throw new InvalidOperationException("Usuario inválido.");

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null)
            throw new InvalidOperationException("El usuario no tiene empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        var empresa = await _empresaRepositorio.ObtenerEmpresaPorIdAsync(empresaId);

        if (empresa == null)
            throw new InvalidOperationException("La empresa no existe.");

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var categoria = new Categoria
        {
            EmpresaId = empresaId,
            Nombre = nombre.Trim()
        };

        return await _categoriaRepositorio.CrearCategoriaAsync(categoria);
    }

    public async Task<IEnumerable<CategoriaDTO>> ObtenerCategoriasActivasPorEmpresaAsync()
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        if (usuarioId == Guid.Empty)
            throw new InvalidOperationException("Usuario inválido.");

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null || !usuarioEmpresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var categorias = await _categoriaRepositorio.ObtenerCategoriasActivasPorEmpresaAsync(empresaId);

        if (categorias == null)
            return Enumerable.Empty<CategoriaDTO>();

        return categorias.Select(c => new CategoriaDTO
        {
            Id = c!.Id,
            EmpresaId = c.EmpresaId,
            Nombre = c.Nombre,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            Activo = c.Activo
        });
    }

    public async Task<CategoriaDTO?> ObtenerCategoriaPorIdAsync(Guid categoriaId)
    {
        if (categoriaId == Guid.Empty)
            throw new InvalidOperationException("Id inválido.");

        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null)
            throw new InvalidOperationException("No hay empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var categoria = await _categoriaRepositorio.ObtenerCategoriaPorIdAsync(categoriaId, empresaId);

        if (categoria == null)
            return null;

        return new CategoriaDTO
        {
            Id = categoria.Id,
            EmpresaId = categoria.EmpresaId,
            Nombre = categoria.Nombre,
            CreatedAt = categoria.CreatedAt,
            UpdatedAt = categoria.UpdatedAt,
            Activo = categoria.Activo
        };
    }

    public async Task<bool> ActualizarCategoriaAsync(Guid categoriaId, string nombre)
    {
        if (categoriaId == Guid.Empty)
            throw new InvalidOperationException("Id inválido.");

        if (string.IsNullOrWhiteSpace(nombre))
            throw new InvalidOperationException("El nombre es obligatorio.");

        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null)
            throw new InvalidOperationException("No hay empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var categoriaExistente = await _categoriaRepositorio.ObtenerCategoriaPorIdAsync(categoriaId, empresaId);

        if (categoriaExistente == null)
            throw new InvalidOperationException("La categoría no existe.");

        var categoria = new Categoria
        {
            Id = categoriaId,
            EmpresaId = empresaId,
            Nombre = nombre.Trim()
        };

        return await _categoriaRepositorio.ActualizarCategoriaAsync(categoria);
    }

    public async Task<bool> EliminarCategoriaAsync(Guid categoriaId)
    {
        if (categoriaId == Guid.Empty)
            throw new InvalidOperationException("Id inválido.");

        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null)
            throw new InvalidOperationException("No hay empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var categoriaExistente = await _categoriaRepositorio.ObtenerCategoriaPorIdAsync(categoriaId, empresaId);

        if (categoriaExistente == null)
            return false;

        return await _categoriaRepositorio.EliminarCategoriaAsync(categoriaId, empresaId);
    }
}