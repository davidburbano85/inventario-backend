// Ubicación: /src/Aplicacion/Interfaces/IRepositorios/ICompraDetalleRepositorio.cs

using inventarioWebAI.Dominio.Entidades;
using System.Data;

namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios;

public interface ICompraDetalleRepositorio
{
    // Se utiliza únicamente desde el repositorio de Compra dentro de la misma transacción.
    Task InsertarAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid compraId,
        Guid empresaId,
        IEnumerable<CompraDetalle> detalles);

    // Consulta los detalles de una compra perteneciente a una empresa.
    Task<IEnumerable<CompraDetalle>> ObtenerPorCompraAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    Guid compraId,
    Guid empresaId);
}