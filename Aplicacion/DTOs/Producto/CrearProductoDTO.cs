// Ubicación: /src/Aplicacion/DTOs/CrearProductoDTO.cs

namespace inventarioWebAI.Aplicacion.DTOs.Producto;

public class CrearProductoDTO
{
    public Guid EmpresaId { get; set; }

    public Guid CategoriaId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? CodigoSku { get; set; }

    public decimal PrecioVenta { get; set; }

    public decimal? PrecioCompra { get; set; }
}

