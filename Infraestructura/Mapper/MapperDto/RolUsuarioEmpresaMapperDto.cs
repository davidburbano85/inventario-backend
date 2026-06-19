using inventarioWebAI.Aplicacion.Enums;
using inventarioWebAI.Dominio.Enums;

namespace inventarioWebAI.Infraestructura.Mapper.MapperDto;

public static class RolUsuarioEmpresaMapperDto
{
    // DTO → Domain
    public static RolUsuarioEmpresa ToDomain(RolUsuarioEmpresaDTO dto)
    {
        return dto switch
        {
            RolUsuarioEmpresaDTO.Usuario => RolUsuarioEmpresa.Usuario,
            RolUsuarioEmpresaDTO.Admin => RolUsuarioEmpresa.Admin,
            RolUsuarioEmpresaDTO.SuperAdmin => RolUsuarioEmpresa.SuperAdmin,
            _ => throw new ArgumentOutOfRangeException(nameof(dto), "Rol inválido en DTO")
        };
    }

    // Domain → DTO
    public static RolUsuarioEmpresaDTO ToDto(RolUsuarioEmpresa domain)
    {
        return domain switch
        {
            RolUsuarioEmpresa.Usuario => RolUsuarioEmpresaDTO.Usuario,
            RolUsuarioEmpresa.Admin => RolUsuarioEmpresaDTO.Admin,
            RolUsuarioEmpresa.SuperAdmin => RolUsuarioEmpresaDTO.SuperAdmin,
            _ => throw new ArgumentOutOfRangeException(nameof(domain), "Rol inválido en dominio")
        };
    }
}