using inventarioWebAI.Aplicacion.DTOs.Usuario;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios
{
    public interface IUsuarioServicio
    {
        Task<UsuarioDTO> CrearAsync(UsuarioDTO dto);

        Task<UsuarioDTO?> ObtenerPorIdAsync(Guid idUsuario);
        Task<UsuarioDTO> ActualizarAsync(UsuarioDTO dto);
        Task<UsuarioDTO> EliminarAsync(Guid idUsuario);

        Task<IEnumerable<UsuarioDTO>> ListarAsync();

        Task<UsuarioDTO> ObtenerPorAuthUserIdAsync(Guid authUserId);
        Task<UsuarioDTO?> ObtenerPorTelefonoAsync(string telefono);
        Task<bool> ActualizarPorAuthAsync(Guid authId, UsuarioDTO dto);

    }
}
