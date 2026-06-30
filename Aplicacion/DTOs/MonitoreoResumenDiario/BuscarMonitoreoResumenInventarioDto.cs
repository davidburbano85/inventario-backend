using inventarioWebAI.Aplicacion.Enums;
using inventarioWebAI.Dominio.Enums;

namespace inventarioWebAI.Aplicacion.DTOs.MonitoreoResumenDiario
{
    public class BuscarMonitoreoResumenInventarioDto
    {
        public Guid? EmpresaId { get; set; }

        public Guid? AlmacenId { get; set; }

        public TipoProcesoResumenInventario? TipoProceso { get; set; }

        public EstadoProcesoResumenInventario? Estado { get; set; }

        public DateOnly? FechaDesde { get; set; }

        public DateOnly? FechaHasta { get; set; }
    }
}