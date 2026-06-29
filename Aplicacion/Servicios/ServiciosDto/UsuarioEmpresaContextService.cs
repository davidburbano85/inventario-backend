using inventarioWebAI.Aplicacion.Interfaces.Context;
using inventarioWebAI.Aplicacion.Interfaces.IPermmisoServicios;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;

public class UsuarioEmpresaContextService : IUsuarioEmpresaContextService
{
    private readonly IUsuarioContext _usuarioContext;
    private readonly IUsuarioEmpresaRepositorio _usuarioEmpresaRepositorio;
    private readonly IPermisoServicio _permisoServicio;

    public UsuarioEmpresaContextService(
        IUsuarioContext usuarioContext,
        IUsuarioEmpresaRepositorio usuarioEmpresaRepositorio,
        IPermisoServicio permisoServicio)
    {
        _usuarioContext = usuarioContext;
        _usuarioEmpresaRepositorio = usuarioEmpresaRepositorio;
        _permisoServicio = permisoServicio;
    }

    public async Task<UsuarioEmpresaContext> GetAsync()
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        if (usuarioId == Guid.Empty)
            throw new InvalidOperationException("Usuario inválido");

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null || !usuarioEmpresa.Activo)
            throw new InvalidOperationException("No hay empresa activa");

        var empresaId = usuarioEmpresa.EmpresaId;

        var esSuperAdmin = await _permisoServicio.EsSuperAdminAsync(usuarioId, empresaId);
        var esAdmin = await _permisoServicio.EsAdminAsync(usuarioId, empresaId);

        return new UsuarioEmpresaContext
        {
            UsuarioId = usuarioId,
            EmpresaId = empresaId,
            EsAdmin = esAdmin,
            EsSuperAdmin = esSuperAdmin
        };
    }
}