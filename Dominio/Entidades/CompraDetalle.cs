namespace inventarioWebAI.Dominio.Entidades;

public class CompraDetalle
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public Guid CompraId { get; set; }

    public Guid ProductoId { get; set; }
  

    public decimal Cantidad { get; set; }

    public decimal Precio { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
    public bool Activo { get; set; }
}