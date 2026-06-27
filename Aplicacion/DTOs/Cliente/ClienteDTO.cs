// Ubicación: /src/Aplicacion/DTOs/ClienteDTO.cs

namespace inventarioWebAI.Aplicacion.DTOs.Cliente;

// NUEVO: DTO de salida para clientes
// POR QUÉ:
// - Necesario para ClienteServicio.ObtenerPorEmpresa
// - Mantener consistencia con otros DTOs de lectura
public class ClienteDTO
{
    public Guid Id { get; set; } // NUEVO: identificador del cliente

    public Guid EmpresaId { get; set; } // NUEVO: multi-tenant

    public string Nombre { get; set; } = string.Empty; // NUEVO: nombre del cliente
    public string Contacto { get; set; } 
    public DateTime CreatedAt { get; set; } // NUEVO: fecha de creación
    public DateTime UpdatedAt { get; set; } // NUEVO: fecha de actualización
    public bool Activo { get; set; } // NUEVO: estado del cliente
}