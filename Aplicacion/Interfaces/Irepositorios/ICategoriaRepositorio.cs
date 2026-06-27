using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios;

public interface ICategoriaRepositorio
{
    Task<Guid> CrearCategoriaAsync(Categoria categoria);

    Task<IEnumerable<Categoria?>> ObtenerCategoriasActivasPorEmpresaAsync(Guid empresaId);

    Task<Categoria?> ObtenerCategoriaPorIdAsync(Guid id, Guid empresaId);

    Task<bool> ActualizarCategoriaAsync(Categoria categoria);

    Task<bool> EliminarCategoriaAsync(Guid id, Guid empresaId);
}