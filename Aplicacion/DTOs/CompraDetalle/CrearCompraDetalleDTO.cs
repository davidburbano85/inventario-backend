// Ubicación: /src/Aplicacion/DTOs/CrearCompraDetalleDTO.cs

namespace inventarioWebAI.Aplicacion.DTOs.CompraDetalle;

public class CrearCompraDetalleDTO
{
    public Guid ProductoId { get; set; }

    public decimal Cantidad { get; set; }

    public decimal Precio { get; set; }
}