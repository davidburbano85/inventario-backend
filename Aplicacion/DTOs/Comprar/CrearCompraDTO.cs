using inventarioWebAI.Aplicacion.DTOs.CompraDetalle;

public class CrearCompraDTO
{
    public Guid ProveedorId { get; set; }
    public string Factura { get; set; } = string.Empty;

    public List<CrearCompraDetalleDTO> Detalles { get; set; } = new();
}