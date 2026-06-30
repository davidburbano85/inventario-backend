using inventarioWebAI.Dominio.Enums;

namespace inventarioWebAI.Aplicacion.DTOs.MonitoreoResumenDiario
{
    public class MonitoreoResumenInventarioDto
    {
        public Guid Id { get; set; }

        public Guid? EmpresaId { get; set; }

        public Guid? AlmacenId { get; set; }

        public TipoProcesoResumenInventario TipoProceso { get; set; }

        public DateOnly? FechaDesde { get; set; }

        public DateOnly? FechaHasta { get; set; }

        public EstadoProcesoResumenInventario Estado { get; set; }

        public int RegistrosProcesados { get; set; }

        public string? Mensaje { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public DateTime CreadoEn { get; set; }

        public long? DuracionSegundos { get; set; }
    }
}