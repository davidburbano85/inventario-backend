using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios;

public interface IProveedorRepositorio
{
    Task<Guid> CrearAsync(Proveedor proveedor);

    Task<IEnumerable<Proveedor?>> ObtenerPorEmpresaAsync(Guid empresaId);

    Task<Proveedor?> ObtenerPorIdAsync(Guid id, Guid empresaId);

    Task<bool> ActualizarAsync(Proveedor proveedor);

    Task<bool> EliminarAsync(Guid id, Guid empresaId);
}