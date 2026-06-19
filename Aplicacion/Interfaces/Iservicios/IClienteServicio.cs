// Ubicación: /src/Aplicacion/Interfaces/IClienteServicio.cs


// Ubicación: /src/Aplicacion/Interfaces/IClienteServicio.cs

using inventarioWebAI.Aplicacion.DTOs.Cliente;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

// NUEVO: interfaz para gestión de clientes
// POR QUÉ:
// - Mantener consistencia con patrón Servicio + Interfaz
// - Permitir inyección de dependencias
public interface IClienteServicio
{
    // NUEVO: obtener clientes por empresa
    Task<IEnumerable<ClienteDTO>> ObtenerPorEmpresa(Guid empresaId);

    // NUEVO: crear cliente
    Task<Guid> Crear(CrearClienteDTO dto);
}