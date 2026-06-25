namespace inventarioWebAI.Aplicacion.Interfaces.IAuth
{
    public interface IJwtServicio
    {
        string generarToken(Guid usuarioId, Guid empresaId);
    }
}
