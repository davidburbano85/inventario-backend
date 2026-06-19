// Ubicación: /src/Aplicacion/Interfaces/IResumenDiarioInventarioServicio.cs


// Ubicación: /src/Aplicacion/Interfaces/IResumenDiarioInventarioServicio.cs

using inventarioWebAI.Aplicacion.DTOs.ResumenDiarioInventario;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

// NUEVO: interfaz para el servicio de resumen diario de inventario
// POR QUÉ:
// - Mantener consistencia con patrón existente (Servicio + Interfaz)
// - Permitir inyección de dependencias
public interface IResumenDiarioInventarioServicio
{
    // NUEVO: obtiene resumen diario por empresa
    Task<IEnumerable<ResumenDiarioInventarioDTO>> ObtenerPorEmpresa(Guid empresaId);

    // NUEVO: obtiene resumen diario por producto
    Task<IEnumerable<ResumenDiarioInventarioDTO>> ObtenerPorProducto(Guid empresaId, Guid productoId);
}