// Ubicación: /src/Dominio/Entidades/Cliente.cs

using System.ComponentModel.DataAnnotations;

namespace inventarioWebAI.Dominio.Entidades;

public class Cliente
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }
    [Required]
    public string Nombre { get; set; } = string.Empty;

    public string? Contacto { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Activo { get; set; } 
}