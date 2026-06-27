using inventarioWebAI.Aplicacion.DTOs.Cliente;

namespace inventarioWebAI.Aplicacion.Interfaces.Iservicios;

public interface IClienteServicio
{
    Task<Guid> CrearAsync(string nombre, string? contacto);

    Task<IEnumerable<ClienteDTO>> ObtenerPorEmpresaAsync();

    Task<ClienteDTO?> ObtenerPorIdAsync(Guid clienteId);

    Task<bool> ActualizarAsync(Guid clienteId, string nombre, string? contacto);

    Task<bool> EliminarAsync(Guid clienteId);
}