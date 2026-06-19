using inventarioWebAI.Aplicacion.Interfaces.Context;
using System.Security.Claims; // Permite leer claims del JWT

namespace inventarioWebAI.Infraestructura.Context
{
    public class UsuarioContextService: IUsuarioContext
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
                ?? usuario?.FindFirst("sub")?.Value; // Intentar también con "sub" si no se encuentra "NameIdentifier"
            if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
            
                throw new Exception("Usuario no autenticado o ID de usuario inválido en claims");
            return Guid.Parse(sub);
        }
    }
}
