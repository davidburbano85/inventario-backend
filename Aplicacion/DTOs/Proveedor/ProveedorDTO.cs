// Ubicación: /src/Aplicacion/DTOs/ProveedorDTO.cs

namespace inventarioWebAI.Aplicacion.DTOs.Proveedor;

// NUEVO: DTO de salida para proveedores
// POR QUÉ:
// - Necesario para ProveedorServicio.ObtenerPorEmpresa
// - Mantener consistencia con otros DTOs de lectura
public class ProveedorDTO
{
    public Guid Id { get; set; } // NUEVO: identificador del proveedor

    public Guid EmpresaId { get; set; } // NUEVO: multi-tenant

    public string Nombre { get; set; } = string.Empty; // NUEVO: nombre del proveedor
    public string? Contacto { get; set; } // NUEVO: contacto del proveedor

    public DateTime CreatedAt { get; set; } // NUEVO: fecha de creación
    public DateTime UpdatedAt { get; set; } // NUEVO: fecha de última actualización
    public bool Activo { get; set; } // NUEVO: estado activo/inactivo
}