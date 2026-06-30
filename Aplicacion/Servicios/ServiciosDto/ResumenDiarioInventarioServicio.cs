using Dapper;
using inventarioWebAI.Aplicacion.Enums;
using inventarioWebAI.Aplicacion.Interfaces.Context;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Dominio.Entidades;
using Microsoft.Extensions.Logging;
using System.Data;
using inventarioWebAI.Dominio.Enums;

namespace inventarioWebAI.Aplicacion.Servicios
{
    public class ResumenDiarioInventarioServicio : IResumenDiarioInventarioServicio
    {
        private readonly IResumenDiarioInventarioRepositorio _repositorio;
        private readonly IUsuarioEmpresaContextService _contextService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ResumenDiarioInventarioServicio> _logger;
        private readonly IMonitoreoResumenInventarioRepositorio _monitoreoRepo;

        public ResumenDiarioInventarioServicio(
            IResumenDiarioInventarioRepositorio repositorio,
            IUsuarioEmpresaContextService contextService,
            IUnitOfWork unitOfWork,
            ILogger<ResumenDiarioInventarioServicio> logger,
            IMonitoreoResumenInventarioRepositorio monitoreoRepo
            )
        {
            _repositorio = repositorio;
            _contextService = contextService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _monitoreoRepo = monitoreoRepo;
        }

        public async Task<int> GenerarResumenDiarioAsync(Guid almacenId, DateTime fecha)
        {
            var ctx = await _contextService.GetAsync();

            var uow = _unitOfWork;

            return await _repositorio.GenerarResumenDiarioAsync(
                uow.Connection,
                uow.Transaction,
                ctx.EmpresaId,
                almacenId,
                fecha
            );
        }

        public async Task<int> GenerarResumenPorRangoAsync(Guid almacenId, DateTime fechaInicio, DateTime fechaFin)
        {
            var ctx = await _contextService.GetAsync();

            var uow = _unitOfWork;

            return await _repositorio.GenerarResumenPorRangoAsync(
                uow.Connection,
                uow.Transaction,
                ctx.EmpresaId,
                almacenId,
                fechaInicio,
                fechaFin
            );
        }

        public async Task<IEnumerable<ResumenDiarioInventario>> ObtenerPorRangoAsync(
            Guid almacenId,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            var ctx = await _contextService.GetAsync();

            var uow = _unitOfWork;

            return await _repositorio.ObtenerPorRangoAsync(
                uow.Connection,
                uow.Transaction,
                ctx.EmpresaId,
                almacenId,
                fechaInicio,
                fechaFin
            );
        }

        public async Task<ResumenDiarioInventario?> ObtenerPorProductoYFechaAsync(
            Guid almacenId,
            Guid productoId,
            DateTime fecha)
        {
            var ctx = await _contextService.GetAsync();

            var uow = _unitOfWork;

            return await _repositorio.ObtenerPorProductoYFechaAsync(
                uow.Connection,
                uow.Transaction,
                ctx.EmpresaId,
                almacenId,
                productoId,
                fecha
            );
        }

        public async Task<bool> RecalcularProductoAsync(
            Guid almacenId,
            Guid productoId,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            var ctx = await _contextService.GetAsync();

            var uow = _unitOfWork;

            try
            {
                await _repositorio.RecalcularProductoAsync(
                    uow.Connection,
                    uow.Transaction,
                    ctx.EmpresaId,
                    almacenId,
                    productoId,
                    fechaInicio,
                    fechaFin
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error recalculando resumen diario. Empresa: {EmpresaId} Almacen: {AlmacenId} Producto: {ProductoId}",
                    ctx.EmpresaId,
                    almacenId,
                    productoId);

                throw;
            }
        }

        public async Task<Guid> EjecutarDiarioAsync(Guid empresaId, Guid? almacenId = null)
        {
            var conn = _unitOfWork.Connection;
            var tx = _unitOfWork.Transaction;

            Guid monitoreoId = Guid.Empty;

            try
            {
                // 1. Registrar inicio del proceso en monitoreo
                monitoreoId = await _monitoreoRepo.CrearAsync(conn, tx, new MonitoreoResumenInventario
                {
                    EmpresaId = empresaId,
                    AlmacenId = almacenId,
                    TipoProceso = TipoProcesoResumenInventario.Diario,
                    FechaDesde = DateTime.UtcNow.Date,
                    FechaHasta = DateTime.UtcNow.Date,
                    Estado = EstadoProcesoResumenInventario.EnProceso,
                    RegistrosProcesados = 0,
                    Mensaje = "Inicio proceso diario"
                });

                // 2. Ejecutar función en base de datos (fuente de verdad)
                await conn.ExecuteAsync(
                    "SELECT generar_resumen_diario_inventario();",
                    transaction: tx
                );

                // 3. Marcar éxito (la BD ya hizo el cálculo)
                await _monitoreoRepo.MarcarExitosoAsync(conn, tx, monitoreoId, 0);

                // 4. Confirmar transacción completa
                _unitOfWork.Commit();

                return monitoreoId;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();

                // Intentar registrar fallo si alcanzó a crearse el monitoreo
                try
                {
                    if (monitoreoId != Guid.Empty)
                    {
                        await _monitoreoRepo.MarcarFallidoAsync(
                            conn,
                            tx,
                            monitoreoId,
                            ex.Message
                        );
                    }
                }
                catch
                {
                    // evitar doble excepción
                }

                throw;
            }
        }


    }
}