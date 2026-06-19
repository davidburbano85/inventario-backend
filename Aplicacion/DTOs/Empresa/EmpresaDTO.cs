// Ubicación: /src/Aplicacion/DTOs/EmpresaDTO.cs

namespace inventarioWebAI.Aplicacion.DTOs.Empresa;

// NUEVO: DTO de salida para empresas
// POR QUÉ:
// - Necesario para EmpresaServicio.ObtenerPorUsuario
// - Mantiene consistencia con DTOs de lectura
public class EmpresaDTO
{
    public Guid Id { get; set; } // NUEVO: identificador de la empresa

    public string Nombre { get; set; } = string.Empty; // NUEVO: nombre de la empresa

    public DateTime CreatedAt { get; set; } // NUEVO: fecha de creación
    public DateTime UpdatedAt { get; set; } // NUEVO: fecha de última actualización
}