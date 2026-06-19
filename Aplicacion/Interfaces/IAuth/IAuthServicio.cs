using inventarioWebAI.Aplicacion.DTOs;
using inventarioWebAI.Aplicacion.DTOs.AuthDTO;

namespace inventarioWebAI.Aplicacion.Interfaces.IAuth
{
    public interface IAuthServicio
    {
        Task<AuthRespuestasDto> LoginAsync(string email, string password);// Método para iniciar sesión y obtener un token de acceso
        Task<AuthRespuestasDto> SignupAsync(string email, string password);// Método para registrar un nuevo usuario y obtener un token de acceso
        Task<AuthRespuestasDto> RefreshTokenAsync(string refreshToken);// Método para refrescar el token de acceso utilizando un token de actualización
    }
}
