// Ubicación: /src/Aplicacion/Servicios/EmpresaServicio.cs

using inventarioWebAI.Aplicacion.DTOs.Empresa;
using inventarioWebAI.Aplicacion.Interfaces.IPermmisoServicios;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Dominio.Entidades;
using inventarioWebAI.Dominio.Enums;


namespace inventarioWebAI.Aplicacion.Servicios;

// NUEVO: servicio para gestión de empresas
// POR QUÉ:
// - Entidad raíz del sistema multi-tenant
// - Necesario para crear y consultar empresas
public class EmpresaServicio : IEmpresaServicio
{
    private readonly IEmpresaRepositorio _empresaRepositorio;
    private readonly IPermisoServicio _permisoServicio;
    private readonly IUsuarioEmpresaRepositorio _usuarioEmpresaRepositorio;

    public EmpresaServicio(IEmpresaRepositorio empresaRepositorio,
                            IPermisoServicio permisoServicio, IUsuarioEmpresaRepositorio usuarioEmpresaRepositorio)
    {
        _empresaRepositorio = empresaRepositorio;
        _permisoServicio = permisoServicio;
        _usuarioEmpresaRepositorio = usuarioEmpresaRepositorio;
    }

  

    // NUEVO: crear empresa
    public async Task<Guid> CrearEmpresaAsync(Guid usuarioId, EmpresaDTO dto)
    {

        // NUEVO: validaciones
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            throw new InvalidOperationException("El nombre de la empresa es obligatorio.");

        var empresa = new Empresa// NUEVO: mapeo manual (sin AutoMapper)
        {
            
            Nombre = dto.Nombre.Trim()
        };
        
        var empresaId = await _empresaRepositorio.CrearEmpresaAsync(empresa);
        await _usuarioEmpresaRepositorio.CrearAsync(
            empresaId,
            usuarioId, RolUsuarioEmpresa.SuperAdmin);

        return empresaId;

    }
    public async Task<EmpresaDTO> ObtenerEmpresaPorIdAsync(Guid empresaId)
    {
        if (empresaId == Guid.Empty)
            throw new InvalidOperationException("El ID de la empresa no puede estar vacío.");

        var empresa = await _empresaRepositorio.ObtenerEmpresaPorIdAsync(empresaId);

        if (empresa == null)
            return null;

        return new EmpresaDTO
        {
            Id = empresa.Id,
            Nombre = empresa.Nombre,
            CreatedAt = empresa.CreatedAt
        };
    }

    // NUEVO: obtener empresas de un usuario (join con usuarios_empresas)
    public async Task<IEnumerable<EmpresaDTO>> ObtenerEmpresaPorUsuarioAsync(Guid usuarioId)
    {
        if (usuarioId == Guid.Empty)
            throw new InvalidOperationException("El ID del usuario no puede estar vacío.");

        var empresas = await _empresaRepositorio.ObtenerEmpresaPorUsuarioAsync(usuarioId);
        return empresas.Select(e => new EmpresaDTO
        {
            Id = e.Id,
            Nombre = e.Nombre,
            CreatedAt = e.CreatedAt
        });
    }


    public async Task<EmpresaDTO> ActualizarEmpresaAsync(Guid usuarioId, EmpresaDTO dto)
    {
        if (dto.Id == Guid.Empty)
            throw new InvalidOperationException("ID inválido.");

        if (string.IsNullOrWhiteSpace(dto.Nombre))
            throw new InvalidOperationException("El nombre es obligatorio.");
        //validar que el usuario tenga permisos de admin o superadmin en la empresa
        await _permisoServicio.ValidarRolAsync(usuarioId, dto.Id,RolUsuarioEmpresa.Admin, RolUsuarioEmpresa.SuperAdmin);

        var empresa = new Empresa
        {
            Id = dto.Id,
            Nombre = dto.Nombre.Trim()
        };

        var empresaActualizada = await _empresaRepositorio.ActualizarEmpresaAsync(empresa);

        if (empresaActualizada == null)
            throw new InvalidOperationException("La empresa no existe.");

        return new EmpresaDTO
        {
            Id = empresaActualizada.Id,
            Nombre = empresaActualizada.Nombre,
            CreatedAt = empresaActualizada.CreatedAt
        };
    }

    public async Task<bool> EliminarEmpresaAsync(Guid usuarioId, Guid empresaId)
    {
        if (empresaId == Guid.Empty)
            throw new InvalidOperationException("ID inválido.");
        Console.WriteLine($"Validando permisos para eliminar empresa. UsuarioId: {usuarioId}, EmpresaId: {empresaId}");
        await _permisoServicio.ValidarSuperAdminAsync(usuarioId, empresaId);// Solo superadmin puede eliminar la empresa

        var result = await _empresaRepositorio.EliminarEmpresaAsync(empresaId);

        return result != null;
    }


}