namespace inventarioWebAI.Aplicacion.Interfaces.Context
{
    public interface IUsuarioContext
    {
        Guid ObtenerAuthUserId();
        Guid ObtenerEmpresaId();
    }
}
