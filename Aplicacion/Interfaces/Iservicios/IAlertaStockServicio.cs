// Ubicación: /src/Aplicacion/Interfaces/IAlertaStockServicio.cs


// Ubicación: /src/Aplicacion/Interfaces/IAlertaStockServicio.cs

using inventarioWebAI.Aplicacion.DTOs.StockActual;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

// Interfaz para detección y gestión de alertas de stock
public interface IAlertaStockServicio
{
    // Obtener productos cuyo stock total (suma en almacenes) está por debajo del umbral
    Task<IEnumerable<StockActualDTO>> ObtenerProductosBajoUmbral(Guid empresaId, decimal umbral);

    // Crear/registrar una alerta manual para un producto
    Task<Guid> RegistrarAlerta(Guid empresaId, Guid productoId, decimal umbral, string nota);

    // Marcar alerta como atendida
    Task MarcarAlertaAtendida(Guid alertaId, Guid usuarioId);
}
