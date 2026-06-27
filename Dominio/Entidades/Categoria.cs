// Ubicación: /src/Dominio/Entidades/Categoria.cs

namespace inventarioWebAI.Dominio.Entidades;

public class Categoria
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Activo { get; set; } = false;
}