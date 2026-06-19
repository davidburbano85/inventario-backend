using inventarioWebAI.Dominio.Entidades;
using inventarioWebAI.Dominio.Enums;

namespace inventarioWebAI.Aplicacion.Interfaces.IPermmisoServicios
{
    public interface IPermisoServicio
    {
        Task<RolUsuarioEmpresa> ObtenerRolAsync(Guid usuarioId, Guid empresaId);
        Task<bool>EsAdminAsync(Guid usuarioId, Guid empresaId);
        Task<bool>EsSuperAdminAsync(Guid usuarioId, Guid empresaId);
        Task ValidarRolAsync(Guid usuarioId, Guid empresaId, params RolUsuarioEmpresa[] roles);
        public Task ValidarSuperAdminAsync(Guid usuarioId, Guid empresaId);
        public Task ValidarAdminOSuperAdminAsync(Guid usuarioId, Guid empresaId);

    }
}
