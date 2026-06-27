public class Producto
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public Guid? CategoriaId { get; set; } // FK opcional según diseño DB

    public string Nombre { get; set; } = string.Empty;

    public string? CodigoSku { get; set; }

    public decimal PrecioVenta { get; set; }

    public decimal? PrecioCompra { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}