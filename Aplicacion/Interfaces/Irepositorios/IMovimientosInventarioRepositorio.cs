using inventarioWebAI.Dominio.Entidades;
using System.Data;

namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios;

public interface IMovimientosInventarioRepositorio
{
    // Inserta un movimiento de inventario
    Task<Guid> InsertarAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        MovimientoInventario movimiento);

    // Historial completo por empresa
    Task<IEnumerable<MovimientoInventario>> ObtenerPorEmpresaAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId);

    // Historial por producto
    Task<IEnumerable<MovimientoInventario>> ObtenerPorProductoAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId,
        Guid productoId);

    // Historial por almacén
    Task<IEnumerable<MovimientoInventario>> ObtenerPorAlmacenAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId,
        Guid almacenId);

    // Historial completo filtrado (flexible para reporting)
    Task<IEnumerable<MovimientoInventario>> FiltrarAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId,
        Guid? productoId = null,
        Guid? almacenId = null,
        string? tipo = null);

    Task<MovimientoInventario?> EncontrarPorFacturaAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId,
        string factura);
}