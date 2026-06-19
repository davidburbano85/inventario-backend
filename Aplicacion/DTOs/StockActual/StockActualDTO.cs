namespace inventarioWebAI.Aplicacion.DTOs.StockActual;

public class StockActualDTO
{
    public Guid ProductoId { get; set; }
    public Guid AlmacenId { get; set; }
    public decimal Cantidad { get; set; }
}