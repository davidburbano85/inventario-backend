using inventarioWebAI.Aplicacion.Interfaces.Context;
using System.Security.Claims;

namespace inventarioWebAI.Infraestructura.Context
{
    public class UsuarioContextService : IUsuarioContext
    {
        private readonly IHttpContextAccessor _httpcontextAccessor;

        public UsuarioContextService(IHttpContextAccessor httpcontextAccessor)
        {
            _httpcontextAccessor = httpcontextAccessor;
        }

        public Guid ObtenerAuthUserId()
        {
            var usuario = _httpcontextAccessor.HttpContext?.User;

            var sub = usuario?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? usuario?.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
                throw new Exception("Usuario no autenticado o ID inválido");

            return userId;
        }


    }
}