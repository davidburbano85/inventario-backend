// Ubicación: /src/Dominio/Entidades/Compra.cs

namespace inventarioWebAI.Dominio.Entidades;

public class Compra
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public Guid? ProveedorId { get; set; }

    public decimal Total { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<CompraDetalle> Detalles { get; set; } = new();
}