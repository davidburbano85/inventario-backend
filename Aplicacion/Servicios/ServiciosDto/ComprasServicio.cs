using inventarioWebAI.Aplicacion.DTOs.CompraDetalle;
using inventarioWebAI.Aplicacion.DTOs.Comprar;
using inventarioWebAI.Aplicacion.Interfaces.Context;
using inventarioWebAI.Aplicacion.Interfaces.IPermmisoServicios;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Servicios;

public class ComprasServicio : IComprasServicio
{
    private readonly IComprasRepositorio _comprasRepositorio;
    private readonly ICompraDetalleRepositorio _detalleRepositorio;
    private readonly IUsuarioContext _usuarioContext;
    private readonly IUsuarioEmpresaRepositorio _usuarioEmpresaRepositorio;
    private readonly IPermisoServicio _permisoServicio;
    private readonly IUnitOfWork _unitOfWork ;

    public ComprasServicio(
        IComprasRepositorio comprasRepositorio,
        ICompraDetalleRepositorio detalleRepositorio,
        IUsuarioContext usuarioContext,
        IUsuarioEmpresaRepositorio usuarioEmpresaRepositorio,
        IPermisoServicio permisoServicio,
        IUnitOfWork unitOfWork)
    {
        _comprasRepositorio = comprasRepositorio;
        _detalleRepositorio = detalleRepositorio;
        _usuarioContext = usuarioContext;
        _usuarioEmpresaRepositorio = usuarioEmpresaRepositorio;
        _permisoServicio = permisoServicio;
        _unitOfWork = unitOfWork;
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

        using var uow = _unitOfWork;

        try
        {
            // 🔥 1. CREAR COMPRA dentro de la transacción
            var compraId = await _comprasRepositorio.CrearCompraAsync(
                uow.Connection,
                uow.Transaction,
                compra
            );

            // 🔥 2. CREAR DETALLES dentro de la misma transacción
            await _detalleRepositorio.InsertarAsync(
                uow.Connection,
                uow.Transaction,
                compraId,
                empresa.EmpresaId,
                detalleEntities
            );

            // 🔥 3. CONFIRMAR TODO
            uow.Commit();

            return compraId;
        }
        catch
        {
            // 🔥 4. ROLLBACK TOTAL
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

        var compras = await _comprasRepositorio.ObtenerPorEmpresaAsync(empresa.EmpresaId);

        return compras.Select(c => new CompraDTO
        {
            Id = c!.Id,
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

        var compra = await _comprasRepositorio.ObtenerPorIdAsync(compraId, empresa.EmpresaId);

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

        var detalles = await _detalleRepositorio.ObtenerPorCompraAsync(compraId, empresa.EmpresaId);

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

        return await _comprasRepositorio.AnularCompraAsync(compraId, empresa.EmpresaId);
    }
}