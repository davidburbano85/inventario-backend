using inventarioWebAI.Aplicacion.DTOs.Proveedor;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

public interface IProveedorServicio
{
    Task<Guid> CrearAsync(string nombre, string? contacto);

    Task<IEnumerable<ProveedorDTO>> ObtenerPorEmpresaAsync();

    Task<ProveedorDTO?> ObtenerPorIdAsync(Guid proveedorId);

    Task<bool> ActualizarAsync(Guid proveedorId, string nombre, string? contacto);

    Task<bool> EliminarAsync(Guid proveedorId);
}