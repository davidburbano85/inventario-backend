using System.Data;
using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios
{
    public interface IMonitoreoResumenInventarioRepositorio
    {
        Task<Guid> CrearAsync(IDbConnection connection, IDbTransaction transaction, MonitoreoResumenInventario entidad);

        Task<bool> MarcarExitosoAsync(IDbConnection connection, IDbTransaction transaction, Guid id, int registrosProcesados);

        Task<bool> MarcarFallidoAsync(IDbConnection connection, IDbTransaction transaction, Guid id, string mensaje);

        Task<IEnumerable<MonitoreoResumenInventario>> ObtenerUltimosAsync(IDbConnection connection, int limite);

        Task<MonitoreoResumenInventario?> ObtenerUltimoFallidoAsync(IDbConnection connection);
    }
}