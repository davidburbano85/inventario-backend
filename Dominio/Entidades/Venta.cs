// Ubicación: /src/Dominio/Entidades/Venta.cs

namespace inventarioWebAI.Dominio.Entidades;

public class Venta
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public Guid? ClienteId { get; set; }

    public decimal Total { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<VentaDetalle> Detalles { get; set; } = new();
}