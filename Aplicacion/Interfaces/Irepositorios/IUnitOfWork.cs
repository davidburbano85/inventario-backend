using System.Data;

namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios;

public interface IUnitOfWork : IDisposable
{
    IDbConnection Connection { get; }
    IDbTransaction Transaction { get; }

    void Commit();
    void Rollback();
}