namespace inventarioWebAI.Dominio.Entidades;

public class Compra
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public Guid? ProveedorId { get; set; }

    public decimal Total { get; set; }
    public string Factura { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public bool Activo { get; set; } = true;
}