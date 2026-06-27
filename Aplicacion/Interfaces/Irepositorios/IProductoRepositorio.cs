
namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios;

public interface IProductoRepositorio
{
    Task<Guid> CrearProductoAsync(Producto producto);

    Task<IEnumerable<Producto?>> ObtenerProductosActivosPorEmpresaAsync(Guid empresaId);

    Task<IEnumerable<Producto?>> ObtenerProductosPorCategoriaAsync(Guid empresaId, Guid categoriaId);

    Task<Producto?> ObtenerProductoPorIdAsync(Guid id, Guid empresaId);

    Task<bool> ActualizarProductoAsync(Producto producto);

    Task<bool> EliminarProductoAsync(Guid id, Guid empresaId);
}
