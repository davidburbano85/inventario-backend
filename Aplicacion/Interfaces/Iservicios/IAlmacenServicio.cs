// Ubicación: /src/Aplicacion/Interfaces/IAlmacenServicio.cs


// Ubicación: /src/Aplicacion/Interfaces/IAlmacenServicio.cs

using inventarioWebAI.Aplicacion.DTOs.Almacen;
using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;


public interface IAlmacenServicio
{
    // NUEVO: obtener todos los almacenes por empresa
    Task<IEnumerable<AlmacenDTO>> ObtenerAlmacenesActivosPorEmpresaAsync();

    // NUEVO: crear un almacén
    Task<Guid> CrearAlmacenAsync(Guid usuarioId, string nombre, string ubicacion);
    Task<bool> ActualizarAlmacenAsync(Guid almacenId, string nombre, string ubicacion);
    Task<AlmacenDTO?> ObtenerAlmacenPorIdAsync(Guid almacenId);
    Task<bool> EliminarAlmacenAsync(Guid almacenId);
}