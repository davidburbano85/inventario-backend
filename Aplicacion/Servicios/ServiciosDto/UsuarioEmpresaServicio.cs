
using inventarioWebAI.Aplicacion.DTOs.UsuarioEmpresa;
using inventarioWebAI.Aplicacion.Enums; // necesario para enum RolUsuarioEmpresaDTO
using inventarioWebAI.Aplicacion.Interfaces.IPermmisoServicios;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios; // requerido para repositorios
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Dominio.Enums;
using inventarioWebAI.Infraestructura.Mapper.MapperDto;
using System.Linq;

namespace inventarioWebAI.Aplicacion.Servicios;


public class UsuarioEmpresaServicio : IUsuarioEmpresaServicio
{
    private readonly IUsuarioEmpresaRepositorio _usuarioEmpresaRepositorio;
    private readonly IEmpresaRepositorio _empresaRepositorio;
    private readonly IUsuarioRepositorio _usuarioRepositorio;
    private readonly IPermisoServicio _permisoServicio;

    public UsuarioEmpresaServicio(IUsuarioEmpresaRepositorio usuarioEmpresaRepositorio,
                                   IEmpresaRepositorio empresaRepositorio,
                                   IUsuarioRepositorio usuarioRepositorio,
                                   IPermisoServicio permisoServicio)
    {
        _usuarioEmpresaRepositorio = usuarioEmpresaRepositorio;
        _empresaRepositorio = empresaRepositorio;
        _usuarioRepositorio = usuarioRepositorio;
        _permisoServicio = permisoServicio;
    }

    public async Task<UsuarioEmpresaDTO?> ObtenerPorIdAsync(Guid id, Guid empresaId)
    {

        if (id == Guid.Empty)
        {
            return null;
        }

        var entity = await _usuarioEmpresaRepositorio.ObtenerPorIdAsync(id);
        await _permisoServicio.ValidarAdminOSuperAdminAsync(empresaId, id);
        if (entity == null)
            return null;

        var resultado = new UsuarioEmpresaDTO
        {
            Id = entity.Id,
            EmpresaId = entity.EmpresaId,
            UsuarioId = entity.UsuarioId,
            Rol = RolUsuarioEmpresaMapperDto.ToDto(entity.Rol),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
        return resultado;
    }

    public async Task<IEnumerable<UsuarioEmpresaDTO>> ObtenerPorEmpresaUsuarioAsync(
     Guid empresaId,
     Guid usuarioId)
    {
        if (empresaId == Guid.Empty || usuarioId == Guid.Empty)
            return Enumerable.Empty<UsuarioEmpresaDTO>();

        var list = await _usuarioEmpresaRepositorio.ObtenerPorEmpresaUsuarioAsync(empresaId, usuarioId);
        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        return list.Select(e => new UsuarioEmpresaDTO
        {
            Id = e.Id,
            EmpresaId = e.EmpresaId,
            UsuarioId = e.UsuarioId,
            Rol = RolUsuarioEmpresaMapperDto.ToDto(e.Rol),
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        });
    }

    public async Task<Guid> CrearAsync(Guid empresaId, Guid usuarioId, RolUsuarioEmpresaDTO rol)
    {

        if (empresaId == Guid.Empty)
        {
            throw new InvalidOperationException("El ID de la empresa es inválido.");
        }

        if (usuarioId == Guid.Empty)
        {
            throw new InvalidOperationException("El ID del usuario es inválido.");
        }

        var empresa = await _empresaRepositorio.ObtenerEmpresaPorIdAsync(empresaId);
        if (empresa == null)
        {
            throw new InvalidOperationException("La empresa no existe.");
        }

        var usuario = await _usuarioRepositorio.ObtenerPorIdAsync(usuarioId);
        if (usuario == null)
        {
            throw new InvalidOperationException("El usuario no existe.");
        }

        var rolDomain = RolUsuarioEmpresaMapperDto.ToDomain(rol);

        var resultado = await _usuarioEmpresaRepositorio.CrearAsync(empresaId, usuarioId, rolDomain);

        return resultado;
    }

    public async Task ActualizarRolAsync(Guid id, Guid empresaId, RolUsuarioEmpresaDTO rol)
    {

        if (id == Guid.Empty)
        {
            throw new InvalidOperationException("El ID es inválido.");
        }

        if (empresaId == Guid.Empty)
        {
            throw new InvalidOperationException("El ID de la empresa es inválido.");
        }

        var relacionesUsuario = await _usuarioEmpresaRepositorio.ObtenerPorEmpresaUsuarioAsync(empresaId, id);

        var relacion = relacionesUsuario.FirstOrDefault(r => r.EmpresaId == empresaId);
        if (relacion == null)
        {
            throw new InvalidOperationException("La relación usuario-empresa no existe.");
        }
      
        var nuevoRol = RolUsuarioEmpresaMapperDto.ToDomain(rol);

        if (relacion.Rol == RolUsuarioEmpresa.SuperAdmin && nuevoRol != RolUsuarioEmpresa.SuperAdmin)
        {
            var admins = await _usuarioEmpresaRepositorio.ContarAdminsPorEmpresaAsync(empresaId);

            if (admins <= 1)
            {
                throw new InvalidOperationException("No se puede quitar el último administrador de la empresa.");
            }
        }

        var actualizado = await _usuarioEmpresaRepositorio.ActualizarRolAsync(id, empresaId, nuevoRol);

        if (!actualizado)
        {
            throw new Exception("No se pudo actualizar el rol del usuario en la empresa.");
        }

    }

    public async Task<string> EliminarAsync(Guid id)
    {

        if (id == Guid.Empty)
        {
            throw new InvalidOperationException("El ID es inválido.");
        }

        var relacion = await _usuarioEmpresaRepositorio.ObtenerPorIdAsync(id);

        if (relacion == null)
        {
            throw new InvalidOperationException("La relación usuario-empresa no existe.");
        }

        if (relacion.Rol == RolUsuarioEmpresa.Admin)
        {
            var admins = await _usuarioEmpresaRepositorio.ContarAdminsPorEmpresaAsync(relacion.EmpresaId);

            if (admins <= 1)
            {
                throw new InvalidOperationException("No se puede eliminar al último administrador de la empresa.");
            }
        }

        var nombre = await _usuarioEmpresaRepositorio.ObtenerNombreUsuarioAsync(id);

        var eliminado = await _usuarioEmpresaRepositorio.EliminarAsync(id);

        if (!eliminado)
        {
            throw new Exception("No se pudo eliminar la relación usuario-empresa.");
        }

        return nombre ?? string.Empty;
    }

}
