using inventarioWebAI.Dominio.Entidades;
using inventarioWebAI.Dominio.Enums;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

public interface IMovimientosInventarioServicio
{
    Task<Guid> RegistrarAsync(
        Guid productoId,
        Guid almacenId,
        decimal cantidad,
        TipoMovimiento tipo,
        string motivo);

    Task<IEnumerable<MovimientoInventario>> ObtenerPorEmpresaAsync();

    Task<IEnumerable<MovimientoInventario>> ObtenerPorProductoAsync(Guid productoId);

    Task<IEnumerable<MovimientoInventario>> ObtenerPorAlmacenAsync(Guid almacenId);

    Task<IEnumerable<MovimientoInventario>> FiltrarAsync(
        Guid? productoId = null,
        Guid? almacenId = null,
        string? tipo = null);
}