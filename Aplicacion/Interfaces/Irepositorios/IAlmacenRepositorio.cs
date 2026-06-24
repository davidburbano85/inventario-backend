using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios
{
    public interface IAlmacenRepositorio
    {
        // NUEVO: crear un almacén
        Task<Guid> CrearAlmacenAsync(Almacen almacen);

        // NUEVO: obtener almacenes por empresa
        Task<IEnumerable<Almacen>> ObtenerAlmacenPorEmpresaIdAsync(Guid empresaId);
   
    }
}
