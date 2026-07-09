using inventarioWebAI.Aplicacion.DTOs.Venta;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

public interface IVentasServicio
{
    Task<Guid> CrearAsync(Guid clienteId, List<CrearVentaDetalleDTO> detalles);

    Task<IEnumerable<VentaDTO>> ObtenerPorEmpresaAsync();

    Task<VentaDTO?> ObtenerPorIdAsync(Guid ventaId);

    Task<bool> AnularAsync(Guid ventaId);
    Task<VentaDTO?> EncontrarPorFacturaAsync(string factura);


}