using inventarioWebAI.Aplicacion.DTOs;
using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios
{
    public interface IUsuarioRepositorio
    {
        Task<int> CrearAsync(Usuario usuario);
        Task<Usuario?>ObtenerPorIdAsync(Guid idUsuario);
        Task<IEnumerable<Usuario>> ListarAsync();
        Task<Usuario?> ObtenerPorTelefonoAsync(string telefono);
        Task<bool> ExisteTelefonoAsync(string telefono);
        Task<bool> ActualizarAsync(Usuario usuario);
        Task<bool> EliminarAsync(Guid idUsuario);
        Task<Usuario?> ObtenerPorAuthUserIdAsync(Guid authUserId);
        Task<bool> ActualizarPorAuthAsync(Guid authId, Usuario usuario);

    }
}
