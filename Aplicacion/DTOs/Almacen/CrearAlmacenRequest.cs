using inventarioWebAI.Aplicacion.Enums;

namespace inventarioWebAI.Aplicacion.DTOs.Almacen
{
    public class CrearAlmacenRequest
    {
        public Guid EmpresaId { get; set; }
        public Guid UsuarioId { get; set; }
        public RolUsuarioEmpresaDTO Rol { get; set; }
    }
}
