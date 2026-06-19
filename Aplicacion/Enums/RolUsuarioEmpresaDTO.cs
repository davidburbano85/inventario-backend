using System.Text.Json.Serialization;

namespace inventarioWebAI.Aplicacion.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]


public enum RolUsuarioEmpresaDTO
{
    Usuario,
    Admin,
    SuperAdmin
}



