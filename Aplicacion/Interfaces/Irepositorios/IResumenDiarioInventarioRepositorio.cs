using System.Data;
using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios
{
    public interface IResumenDiarioInventarioRepositorio
    {
        Task<int> GenerarResumenDiarioAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            Guid empresaId,
            Guid almacenId,
            DateTime fecha
        );

        Task<int> GenerarResumenPorRangoAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            Guid empresaId,
            Guid almacenId,
            DateTime fechaInicio,
            DateTime fechaFin
        );

        Task<IEnumerable<ResumenDiarioInventario>> ObtenerPorRangoAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            Guid empresaId,
            Guid almacenId,
            DateTime fechaInicio,
            DateTime fechaFin
        );

        Task<ResumenDiarioInventario?> ObtenerPorProductoYFechaAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            Guid empresaId,
            Guid almacenId,
            Guid productoId,
            DateTime fecha
        );

        Task<bool> UpsertResumenAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            ResumenDiarioInventario resumen
        );

        Task<bool> RecalcularProductoAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            Guid empresaId,
            Guid almacenId,
            Guid productoId,
            DateTime fechaInicio,
            DateTime fechaFin
        );
    }
}