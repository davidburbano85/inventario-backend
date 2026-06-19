namespace inventarioWebAI.Aplicacion.DTOs.Categoria;

public class CategoriaDTO
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
}