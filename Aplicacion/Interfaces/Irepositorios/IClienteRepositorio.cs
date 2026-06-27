using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios;

public interface IClienteRepositorio
{
    Task<Guid> CrearAsync(Cliente cliente);

    Task<IEnumerable<Cliente?>> ObtenerPorEmpresaAsync(Guid empresaId);

    Task<Cliente?> ObtenerPorIdAsync(Guid id, Guid empresaId);

    Task<bool> ActualizarAsync(Cliente cliente);

    Task<bool> EliminarAsync(Guid id, Guid empresaId);
}