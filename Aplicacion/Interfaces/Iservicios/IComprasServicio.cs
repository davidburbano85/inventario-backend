using inventarioWebAI.Aplicacion.DTOs.CompraDetalle;
using inventarioWebAI.Aplicacion.DTOs.Comprar;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

public interface IComprasServicio
{
    Task<Guid> CrearAsync(Guid proveedorId, List<CrearCompraDetalleDTO> detalles);

    Task<IEnumerable<CompraDTO>> ObtenerPorEmpresaAsync();

    Task<CompraDTO?> ObtenerPorIdAsync(Guid compraId);

    Task<IEnumerable<CompraDetalleDTO>> ObtenerDetalleAsync(Guid compraId);

    Task<bool> AnularAsync(Guid compraId);
}