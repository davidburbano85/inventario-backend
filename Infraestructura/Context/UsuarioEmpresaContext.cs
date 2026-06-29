public class UsuarioEmpresaContext
{
    public Guid UsuarioId { get; set; }
    public Guid EmpresaId { get; set; }
    public bool EsAdmin { get; set; }
    public bool EsSuperAdmin { get; set; }
}