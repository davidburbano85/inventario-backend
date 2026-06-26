using inventarioWebAI.Dominio.Entidades;
using System;

namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios
{
    public interface IAlmacenRepositorio
    {
        // NUEVO: crear un almacén
        Task<Guid> CrearAlmacenAsync(Almacen almacen);


        Task<IEnumerable<Almacen?>> ObtenerAlmacenesActivosPorEmpresaAsync(Guid empresaId);
        Task<bool> ActualizarAlmacenAsync(Almacen almacen);
        Task<Almacen?> ObtenerAlmacenPorIdAsync(Guid id, Guid empresaId);
        Task<bool> EliminarAlmacenAsync(Guid id, Guid empresaId);

    }
}
