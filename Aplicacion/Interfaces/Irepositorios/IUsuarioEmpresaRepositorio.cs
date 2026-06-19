using inventarioWebAI.Aplicacion.DTOs.UsuarioEmpresa;
using inventarioWebAI.Aplicacion.Enums;
using inventarioWebAI.Dominio.Entidades;
using inventarioWebAI.Dominio.Enums;

namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios
{
    public interface IUsuarioEmpresaRepositorio
    {
        Task<Guid> CrearAsync(Guid empresaId, Guid usuarioId, RolUsuarioEmpresa rol);

        Task<UsuarioEmpresa?> ObtenerPorIdAsync(Guid id);

        Task<IEnumerable<UsuarioEmpresa>> ObtenerPorEmpresaUsuarioAsync(Guid? usuarioId,Guid? empresaId);


        Task<bool> EliminarAsync(Guid id);

        Task<string?> ObtenerNombreUsuarioAsync(Guid id);
        Task<bool> ActualizarRolAsync(Guid usuarioId, Guid empresaId, RolUsuarioEmpresa rol);
        Task <int> ContarAdminsPorEmpresaAsync(Guid empresaId);
        Task<UsuarioEmpresa> ObtenerPorUsuarioYEmpresaAsync(Guid usuarioId, Guid empresaId);

    }
}
