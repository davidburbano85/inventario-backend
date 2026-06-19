using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios
{
    public interface IEmpresaRepositorio
    {
        Task<Guid>CrearEmpresaAsync(Empresa empresa);
        Task<IEnumerable<Empresa>> ObtenerEmpresaPorUsuarioAsync(Guid usuarioId);
        Task<Empresa?> ObtenerEmpresaPorIdAsync(Guid empresaId);
        Task<Empresa>ActualizarEmpresaAsync( Empresa empresa);
        Task<Empresa>EliminarEmpresaAsync(Guid empresaId);
    }
}
