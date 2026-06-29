using inventarioWebAI.Dominio.Entidades;
using System.Data;

namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios;

public interface IVentasDetalleRepositorio
{
    Task InsertarAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid ventaId,
        Guid empresaId,
        IEnumerable<VentaDetalle> detalles);

    Task<IEnumerable<VentaDetalle>> ObtenerPorVentaAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid ventaId,
        Guid empresaId);
}