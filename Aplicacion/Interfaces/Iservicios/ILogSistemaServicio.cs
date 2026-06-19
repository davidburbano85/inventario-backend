// Ubicación: /src/Aplicacion/Interfaces/ILogSistemaServicio.cs


// Ubicación: /src/Aplicacion/Interfaces/ILogSistemaServicio.cs

using inventarioWebAI.Aplicacion.DTOs.LogSistema;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

public interface ILogSistemaServicio
{
    // MODIFICADO: se mantiene firma pero ahora LogSistemaDTO incluye EmpresaId (alineado con DB)
    Task<Guid> Crear(LogSistemaDTO dto); // MODIFICADO (impacta DTO)

    // EXISTENTE: coherente con nuevo modelo multi-tenant (usa empresaId)
    Task<IEnumerable<LogSistemaDTO>> ObtenerPorEmpresa(Guid empresaId);
}