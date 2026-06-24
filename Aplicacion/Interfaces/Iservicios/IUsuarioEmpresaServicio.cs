// Ubicación: /src/Aplicacion/Interfaces/IUsuarioEmpresaServicio.cs

using inventarioWebAI.Aplicacion.DTOs.UsuarioEmpresa;
using inventarioWebAI.Aplicacion.Enums;
using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

// NUEVO: interfaz para gestión de relación usuario-empresa
// POR QUÉ:
// - Mantener consistencia con patrón Servicio + Interfaz
// - Permitir inyección de dependencias
public interface IUsuarioEmpresaServicio
{
    Task<UsuarioEmpresaDTO?> ObtenerPorIdAsync(Guid usuarioId, Guid empresaId);

    Task<IEnumerable<UsuarioEmpresaDTO>> ObtenerPorEmpresaUsuarioAsync(Guid usuarioId, Guid empresaId);


    Task<Guid> CrearAsync(Guid empresaId, Guid usuarioId, RolUsuarioEmpresaDTO rol);

    Task ActualizarRolAsync(Guid id, Guid empresaId, RolUsuarioEmpresaDTO rol);

    Task<string> EliminarAsync(Guid id);
    Task<string> SeleccionarEmpresaAsync(Guid usuarioId, Guid empresaId);

}