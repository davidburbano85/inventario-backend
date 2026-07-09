using inventarioWebAI.Dominio.Entidades;
using System.Data;

namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios;

public interface IVentasRepositorio
{
    Task<Guid> CrearVentaAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Venta venta);

    Task<IEnumerable<Venta>> ObtenerPorEmpresaAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId);

    Task<Venta?> ObtenerPorIdAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid id,
        Guid empresaId);

    Task<bool> AnularVentaAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid id,
        Guid empresaId);
    Task<Venta?> EncontrarPorFacturaAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId,
        string factura);
}