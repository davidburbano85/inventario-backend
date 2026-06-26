namespace inventarioWebAI.Aplicacion.Interfaces.IAuth
{
    [Obsolete("Se elimina. Autenticación ahora es Supabase JWT")]
    public interface IJwtServicio
    {
        string generarToken(Guid usuarioId, Guid empresaId);
    }
}
