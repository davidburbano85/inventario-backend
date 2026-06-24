// Ubicación: /src/Dominio/Entidades/Almacen.cs

namespace inventarioWebAI.Dominio.Entidades;

public class Almacen
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Ubicacion { get; set; }

    public DateTime CreatedAt { get; set; }
}