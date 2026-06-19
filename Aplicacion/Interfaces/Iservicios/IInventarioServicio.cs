// Ubicación: /src/Aplicacion/Interfaces/IInventarioServicio.cs

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

using inventarioWebAI.Aplicacion.DTOs.Movimiento;
using inventarioWebAI.Aplicacion.DTOs.MovimientoUnitario;

public interface IInventarioServicio
{
    Task<IEnumerable<MovimientoInventarioDTO>> ObtenerMovimientos(Guid empresaId);

    Task RegistrarMovimiento(CrearMovimientoDTO dto);
}