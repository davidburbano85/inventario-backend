using inventarioWebAI.Dominio.Enums;

namespace inventarioWebAI.Infraestructura.Mapper.MapperDto;

public static class TipoMovimientoInventarioMapperDto
{
    // DTO → Domain
    public static TipoMovimiento ToDomain(TipoMovimiento dto)
    {
        return dto switch
        {
            TipoMovimiento.Entrada => TipoMovimiento.Entrada,
            TipoMovimiento.Salida => TipoMovimiento.Salida,
            TipoMovimiento.Ajuste => TipoMovimiento.Ajuste,
            _ => throw new ArgumentOutOfRangeException(nameof(dto), "TipoMovimiento inválido en DTO")
        };
    }

    // Domain → DTO
    public static TipoMovimiento ToDto(TipoMovimiento domain)
    {
        return domain switch
        {
            TipoMovimiento.Entrada => TipoMovimiento.Entrada,
            TipoMovimiento.Salida => TipoMovimiento.Salida,
            TipoMovimiento.Ajuste => TipoMovimiento.Ajuste,
            _ => throw new ArgumentOutOfRangeException(nameof(domain), "TipoMovimiento inválido en dominio")
        };
    }
}