using inventarioWebAI.Dominio.Enums;

namespace inventarioWebAI.Dominio.Entidades
{
    public class MonitoreoResumenInventario
    {
        public Guid Id { get; set; }

        public Guid? EmpresaId { get; set; }

        public Guid? AlmacenId { get; set; }

        public TipoProcesoResumenInventario TipoProceso { get; set; }

        public DateTime FechaDesde { get; set; }

        public DateTime FechaHasta { get; set; }

        public EstadoProcesoResumenInventario Estado { get; set; }

        public int RegistrosProcesados { get; set; }

        public string? Mensaje { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public DateTime CreadoEn { get; set; }

        // Se calcula cuando finaliza el proceso.
        // Requiere agregar la columna duracion_segundos en la base de datos.
        public long? DuracionSegundos { get; set; }
    }
}