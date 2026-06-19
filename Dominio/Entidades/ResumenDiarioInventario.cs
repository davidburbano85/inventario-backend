// Ubicación: /src/Dominio/Entidades/ResumenDiarioInventario.cs

namespace inventarioWebAI.Dominio.Entidades;

public class ResumenDiarioInventario
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public Guid? ProductoId { get; set; }

    public DateOnly Fecha { get; set; }

    public decimal TotalVentas { get; set; }

    public decimal TotalCompras { get; set; }
    public DateTime UpdatedAt { get; set; }
}
