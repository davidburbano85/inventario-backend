using inventarioWebAI.Aplicacion.Enums;
using System.Text.Json.Serialization;

namespace inventarioWebAI.Aplicacion.DTOs.UsuarioEmpresa;

public class UsuarioEmpresaDTO
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid UsuarioId { get; set; }
    public RolUsuarioEmpresaDTO Rol { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }


}