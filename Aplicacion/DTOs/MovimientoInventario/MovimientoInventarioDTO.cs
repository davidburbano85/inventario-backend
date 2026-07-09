// Ubicación: /src/Aplicacion/DTOs/MovimientoInventarioDTO.cs

namespace inventarioWebAI.Aplicacion.DTOs.MovimientoUnitario;

public class MovimientoInventarioDTO
{
    public Guid Id { get; set; }

    public Guid ProductoId { get; set; }

    public Guid AlmacenId { get; set; }
    public Guid? UsuarioId { get; set; }
    public string? Factura { get; set; }

    public string Tipo { get; set; } = string.Empty;

    public decimal Cantidad { get; set; }

    public string? Motivo { get; set; }

    public DateTime CreatedAt { get; set; }
}