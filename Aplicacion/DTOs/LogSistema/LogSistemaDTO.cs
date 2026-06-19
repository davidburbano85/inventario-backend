// Ubicación: /src/Aplicacion/DTOs/LogSistemaDTO.cs

namespace inventarioWebAI.Aplicacion.DTOs.LogSistema;

public class LogSistemaDTO
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; } // NUEVO: requerido para multi-tenant (alineado con tabla logs_sistema)

    public Guid UsuarioId { get; set; }

    public string? Accion { get; set; }

    public string? Detalle { get; set; }

    public DateTime CreatedAt { get; set; }
}