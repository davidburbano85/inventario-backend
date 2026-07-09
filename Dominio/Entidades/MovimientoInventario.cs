using inventarioWebAI.Dominio.Enums;

public class MovimientoInventario
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public Guid ProductoId { get; set; }

    public Guid AlmacenId { get; set; }

    public Guid? UsuarioId { get; set; }
    public string? Factura { get; set; }

    public TipoMovimiento Tipo { get; set; } 

    public decimal Cantidad { get; set; }

    public string? Motivo { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}