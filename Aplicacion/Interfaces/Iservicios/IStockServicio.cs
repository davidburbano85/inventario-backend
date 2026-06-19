// Ubicación: /src/Aplicacion/Interfaces/IStockServicio.cs


// Ubicación: /src/Aplicacion/Interfaces/IStockServicio.cs

using inventarioWebAI.Aplicacion.DTOs.Movimiento;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

// Interfaz para comandos sobre stock (operaciones que modifican stock)
public interface IStockServicio
{
    // Crear la fila inicial de stock para un producto en un almacén
    Task<Guid> CrearStockInicial(Guid empresaId, Guid productoId, Guid almacenId, decimal cantidad);

    // Ajuste manual de stock mediante creación de movimiento (entrada/salida/ajuste)
    Task<Guid> AjustarStockManual(CrearMovimientoDTO dto);
}
