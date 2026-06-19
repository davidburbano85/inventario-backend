namespace inventarioWebAI.Aplicacion.DTOs.ResumenDiarioInventario;

public class ResumenDiarioInventarioDTO
{
    public Guid? ProductoId { get; set; }
    public DateOnly Fecha { get; set; }
    public decimal TotalVentas { get; set; }
    public decimal TotalCompras { get; set; }
}