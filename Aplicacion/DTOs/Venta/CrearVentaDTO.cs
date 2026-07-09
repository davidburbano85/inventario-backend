// Ubicación: /src/Aplicacion/DTOs/CrearVentaDTO.cs

namespace inventarioWebAI.Aplicacion.DTOs.Venta;

public class CrearVentaDTO
{

    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }

    public Guid? ClienteId { get; set; }
    public Guid AlmacenId { get; set; }

    public Guid UsuarioId { get; set; }
    public string Factura { get; set; } = string.Empty;



    public List<CrearVentaDetalleDTO> Detalles { get; set; } = new();
}

