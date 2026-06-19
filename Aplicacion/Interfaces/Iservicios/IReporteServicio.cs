// Ubicación: /src/Aplicacion/Interfaces/IReporteServicio.cs


// Ubicación: /src/Aplicacion/Interfaces/IReporteServicio.cs

using inventarioWebAI.Aplicacion.DTOs.Producto;
using inventarioWebAI.Aplicacion.DTOs.ResumenDiarioInventario;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

// Interfaz para consultas analíticas y reportes
public interface IReporteServicio
{
    // Ventas totales por periodo (desde - hasta)
    Task<decimal> ObtenerVentasPorPeriodo(Guid empresaId, DateTime desde, DateTime hasta);

    // Top N productos por ventas (cantidad) en un periodo
    Task<IEnumerable<ProductoDTO>> ObtenerTopProductos(Guid empresaId, DateTime desde, DateTime hasta, int topN = 10);

    // Stock valorizado total de la empresa
    Task<decimal> ObtenerStockValorizado(Guid empresaId);

    // Resumen diario de inventario para una fecha
    Task<IEnumerable<ResumenDiarioInventarioDTO>> ObtenerResumenDiario(Guid empresaId, DateTime fecha);
}
