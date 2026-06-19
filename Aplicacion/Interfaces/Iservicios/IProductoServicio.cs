// Ubicación: /src/Aplicacion/Interfaces/IProductoServicio.cs

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

using inventarioWebAI.Aplicacion.DTOs.Producto;

public interface IProductoServicio
{
    Task<IEnumerable<ProductoDTO>> ObtenerPorEmpresa(Guid empresaId);

    Task<ProductoDTO?> ObtenerPorId(Guid empresaId, Guid productoId);

    Task<Guid> Crear(CrearProductoDTO dto);
}