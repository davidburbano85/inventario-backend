// Ubicación: /src/Aplicacion/DTOs/ProductoDTO.cs

namespace inventarioWebAI.Aplicacion.DTOs.Producto;

public class ProductoDTO
{
    public Guid Id { get; set; }
    public Guid? CategoriaId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? CodigoSku { get; set; }

    public decimal PrecioVenta { get; set; }

    public decimal? PrecioCompra { get; set; }

    public bool Activo { get; set; }
}