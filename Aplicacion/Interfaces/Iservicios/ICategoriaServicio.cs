// Ubicación: /src/Aplicacion/Interfaces/ICategoriaServicio.cs


// Ubicación: /src/Aplicacion/Interfaces/ICategoriaServicio.cs

using inventarioWebAI.Aplicacion.DTOs.Categoria;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

// NUEVO: interfaz para gestión de categorías
// POR QUÉ:
// - Mantener consistencia con patrón Servicio + Interfaz
// - Permitir inyección de dependencias
public interface ICategoriaServicio
{
    // NUEVO: obtener categorías por empresa
    Task<IEnumerable<CategoriaDTO>> ObtenerPorEmpresa(Guid empresaId);

    // NUEVO: crear categoría
    Task<Guid> Crear(CrearCategoriaDTO dto);
}