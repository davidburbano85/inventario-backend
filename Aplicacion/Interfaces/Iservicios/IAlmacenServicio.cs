using inventarioWebAI.Aplicacion.DTOs.Almacen;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

public interface IAlmacenServicio
{
    Task<Guid> CrearAlmacenAsync(Guid usuarioId, string nombre, string ubicacion);

    Task<IEnumerable<AlmacenDTO>> ObtenerAlmacenesActivosPorEmpresaAsync();

    Task<AlmacenDTO?> ObtenerAlmacenPorIdAsync(Guid almacenId);

    Task<bool> ActualizarAlmacenAsync(Guid almacenId, string nombre, string ubicacion);

    Task<bool> EliminarAlmacenAsync(Guid almacenId);

    Task SeleccionarAlmacenAsync(Guid almacenId);

    Task<Guid> ObtenerAlmacenActivoAsync();
}