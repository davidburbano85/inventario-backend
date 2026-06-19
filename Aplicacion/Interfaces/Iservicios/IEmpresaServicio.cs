
using inventarioWebAI.Aplicacion.DTOs.Empresa;
using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

// NUEVO: interfaz para gestión de empresas
// POR QUÉ:
// - Mantener consistencia con patrón Servicio + Interfaz
// - Permitir inyección de dependencias
public interface IEmpresaServicio
{
   



    // NUEVO: crear empresa
    Task<Guid> CrearEmpresaAsync(Guid usuarioId, EmpresaDTO dto);
    // NUEVO: obtener empresas asociadas a un usuario
    Task<EmpresaDTO> ObtenerEmpresaPorIdAsync( Guid empresaId);
    Task<IEnumerable<EmpresaDTO>> ObtenerEmpresaPorUsuarioAsync(Guid usuarioId); 

    Task <EmpresaDTO>ActualizarEmpresaAsync( Guid usuarioId, EmpresaDTO dto);
    Task<bool>EliminarEmpresaAsync(Guid usuarioId ,Guid empresaId);
}