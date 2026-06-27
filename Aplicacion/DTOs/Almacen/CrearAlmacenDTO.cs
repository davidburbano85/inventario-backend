
namespace inventarioWebAI.Aplicacion.DTOs.Almacen;

public class CrearAlmacenDTO
{
   

    public string Nombre { get; set; } = string.Empty; // NUEVO: nombre del almacén

    public string? Ubicacion { get; set; } // NUEVO: ubicación opcional
}