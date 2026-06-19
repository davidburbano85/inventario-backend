// Ubicación: /src/Aplicacion/Interfaces/IComprasServicio.cs

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

using inventarioWebAI.Aplicacion.DTOs.Comprar;

public interface IComprasServicio
{
    Task<IEnumerable<CompraDTO>> ObtenerPorEmpresa(Guid empresaId);

    Task<Guid> Crear(CrearCompraDTO dto);
}