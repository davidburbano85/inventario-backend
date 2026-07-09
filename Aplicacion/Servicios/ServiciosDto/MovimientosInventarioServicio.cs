using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Aplicacion.Interfaces.Context;
using inventarioWebAI.Dominio.Enums;
using inventarioWebAI.Dominio.Entidades;
using Microsoft.Extensions.Logging;

namespace inventarioWebAI.Aplicacion.Servicios;

public class MovimientosInventarioServicio : IMovimientosInventarioServicio
{
    private readonly IMovimientosInventarioRepositorio _repo;
    private readonly IUsuarioContext _usuarioContext;
    private readonly IUsuarioEmpresaRepositorio _usuarioEmpresaRepositorio;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MovimientosInventarioServicio> _logger;
    private readonly IStockRepositorio _stockRepositorio;

    public MovimientosInventarioServicio(
        IMovimientosInventarioRepositorio repo,
        IUsuarioContext usuarioContext,
        IUsuarioEmpresaRepositorio usuarioEmpresaRepositorio,
        IUnitOfWork unitOfWork,
        ILogger<MovimientosInventarioServicio> logger,
        IStockRepositorio stockRepositorio)
    {
        _repo = repo;
        _usuarioContext = usuarioContext;
        _usuarioEmpresaRepositorio = usuarioEmpresaRepositorio;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _stockRepositorio = stockRepositorio;
    }

    public async Task<Guid> RegistrarAsync(
       Guid productoId,
       Guid almacenId,
       decimal cantidad,
       TipoMovimiento tipo,
       string motivo,
       string factura)
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var empresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (empresa == null || !empresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        var movimiento = new MovimientoInventario
        {
            EmpresaId = empresa.EmpresaId,
            ProductoId = productoId,
            AlmacenId = almacenId,
            UsuarioId = usuarioId,
            Tipo = tipo,
            Cantidad = cantidad,
            Motivo = motivo,
            Factura = factura,
        };

        try
        {
            var movimientoId = await _repo.InsertarAsync(
                _unitOfWork.Connection,
                _unitOfWork.Transaction,
                movimiento);

            var stockActual = await _stockRepositorio.ObtenerCantidadAsync(
                _unitOfWork.Connection,
                _unitOfWork.Transaction,
                empresa.EmpresaId,
                productoId,
                almacenId);

            if (stockActual == null)
            {
                await _stockRepositorio.CrearRegistroAsync(
                    _unitOfWork.Connection,
                    _unitOfWork.Transaction,
                    empresa.EmpresaId,
                    productoId,
                    almacenId,
                    tipo == TipoMovimiento.Salida ? -cantidad : cantidad);
            }
            else
            {
                switch (tipo)
                {
                    case TipoMovimiento.Entrada:

                        await _stockRepositorio.IncrementarStockAsync(
                            _unitOfWork.Connection,
                            _unitOfWork.Transaction,
                            empresa.EmpresaId,
                            productoId,
                            almacenId,
                            cantidad);
                        break;

                    case TipoMovimiento.Salida:

                        await _stockRepositorio.DisminuirStockAsync(
                            _unitOfWork.Connection,
                            _unitOfWork.Transaction,
                            empresa.EmpresaId,
                            productoId,
                            almacenId,
                            cantidad);
                        break;

                    case TipoMovimiento.Ajuste:

                        if (cantidad >= 0)
                        {
                            await _stockRepositorio.IncrementarStockAsync(
                                _unitOfWork.Connection,
                                _unitOfWork.Transaction,
                                empresa.EmpresaId,
                                productoId,
                                almacenId,
                                cantidad);
                        }
                        else
                        {
                            await _stockRepositorio.DisminuirStockAsync(
                                _unitOfWork.Connection,
                                _unitOfWork.Transaction,
                                empresa.EmpresaId,
                                productoId,
                                almacenId,
                                Math.Abs(cantidad));
                        }
                        break;
                }
            }

            return movimientoId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registrando movimiento de inventario");
            throw;
        }
    }




    public async Task<IEnumerable<MovimientoInventario>> ObtenerPorEmpresaAsync()
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var empresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (empresa == null || !empresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        return await _repo.ObtenerPorEmpresaAsync(
            _unitOfWork.Connection,
            _unitOfWork.Transaction,
            empresa.EmpresaId);
    }

    public async Task<IEnumerable<MovimientoInventario>> ObtenerPorProductoAsync(Guid productoId)
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var empresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (empresa == null || !empresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        return await _repo.ObtenerPorProductoAsync(
            _unitOfWork.Connection,
            _unitOfWork.Transaction,
            empresa.EmpresaId,
            productoId);
    }

    public async Task<IEnumerable<MovimientoInventario>> ObtenerPorAlmacenAsync(Guid almacenId)
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var empresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (empresa == null || !empresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        return await _repo.ObtenerPorAlmacenAsync(
            _unitOfWork.Connection,
            _unitOfWork.Transaction,
            empresa.EmpresaId,
            almacenId);
    }

    public async Task<IEnumerable<MovimientoInventario>> FiltrarAsync(
        Guid? productoId = null,
        Guid? almacenId = null,
        string? tipo = null)
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var empresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (empresa == null || !empresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        return await _repo.FiltrarAsync(
            _unitOfWork.Connection,
            _unitOfWork.Transaction,
            empresa.EmpresaId,
            productoId,
            almacenId,
            tipo);
    }

    public async Task<MovimientoInventario?> EncontrarPorFacturaAsync(
    string factura)
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var empresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (empresa == null || !empresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        return await _repo.EncontrarPorFacturaAsync(
            _unitOfWork.Connection,
            _unitOfWork.Transaction,
            empresa.EmpresaId,
            factura
        );
    }

}