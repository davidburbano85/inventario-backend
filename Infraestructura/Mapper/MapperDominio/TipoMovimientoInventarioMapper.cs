using inventarioWebAI.Dominio.Enums;

namespace inventarioWebAI.Infraestructura.Mapper.MapperDominio;

public static class TipoMovimientoInventarioMapper
{
    // BD → Domain
    public static TipoMovimiento ToDomain(string tipo)
    {
        return tipo.ToLower() switch
        {
            "entrada" => TipoMovimiento.Entrada,
            "salida" => TipoMovimiento.Salida,
            "ajuste" => TipoMovimiento.Ajuste,
            _ => throw new Exception($"TipoMovimiento inválido en BD: {tipo}")
        };
    }

    // Domain → BD
    public static string ToDb(TipoMovimiento tipo)
    {
        return tipo switch
        {
            TipoMovimiento.Entrada => "entrada",
            TipoMovimiento.Salida => "salida",
            TipoMovimiento.Ajuste => "ajuste",
            _ => throw new Exception($"TipoMovimiento inválido en dominio: {tipo}")
        };
    }
}