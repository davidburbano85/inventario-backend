// Ubicación: /src/Aplicacion/Interfaces/IStockActualServicio.cs


// Ubicación: /src/Aplicacion/Interfaces/IStockActualServicio.cs

using inventarioWebAI.Aplicacion.DTOs.StockActual;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

// NUEVO: interfaz para consultas de stock (read-model)
// POR QUÉ:
// - No modificar InventarioServicio (cerrado)
// - Separar lectura de stock (query) de comandos (movimientos)
public interface IStockActualServicio
{
    // NUEVO: obtiene todo el stock de una empresa (producto + almacén)
    Task<IEnumerable<StockActualDTO>> ObtenerPorEmpresa(Guid empresaId);

    // NUEVO: obtiene stock por producto en todos los almacenes
    Task<IEnumerable<StockActualDTO>> ObtenerPorProducto(Guid empresaId, Guid productoId);
}