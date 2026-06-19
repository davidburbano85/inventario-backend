using inventarioWebAI.Aplicacion.Enums;

namespace inventarioWebAI.Aplicacion.DTOs.UsuarioEmpresa
{
    public class CrearUsuarioEmpresaRequest
    {
        public Guid EmpresaId { get; set; }
        public Guid UsuarioId { get; set; }
        public RolUsuarioEmpresaDTO Rol { get; set; }
    }
}
