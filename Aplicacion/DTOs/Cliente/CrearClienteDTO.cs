namespace inventarioWebAI.Aplicacion.DTOs.Cliente;

public class CrearClienteDTO
{
    public Guid EmpresaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Contacto { get; set; }
}