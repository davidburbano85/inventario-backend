using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios
{
    public interface IResumenDiarioInventarioServicio
    {
        Task<int> GenerarResumenDiarioAsync(Guid almacenId, DateTime fecha);

        Task<int> GenerarResumenPorRangoAsync(Guid almacenId, DateTime fechaInicio, DateTime fechaFin);

        Task<IEnumerable<ResumenDiarioInventario>> ObtenerPorRangoAsync(
            Guid almacenId,
            DateTime fechaInicio,
            DateTime fechaFin);

        Task<ResumenDiarioInventario?> ObtenerPorProductoYFechaAsync(
            Guid almacenId,
            Guid productoId,
            DateTime fecha);

        Task<bool> RecalcularProductoAsync(
            Guid almacenId,
            Guid productoId,
            DateTime fechaInicio,
            DateTime fechaFin);

        Task<Guid> EjecutarDiarioAsync(Guid empresaId, Guid? almacenId = null);
    }
}