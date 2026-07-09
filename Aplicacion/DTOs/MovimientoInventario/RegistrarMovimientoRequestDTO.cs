using inventarioWebAI.Dominio.Enums;

namespace inventarioWebAI.Aplicacion.DTOs.MovimientoInventario
{
    public class RegistrarMovimientoRequestDTO
    {
        public Guid ProductoId { get; set; }
        public Guid AlmacenId { get; set; }
        public decimal Cantidad { get; set; }
        public string? Factura { get; set; } 
        public TipoMovimiento Tipo { get; set; }
        public string? Motivo { get; set; }
    }
}
