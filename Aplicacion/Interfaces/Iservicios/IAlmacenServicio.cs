// Ubicación: /src/Aplicacion/Interfaces/IAlmacenServicio.cs


// Ubicación: /src/Aplicacion/Interfaces/IAlmacenServicio.cs

using inventarioWebAI.Aplicacion.DTOs.Almacen;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

// NUEVO: interfaz para gestión de almacenes
// POR QUÉ:
// - Mantener consistencia con patrón Servicio + Interfaz
// - Permitir inyección de dependencias
public interface IAlmacenServicio
{
    // NUEVO: obtener todos los almacenes por empresa
    Task<IEnumerable<AlmacenDTO>> ObtenerAlmacenPorEmpresaAsync();

    // NUEVO: crear un almacén
    Task<Guid> CrearAlmacenAsync(Guid usuarioId, string nombre, string ubicacion);
}