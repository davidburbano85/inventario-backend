// Ubicación: /src/Aplicacion/Interfaces/IVentasServicio.cs

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

using inventarioWebAI.Aplicacion.DTOs.Venta;

public interface IVentasServicio
{
    Task<IEnumerable<VentaDTO>> ObtenerPorEmpresa(Guid empresaId);

    Task<Guid> Crear(CrearVentaDTO dto);
}