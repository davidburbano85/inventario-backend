// Ubicación: /src/Aplicacion/DTOs/CrearAlmacenDTO.cs

namespace inventarioWebAI.Aplicacion.DTOs.Almacen;

// NUEVO: DTO para creación de almacenes
// POR QUÉ:
// - Necesario para AlmacenServicio.Crear
// - Mantiene consistencia con patrón DTO existente
public class CrearAlmacenDTO
{
   

    public string Nombre { get; set; } = string.Empty; // NUEVO: nombre del almacén

    public string? Ubicacion { get; set; } // NUEVO: ubicación opcional
}