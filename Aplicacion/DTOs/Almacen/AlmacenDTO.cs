
namespace inventarioWebAI.Aplicacion.DTOs.Almacen;



public class AlmacenDTO
{
    public Guid Id { get; set; } // NUEVO: identificador del almacén

    public Guid EmpresaId { get; set; } // NUEVO: multi-tenant

    public string Nombre { get; set; } = string.Empty; // NUEVO: nombre del almacén
    public bool Activo { get; set; }=false;
    public string? Ubicacion { get; set; } // NUEVO: ubicación opcional

    public DateTime CreatedAt { get; set; } // NUEVO: fecha de creación
    public DateTime? UpdatedAt { get; set; }
}