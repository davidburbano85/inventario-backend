// Ubicación: /src/Dominio/Entidades/LogSistema.cs

namespace inventarioWebAI.Dominio.Entidades;

public class LogSistema
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public Guid UsuarioId { get; set; }

    public string? Accion { get; set; }

    public string? Detalle { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}