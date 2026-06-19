// Ubicación: /src/Aplicacion/DTOs/CrearMovimientoDTO.cs
using inventarioWebAI.Aplicacion.Enums;
using System.Text.Json.Serialization;
namespace inventarioWebAI.Aplicacion.DTOs.Movimiento;

public class CrearMovimientoDTO
{
    public Guid EmpresaId { get; set; }

    public Guid ProductoId { get; set; }

    public Guid AlmacenId { get; set; }

    public TipoMovimientoDTO Tipo { get; set; } 

    public decimal Cantidad { get; set; }

    public string? Motivo { get; set; }
    public Guid UsuarioId { get; set; }
}