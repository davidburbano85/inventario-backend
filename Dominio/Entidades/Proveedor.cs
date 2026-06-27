// Ubicación: /src/Dominio/Entidades/Proveedor.cs

namespace inventarioWebAI.Dominio.Entidades;

public class Proveedor
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Contacto { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public bool Activo { get; set; } 
}