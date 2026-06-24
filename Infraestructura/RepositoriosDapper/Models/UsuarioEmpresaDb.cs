namespace inventarioWebAI.Infraestructura.RepositoriosDapper.Models;

public class UsuarioEmpresaDb
{
    public Guid Id { get; set; }
    public Guid Empresa_Id { get; set; }
    public Guid Usuario_Id { get; set; }
    public string Rol { get; set; } = default!;
    public DateTime Created_At { get; set; }
    public bool Activo { get; set; }
    public DateTime UpdatedAt { get; set; }
}