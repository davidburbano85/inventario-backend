using inventarioWebAI.Aplicacion.DTOs.CompraDetalle;

public class CrearCompraDTO
{
    public Guid ProveedorId { get; set; }

    public List<CrearCompraDetalleDTO> Detalles { get; set; } = new();
}