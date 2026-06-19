public class CrearVentaDetalleDTO
{
    public Guid EmpresaId { get; set; }
    public Guid ProductoId { get; set; }

    public decimal Cantidad { get; set; }

    public decimal Precio { get; set; }
}