namespace inventarioWebAI.Aplicacion.DTOs.ResumenDiarioInventario
{
    public class RecalcularProductoRequestDto
    {
        public Guid AlmacenId { get; set; }
        public Guid ProductoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
