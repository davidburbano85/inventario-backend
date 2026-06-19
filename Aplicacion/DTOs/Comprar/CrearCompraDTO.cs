// Ubicación: /src/Aplicacion/DTOs/CrearCompraDTO.cs

using inventarioWebAI.Aplicacion.DTOs.CompraDetalle;

namespace inventarioWebAI.Aplicacion.DTOs.Comprar;

public class CrearCompraDTO
{
    public Guid EmpresaId { get; set; }

    public Guid? ProveedorId { get; set; }
    public Guid AlmacenId { get; set; }   

    public Guid UsuarioId { get; set; }  

    public List<CrearCompraDetalleDTO> Detalles { get; set; } = new();
}