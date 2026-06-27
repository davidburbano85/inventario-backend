using inventarioWebAI.Aplicacion.DTOs.Cliente;
using inventarioWebAI.Aplicacion.Interfaces.Context;
using inventarioWebAI.Aplicacion.Interfaces.IPermmisoServicios;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Servicios;

public class ClienteServicio : IClienteServicio
{
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IUsuarioContext _usuarioContext;
    private readonly IUsuarioEmpresaRepositorio _usuarioEmpresaRepositorio;
    private readonly IEmpresaRepositorio _empresaRepositorio;
    private readonly IPermisoServicio _permisoServicio;

    public ClienteServicio(
        IClienteRepositorio clienteRepositorio,
        IUsuarioContext usuarioContext,
        IUsuarioEmpresaRepositorio usuarioEmpresaRepositorio,
        IEmpresaRepositorio empresaRepositorio,
        IPermisoServicio permisoServicio)
    {
        _clienteRepositorio = clienteRepositorio;
        _usuarioContext = usuarioContext;
        _usuarioEmpresaRepositorio = usuarioEmpresaRepositorio;
        _empresaRepositorio = empresaRepositorio;
        _permisoServicio = permisoServicio;
    }

    public async Task<Guid> CrearAsync(string nombre, string? contacto)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new InvalidOperationException("El nombre es obligatorio.");

        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        if (usuarioId == Guid.Empty)
            throw new InvalidOperationException("Usuario inválido.");

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null || !usuarioEmpresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var cliente = new Cliente
        {
            EmpresaId = empresaId,
            Nombre = nombre.Trim(),
            Contacto = contacto
        };

        return await _clienteRepositorio.CrearAsync(cliente);
    }

    public async Task<IEnumerable<ClienteDTO>> ObtenerPorEmpresaAsync()
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        if (usuarioId == Guid.Empty)
            throw new InvalidOperationException("Usuario inválido.");

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null || !usuarioEmpresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var clientes = await _clienteRepositorio.ObtenerPorEmpresaAsync(empresaId);

        if (clientes == null)
            return Enumerable.Empty<ClienteDTO>();

        return clientes.Select(c => new ClienteDTO
        {
            Id = c!.Id,
            EmpresaId = c.EmpresaId,
            Nombre = c.Nombre,
            Contacto = c.Contacto,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            Activo = c.Activo
        });
    }

    public async Task<ClienteDTO?> ObtenerPorIdAsync(Guid clienteId)
    {
        if (clienteId == Guid.Empty)
            throw new InvalidOperationException("Cliente inválido.");

        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null || !usuarioEmpresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var cliente = await _clienteRepositorio.ObtenerPorIdAsync(clienteId, empresaId);

        if (cliente == null)
            return null;

        return new ClienteDTO
        {
            Id = cliente.Id,
            EmpresaId = cliente.EmpresaId,
            Nombre = cliente.Nombre,
            Contacto = cliente.Contacto,
            CreatedAt = cliente.CreatedAt,
            UpdatedAt = cliente.UpdatedAt,
            Activo = cliente.Activo
        };
    }

    public async Task<bool> ActualizarAsync(Guid clienteId, string nombre, string? contacto)
    {
        if (clienteId == Guid.Empty)
            throw new InvalidOperationException("Cliente inválido.");

        if (string.IsNullOrWhiteSpace(nombre))
            throw new InvalidOperationException("El nombre es obligatorio.");

        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null || !usuarioEmpresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var clienteExistente = await _clienteRepositorio.ObtenerPorIdAsync(clienteId, empresaId);

        if (clienteExistente == null)
            throw new InvalidOperationException("El cliente no existe.");

        var cliente = new Cliente
        {
            Id = clienteId,
            EmpresaId = empresaId,
            Nombre = nombre.Trim(),
            Contacto = contacto
        };

        return await _clienteRepositorio.ActualizarAsync(cliente);
    }

    public async Task<bool> EliminarAsync(Guid clienteId)
    {
        if (clienteId == Guid.Empty)
            throw new InvalidOperationException("Cliente inválido.");

        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null || !usuarioEmpresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var cliente = await _clienteRepositorio.ObtenerPorIdAsync(clienteId, empresaId);

        if (cliente == null)
            return false;

        return await _clienteRepositorio.EliminarAsync(clienteId, empresaId);
    }
}