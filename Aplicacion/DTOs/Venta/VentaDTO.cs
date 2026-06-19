// Ubicación: /src/Aplicacion/DTOs/VentaDTO.cs

namespace inventarioWebAI.Aplicacion.DTOs.Venta;

public class VentaDTO
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }

    public Guid? ClienteId { get; set; }

    public decimal Total { get; set; }

    public DateTime CreatedAt { get; set; }
}