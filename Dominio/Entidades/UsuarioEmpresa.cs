using inventarioWebAI.Dominio.Enums;
namespace inventarioWebAI.Dominio.Entidades;

public class UsuarioEmpresa
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public Guid UsuarioId { get; set; }

    public RolUsuarioEmpresa Rol { get; set; } 

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}