using inventarioWebAI.Aplicacion.DTOs.Categoria;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

public interface ICategoriaServicio
{
    Task<Guid> CrearCategoriaAsync(string nombre);

    Task<IEnumerable<CategoriaDTO>> ObtenerCategoriasActivasPorEmpresaAsync();

    Task<CategoriaDTO?> ObtenerCategoriaPorIdAsync(Guid categoriaId);

    Task<bool> ActualizarCategoriaAsync(Guid categoriaId, string nombre);

    Task<bool> EliminarCategoriaAsync(Guid categoriaId);
}