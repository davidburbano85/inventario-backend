using inventarioWebAI.Dominio.Entidades;
using System.Data;

namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios;

public interface IComprasRepositorio
{
    Task<Guid> CrearCompraAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Compra compra);

    Task<IEnumerable<Compra?>> ObtenerPorEmpresaAsync(Guid empresaId);

    Task<Compra?> ObtenerPorIdAsync(Guid id, Guid empresaId);

    Task<bool> AnularCompraAsync(Guid id, Guid empresaId);
}