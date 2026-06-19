using inventarioWebAI.Dominio.Enums;

namespace inventarioWebAI.Infraestructura.Mapper;

public static class RolUsuarioEmpresaMapper
{
    // BD → Domain
    public static RolUsuarioEmpresa ToDomain(string rol)
    {
        return rol.ToLower() switch
        {
            "admin" => RolUsuarioEmpresa.Admin,
            "usuario" => RolUsuarioEmpresa.Usuario,
            "superadmin" => RolUsuarioEmpresa.SuperAdmin,
            _ => throw new Exception($"Rol inválido en BD: {rol}")
        };
    }

    // Domain → BD
    public static string ToDb(RolUsuarioEmpresa rol)
    {
        return rol switch
        {
            RolUsuarioEmpresa.Admin => "admin",
            RolUsuarioEmpresa.Usuario => "usuario",
            RolUsuarioEmpresa.SuperAdmin => "superadmin",
            _ => throw new Exception($"Rol inválido en dominio: {rol}")
        };
    }
}