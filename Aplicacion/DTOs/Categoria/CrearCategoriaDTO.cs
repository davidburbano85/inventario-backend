// Ubicación: /src/Aplicacion/DTOs/CrearCategoriaDTO.cs

namespace inventarioWebAI.Aplicacion.DTOs.Categoria;

// NUEVO: DTO para creación de categorías
// POR QUÉ:
// - Necesario para CategoriaServicio.Crear
// - Mantener consistencia con patrón de DTOs existente
public class CrearCategoriaDTO
{
    public Guid EmpresaId { get; set; } // NUEVO: requerido para multi-tenant

    public string Nombre { get; set; } = string.Empty; // NUEVO: nombre de la categoría
}