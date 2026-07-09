namespace inventarioWebAI.Dominio.Entidades;

public class VentaDetalle
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public Guid VentaId { get; set; }

    public Guid ProductoId { get; set; }

    public decimal Cantidad { get; set; }

    public decimal Precio { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool Activo { get; set; }

}