using inventarioWebAI.Aplicacion.Interfaces.IPermmisoServicios;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Dominio.Enums;

namespace inventarioWebAI.Aplicacion.Servicios.PermisoServicios
{
    public class PermisoServicio : IPermisoServicio
    {
        private readonly IUsuarioEmpresaRepositorio _repo;
        public PermisoServicio(IUsuarioEmpresaRepositorio repo)
        {
            _repo = repo;
        }

        public async Task<bool> EsAdminAsync(Guid usuarioId, Guid empresaId)// si el usuario es admin o superadmin, ambos tienen permisos de admin  
        {
            var rol =  await ObtenerRolAsync(usuarioId, empresaId);// Obtener el rol del usuario en la empresa
            return rol == RolUsuarioEmpresa.Admin || rol == RolUsuarioEmpresa.SuperAdmin;// Si el rol es Admin o SuperAdmin, entonces el usuario es considerado Admin

        }

        public async Task<bool> EsSuperAdminAsync(Guid usuarioId, Guid empresaId)
        {
           var rol = await ObtenerRolAsync(usuarioId, empresaId);// Obtener el rol del usuario en la empresa
            return rol == RolUsuarioEmpresa.SuperAdmin;// Si el rol es SuperAdmin, entonces el usuario es considerado SuperAdmin
        }



        public async Task<RolUsuarioEmpresa> ObtenerRolAsync(Guid usuarioId, Guid empresaId)
        {
            var relacion = await _repo.ObtenerPorUsuarioYEmpresaAsync(usuarioId, empresaId);// Obtener la relación entre el usuario y la empresa
           if (relacion == null) 
                throw new UnauthorizedAccessException("El usuario no tiene permisos para acceder a esta empresa.");// Si no existe la relación, se lanza una excepción indicando que el usuario no tiene permisos para acceder a la empresa

            return relacion.Rol ;// Si no existe la relación, se considera que el usuario tiene el rol de Usuario por defecto
        }




        public async Task ValidarRolAsync(Guid usuarioId, Guid empresaId, params RolUsuarioEmpresa[] roles)
        {
            var rol = await ObtenerRolAsync(usuarioId, empresaId);// Obtener el rol del usuario en la empresa
            if (!roles.Contains(rol))// Si el rol del usuario no está en la lista de roles permitidos, entonces se lanza una excepción
            {
                throw new UnauthorizedAccessException("El usuario no tiene permisos para realizar esta acción.");// Lanzar una excepción indicando que el usuario no tiene permisos
            }
        }


        public Task ValidarSuperAdminAsync(Guid usuarioId, Guid empresaId)
        {
            return ValidarRolAsync(
                usuarioId,
                empresaId,
                RolUsuarioEmpresa.SuperAdmin);
        }

        public Task ValidarAdminOSuperAdminAsync(Guid usuarioId, Guid empresaId)
        {
            return ValidarRolAsync(
                usuarioId,
                empresaId,
                RolUsuarioEmpresa.Admin,
                RolUsuarioEmpresa.SuperAdmin);
        }

    }
}
