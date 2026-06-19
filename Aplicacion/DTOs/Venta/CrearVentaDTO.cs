// Ubicación: /src/Aplicacion/DTOs/CrearVentaDTO.cs

namespace inventarioWebAI.Aplicacion.DTOs.Venta;

public class CrearVentaDTO
{
    public Guid EmpresaId { get; set; }

    public Guid? ClienteId { get; set; }
    public Guid AlmacenId { get; set; }

    public Guid UsuarioId { get; set; }



    public List<CrearVentaDetalleDTO> Detalles { get; set; } = new();
}

