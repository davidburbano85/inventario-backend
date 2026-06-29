using inventarioWebAI.Aplicacion.DTOs.CompraDetalle;
using inventarioWebAI.Aplicacion.DTOs.Comprar;
using inventarioWebAI.Aplicacion.Interfaces.Context;
using inventarioWebAI.Aplicacion.Interfaces.IPermmisoServicios;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Dominio.Entidades;
using inventarioWebAI.Dominio.Enums;

namespace inventarioWebAI.Aplicacion.Servicios;

public class ComprasServicio : IComprasServicio
{
    private readonly IComprasRepositorio _comprasRepositorio;
    private readonly ICompraDetalleRepositorio _detalleRepositorio;
    private readonly IUsuarioContext _usuarioContext;
    private readonly IUsuarioEmpresaRepositorio _usuarioEmpresaRepositorio;
    private readonly IPermisoServicio _permisoServicio;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMovimientosInventarioServicio _movimientosServicio;
    private readonly IAlmacenRepositorio _almacenRepositorio;
    private readonly IAlmacenServicio _almacenServicio;

    public ComprasServicio(
        IComprasRepositorio comprasRepositorio,
        ICompraDetalleRepositorio detalleRepositorio,
        IUsuarioContext usuarioContext,
        IUsuarioEmpresaRepositorio usuarioEmpresaRepositorio,
        IPermisoServicio permisoServicio,
        IUnitOfWork unitOfWork,
        IMovimientosInventarioServicio movimientosServicio,
        IAlmacenRepositorio almacenRepositorio,
        IAlmacenServicio almacenServicio
        )
    {
        _comprasRepositorio = comprasRepositorio;
        _detalleRepositorio = detalleRepositorio;
        _usuarioContext = usuarioContext;
        _usuarioEmpresaRepositorio = usuarioEmpresaRepositorio;
        _permisoServicio = permisoServicio;
        _unitOfWork = unitOfWork;
        _movimientosServicio = movimientosServicio;
        _almacenRepositorio = almacenRepositorio;
        _almacenServicio = almacenServicio;
    }

    public async Task<Guid> CrearAsync(Guid proveedorId, List<CrearCompraDetalleDTO> detalles)
    {
        if (proveedorId == Guid.Empty)
            throw new InvalidOperationException("Proveedor inválido.");

        if (detalles == null || !detalles.Any())
            throw new InvalidOperationException("La compra debe tener detalles.");

        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var empresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (empresa == null || !empresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresa.EmpresaId);

        var almacenId = await _almacenServicio.ObtenerAlmacenActivoAsync();

        var total = detalles.Sum(d => d.Cantidad * d.Precio);

        var compra = new Compra
        {
            EmpresaId = empresa.EmpresaId,
            ProveedorId = proveedorId,
            Total = total
        };

        var detalleEntities = detalles.Select(d => new CompraDetalle
        {
            ProductoId = d.ProductoId,
            Cantidad = d.Cantidad,
            Precio = d.Precio
        });

        var uow = _unitOfWork;

        try
        {
            var compraId = await _comprasRepositorio.CrearCompraAsync(
                uow.Connection,
                uow.Transaction,
                compra
            );

            await _detalleRepositorio.InsertarAsync(
                uow.Connection,
                uow.Transaction,
                compraId,
                empresa.EmpresaId,
                detalleEntities
            );

            foreach (var d in detalleEntities)
            {
                await _movimientosServicio.RegistrarAsync(
                    d.ProductoId,
                    almacenId,
                    d.Cantidad,
                    TipoMovimiento.Entrada,
                    $"COMPRA: {compraId}"
                );
            }

            uow.Commit();
            return compraId;
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }


    public async Task<IEnumerable<CompraDTO>> ObtenerPorEmpresaAsync()
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var empresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (empresa == null || !empresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        var uow = _unitOfWork;

        var compras = await _comprasRepositorio.ObtenerPorEmpresaAsync(
            uow.Connection,
            uow.Transaction,
            empresa.EmpresaId
        );

        return compras.Select(c => new CompraDTO
        {
            Id = c.Id,
            EmpresaId = c.EmpresaId,
            ProveedorId = c.ProveedorId,
            Total = c.Total,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            Activo = c.Activo
        });
    }
    public async Task<CompraDTO?> ObtenerPorIdAsync(Guid compraId)
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var empresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (empresa == null || !empresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        var uow = _unitOfWork;

        var compra = await _comprasRepositorio.ObtenerPorIdAsync(
            uow.Connection,
            uow.Transaction,
            compraId,
            empresa.EmpresaId
        );

        if (compra == null)
            return null;

        return new CompraDTO
        {
            Id = compra.Id,
            EmpresaId = compra.EmpresaId,
            ProveedorId = compra.ProveedorId,
            Total = compra.Total,
            CreatedAt = compra.CreatedAt,
            UpdatedAt = compra.UpdatedAt,
            Activo = compra.Activo
        };
    }
    public async Task<IEnumerable<CompraDetalleDTO>> ObtenerDetalleAsync(Guid compraId)
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var empresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (empresa == null || !empresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        var uow = _unitOfWork;

        var detalles = await _detalleRepositorio.ObtenerPorCompraAsync(
            uow.Connection,
            uow.Transaction,
            compraId,
            empresa.EmpresaId
        );

        return detalles.Select(d => new CompraDetalleDTO
        {
            Id = d.Id,
            EmpresaId = d.EmpresaId,
            CompraId = d.CompraId,
            ProductoId = d.ProductoId,
            Cantidad = d.Cantidad,
            Precio = d.Precio,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt,
            Activo = d.Activo
        });
    }
    public async Task<bool> AnularAsync(Guid compraId)
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var empresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (empresa == null || !empresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresa.EmpresaId);

        var uow = _unitOfWork;

        var result = await _comprasRepositorio.AnularCompraAsync(
            uow.Connection,
            uow.Transaction,
            compraId,
            empresa.EmpresaId
        );

        if (result)
            uow.Commit();
        else
            uow.Rollback();

        return result;
    }



}