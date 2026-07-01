using inventarioWebAI.Dominio.Entidades;
using inventarioWebAI.Dominio.Enums;

namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios
{
    public interface IUsuarioEmpresaRepositorio
    {
        // Crear relación usuario - empresa
        Task<Guid> CrearAsync(Guid empresaId, Guid usuarioId, RolUsuarioEmpresa rol);

        // Obtener por Id de la relación
        Task<UsuarioEmpresa?> ObtenerPorIdAsync(Guid id);

        // Obtener relaciones filtrando por empresa y/o usuario
        Task<IEnumerable<UsuarioEmpresa>> ObtenerPorEmpresaUsuarioAsync(
            Guid? empresaId = null,
            Guid? usuarioId = null);

        // Eliminar relación
        Task<bool> EliminarAsync(Guid id);

        // Obtener nombre del usuario asociado a una relación
        Task<string?> ObtenerNombreUsuarioAsync(Guid id);

        // Actualizar rol
        Task<bool> ActualizarRolAsync(
            Guid usuarioId,
            Guid empresaId,
            RolUsuarioEmpresa rol);

        // Contar SuperAdmin de una empresa
        Task<int> ContarAdminsPorEmpresaAsync(Guid empresaId);

        // Obtener una relación usuario - empresa
        Task<UsuarioEmpresa?> ObtenerPorUsuarioYEmpresaAsync(
            Guid usuarioId,
            Guid empresaId);

        // Obtener empresa activa del usuario
        Task<UsuarioEmpresa?> ObtenerEmpresaActivaAsync(Guid usuarioId);

        // Desactivar todas las empresas activas del usuario
        Task DesactivarTodasAsync(Guid usuarioId);

        // Activar una empresa
        Task ActivarEmpresaAsync(Guid usuarioId, Guid empresaId);
    }
}