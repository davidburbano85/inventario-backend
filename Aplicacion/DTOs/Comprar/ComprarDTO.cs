// Ubicación: /src/Aplicacion/DTOs/CompraDTO.cs

namespace inventarioWebAI.Aplicacion.DTOs.Comprar;

public class CompraDTO
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }

    public Guid? ProveedorId { get; set; }

    public decimal Total { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Activo { get; set; }
}