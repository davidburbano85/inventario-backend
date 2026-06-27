using inventarioWebAI.Aplicacion.DTOs.Producto;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

public interface IProductoServicio
{
    Task<Guid> CrearProductoAsync(string nombre, string? codigoSku, decimal precioVenta, decimal? precioCompra, Guid categoriaId);

    Task<IEnumerable<ProductoDTO>> ObtenerProductosActivosPorEmpresaAsync();

    Task<IEnumerable<ProductoDTO>> ObtenerProductosPorCategoriaAsync(Guid categoriaId);

    Task<ProductoDTO?> ObtenerProductoPorIdAsync(Guid productoId);

    Task<bool> ActualizarProductoAsync(Guid productoId, string nombre, string? codigoSku, decimal precioVenta, decimal? precioCompra, Guid categoriaId);

    Task<bool> EliminarProductoAsync(Guid productoId);
}