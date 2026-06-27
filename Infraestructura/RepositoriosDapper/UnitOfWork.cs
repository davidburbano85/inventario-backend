using System.Data;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;

namespace inventarioWebAI.Infraestructura.AccesoDatos;

public class UnitOfWork : IUnitOfWork
{
    public IDbConnection Connection { get; }
    public IDbTransaction Transaction { get; private set; }

    public UnitOfWork(IDbConnectionFactory factory)
    {
        Connection = factory.CrearConexion();
        Connection.Open();
        Transaction = Connection.BeginTransaction();
    }

    public void Commit()
    {
        Transaction?.Commit();
        Dispose();
    }

    public void Rollback()
    {
        Transaction?.Rollback();
        Dispose();
    }

    public void Dispose()
    {
        Transaction?.Dispose();
        Connection?.Dispose();
    }
}