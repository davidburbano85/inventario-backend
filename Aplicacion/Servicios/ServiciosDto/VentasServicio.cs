using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Aplicacion.Interfaces.Context;
using inventarioWebAI.Dominio.Entidades;
using inventarioWebAI.Dominio.Enums;

namespace inventarioWebAI.Aplicacion.Servicios;

public class VentasServicio : IVentasServicio
{
    private readonly IVentasRepositorio _ventasRepositorio;
    private readonly IStockRepositorio _stockRepositorio;
    private readonly IMovimientosInventarioServicio _movimientosServicio;
    private readonly IUsuarioContext _usuarioContext;
    private readonly IUsuarioEmpresaRepositorio _usuarioEmpresaRepositorio;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VentasServicio> _logger;
    private readonly IAlmacenServicio _almacenServicio;
    private readonly IVentasDetalleRepositorio _ventasDetalleRepositorio;
    private readonly IMovimientosInventarioRepositorio _movimientosInventarioRepositorio;


    public VentasServicio(
        IVentasRepositorio ventasRepositorio,
        IStockRepositorio stockRepositorio,
        IMovimientosInventarioServicio movimientosServicio,
        IUsuarioContext usuarioContext,
        IUsuarioEmpresaRepositorio usuarioEmpresaRepositorio,
        IUnitOfWork unitOfWork,
        ILogger<VentasServicio> logger,
        IAlmacenServicio almacenServicio,
        IVentasDetalleRepositorio ventasDetalleRepositorio,
        IMovimientosInventarioRepositorio movimientosInventarioRepositorio)
    {
        _ventasRepositorio = ventasRepositorio;
        _stockRepositorio = stockRepositorio;
        _movimientosServicio = movimientosServicio;
        _usuarioContext = usuarioContext;
        _usuarioEmpresaRepositorio = usuarioEmpresaRepositorio;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _almacenServicio = almacenServicio;
        _ventasDetalleRepositorio = ventasDetalleRepositorio;
        _movimientosInventarioRepositorio = movimientosInventarioRepositorio;
    }

    public async Task<Guid> CrearAsync(Guid clienteId, List<VentaDetalle> detalles)
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var empresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (empresa == null || !empresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        var uow = _unitOfWork;

        try
        {
            var almacenId = await _almacenServicio.ObtenerAlmacenActivoAsync();

            // =========================
            // VALIDAR STOCK
            // =========================
            foreach (var d in detalles)
            {
                var stock = await _stockRepositorio.ObtenerCantidadAsync(
                    uow.Connection,
                    uow.Transaction,
                    empresa.EmpresaId,
                    d.ProductoId,
                    almacenId
                );

                if (stock == null || stock < d.Cantidad)
                    throw new InvalidOperationException("Stock insuficiente.");
            }

            // =========================
            // TOTAL
            // =========================
            var total = detalles.Sum(x => x.Cantidad * x.Precio);

            // =========================
            // INSERT VENTA
            // =========================
            var ventaId = await _ventasRepositorio.CrearVentaAsync(
                uow.Connection,
                uow.Transaction,
                new Venta
                {
                    EmpresaId = empresa.EmpresaId,
                    ClienteId = clienteId,
                    Total = total
                });

            // =========================
            // INSERT DETALLES
            // =========================
            await _ventasDetalleRepositorio.InsertarAsync(
                uow.Connection,
                uow.Transaction,
                ventaId,
                empresa.EmpresaId,
                detalles
            );

            // =========================
            // MOVIMIENTOS INVENTARIO (SALIDA)
            // =========================
            foreach (var d in detalles)
            {
                await _movimientosInventarioRepositorio.InsertarAsync(
                    uow.Connection,
                    uow.Transaction,
                    new MovimientoInventario
                    {
                        EmpresaId = empresa.EmpresaId,
                        ProductoId = d.ProductoId,
                        AlmacenId = almacenId,
                        UsuarioId = usuarioId,
                        Tipo = TipoMovimiento.Salida,
                        Cantidad = d.Cantidad,
                        Motivo = "Venta"
                    }
                );
            }

            // =========================
            // COMMIT
            // =========================
            uow.Commit();

            return ventaId;
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }


    public async Task<IEnumerable<Venta>> ObtenerPorEmpresaAsync()
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var empresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        var uow = _unitOfWork;

        return await _ventasRepositorio.ObtenerPorEmpresaAsync(
            uow.Connection,
            uow.Transaction,
            empresa!.EmpresaId
        );
    }

    public async Task<Venta?> ObtenerPorIdAsync(Guid ventaId)
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var empresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        var uow = _unitOfWork;

        return await _ventasRepositorio.ObtenerPorIdAsync(
            uow.Connection,
            uow.Transaction,
            ventaId,
            empresa!.EmpresaId
        );
    }

    public async Task<bool> AnularAsync(Guid ventaId)
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var empresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (empresa == null)
            throw new InvalidOperationException("No hay empresa activa.");

        var uow = _unitOfWork;

        try
        {
            var almacenId = await _almacenServicio.ObtenerAlmacenActivoAsync();

            if (almacenId == Guid.Empty)
                throw new InvalidOperationException("Almacén inválido.");

            var detalles = await _ventasDetalleRepositorio.ObtenerPorVentaAsync(
                uow.Connection,
                uow.Transaction,
                ventaId,
                empresa.EmpresaId
            );

            var result = await _ventasRepositorio.AnularVentaAsync(
                uow.Connection,
                uow.Transaction,
                ventaId,
                empresa.EmpresaId
            );

            if (!result)
            {
                uow.Rollback();
                return false;
            }

            foreach (var d in detalles)
            {
                await _movimientosInventarioRepositorio.InsertarAsync(
                    uow.Connection,
                    uow.Transaction,
                    new MovimientoInventario
                    {
                        EmpresaId = empresa.EmpresaId,
                        ProductoId = d.ProductoId,
                        AlmacenId = almacenId,
                        UsuarioId = usuarioId,
                        Tipo = TipoMovimiento.Entrada,
                        Cantidad = d.Cantidad,
                        Motivo = "Anulación de venta"
                    }
                );
            }

            uow.Commit();
            return true;
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}