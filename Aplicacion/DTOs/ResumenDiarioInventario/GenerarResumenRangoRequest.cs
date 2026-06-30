namespace inventarioWebAI.Aplicacion.DTOs.ResumenDiarioInventario
{
    public class GenerarResumenRangoRequestDto
    {
        public Guid AlmacenId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
