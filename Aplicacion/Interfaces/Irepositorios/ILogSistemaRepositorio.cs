using inventarioWebAI.Dominio.Entidades;
using System.Data;

namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios;

public interface ILogSistemaRepositorio
{
    // Registra una acción realizada dentro de una empresa.
    // Se reciben conexión y transacción porque puede formar parte
    // de operaciones críticas que ya usan UnitOfWork.
    Task<Guid> RegistrarAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        LogSistema log);

    // Consulta la auditoría filtrada por empresa.
    // La empresa será validada antes desde la capa de servicio.
    Task<IEnumerable<LogSistema>> ObtenerPorEmpresaAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId);

    // Consulta un registro específico de auditoría.
    Task<LogSistema?> ObtenerPorIdAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid id,
        Guid empresaId);
}