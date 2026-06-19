using System.Data;

namespace inventarioWebAI.Infraestructura.AccesoDatos
{
    public interface IDbConnectionFactory
    {
        IDbConnection CrearConexion(); // Método que debe implementar cualquier clase que herede de esta interfaz
                                       // para crear y devolver una conexión a la base de datos
    }
}
