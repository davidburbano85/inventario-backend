// Ubicación: /src/Dominio/Entidades/StockActual.cs

namespace inventarioWebAI.Dominio.Entidades;

public class StockActual
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public Guid ProductoId { get; set; }

    public Guid AlmacenId { get; set; }

    public decimal Cantidad { get; set; }

    public DateTime UpdatedAt { get; set; }
}