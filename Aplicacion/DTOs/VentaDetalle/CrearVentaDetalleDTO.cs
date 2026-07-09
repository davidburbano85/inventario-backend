public class CrearVentaDetalleDTO
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid ProductoId { get; set; }

    public decimal Cantidad { get; set; }

    public decimal Precio { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}