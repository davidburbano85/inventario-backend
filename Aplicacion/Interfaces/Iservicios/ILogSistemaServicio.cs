using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

public interface ILogSistemaServicio
{
    // Registra una acción del sistema usando el contexto
    // actual del usuario y empresa.
    Task RegistrarAsync(
        string accion,
        string detalle);

    // Obtiene la auditoría de la empresa activa.
    Task<IEnumerable<LogSistema>> ObtenerPorEmpresaAsync();

    // Obtiene un registro específico validando pertenencia
    // a la empresa activa.
    Task<LogSistema?> ObtenerPorIdAsync(Guid logId);
}