using inventarioWebAI.Dominio.Entidades;
using System;

namespace inventarioWebAI.Aplicacion.Interfaces.Irepositorios
{
    public interface IAlmacenRepositorio
    {
        // NUEVO: crear un almacén
        Task<Guid> CrearAlmacenAsync(Almacen almacen);

        // NUEVO: obtener almacenes por empresa
       // Task<Almacen?> ObtenerAlmacenPorIdAsync(Guid id, Guid empresaId);
        Task<Almacen?> ObtenerAlmacenActivoPorEmpresaAsync(Guid empresaId);


    }
}
