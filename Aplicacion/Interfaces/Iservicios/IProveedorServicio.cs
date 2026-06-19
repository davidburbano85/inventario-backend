// Ubicación: /src/Aplicacion/Interfaces/IProveedorServicio.cs


// Ubicación: /src/Aplicacion/Interfaces/IProveedorServicio.cs

using inventarioWebAI.Aplicacion.DTOs.Proveedor;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

// NUEVO: interfaz para gestión de proveedores
// POR QUÉ:
// - Mantener consistencia con patrón Servicio + Interfaz
// - Permitir inyección de dependencias
public interface IProveedorServicio
{
    // NUEVO: obtener proveedores por empresa
    Task<IEnumerable<ProveedorDTO>> ObtenerPorEmpresa(Guid empresaId);

    // NUEVO: crear proveedor
    Task<Guid> Crear(CrearProveedorDTO dto);
}