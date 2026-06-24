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

        public Guid ObtenerEmpresaId()
        {
            var usuario = _httpcontextAccessor.HttpContext?.User;

            var empresaId = usuario?.FindFirst("empresaId")?.Value;

            if (string.IsNullOrEmpty(empresaId) || !Guid.TryParse(empresaId, out var id))
                throw new Exception("Empresa no válida en el token");

            return id;
        }
    }
}