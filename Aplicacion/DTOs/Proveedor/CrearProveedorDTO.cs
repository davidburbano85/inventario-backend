namespace inventarioWebAI.Aplicacion.DTOs.Proveedor;

public class CrearProveedorDTO
{
    public Guid EmpresaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Contacto { get; set; }
}