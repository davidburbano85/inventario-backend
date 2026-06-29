using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

public interface IVentasServicio
{
    Task<Guid> CrearAsync(Guid clienteId, List<VentaDetalle> detalles);

    Task<IEnumerable<Venta>> ObtenerPorEmpresaAsync();

    Task<Venta?> ObtenerPorIdAsync(Guid ventaId);

    Task<bool> AnularAsync(Guid ventaId);
}