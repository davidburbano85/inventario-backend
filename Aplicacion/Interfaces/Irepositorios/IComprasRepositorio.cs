using inventarioWebAI.Dominio.Entidades;
using System.Data;

public interface IComprasRepositorio
{
    Task<Guid> CrearCompraAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Compra compra);

    Task<IEnumerable<Compra>> ObtenerPorEmpresaAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId);

    Task<Compra?> ObtenerPorIdAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid id,
        Guid empresaId);

    Task<bool> AnularCompraAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid id,
        Guid empresaId);

    Task<Compra?> EncontrarPorFacturaAsync(
       IDbConnection connection,
       IDbTransaction transaction,
       Guid empresaId,
       string factura);
}