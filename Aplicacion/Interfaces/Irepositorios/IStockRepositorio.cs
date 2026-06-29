using System.Data;

namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios;

public interface IStockRepositorio
{
    /// <summary>
    /// Obtiene la cantidad actual de un producto en un almacén.
    /// </summary>
    Task<decimal?> ObtenerCantidadAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId,
        Guid productoId,
        Guid almacenId
        );

    /// <summary>
    /// Crea el registro inicial de stock cuando aún no existe.
    /// </summary>
    Task CrearRegistroAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId,
        Guid productoId,
        Guid almacenId,
        decimal cantidadInicial);

    /// <summary>
    /// Incrementa el stock del producto.
    /// </summary>
    Task IncrementarStockAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId,
        Guid productoId,
        Guid almacenId,
        decimal cantidad);

    /// <summary>
    /// Disminuye el stock del producto.
    /// </summary>
    Task DisminuirStockAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId,
        Guid productoId,
        Guid almacenId,
        decimal cantidad);
}