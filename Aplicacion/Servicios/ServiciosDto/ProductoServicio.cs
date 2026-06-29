using inventarioWebAI.Aplicacion.DTOs.Producto;
using inventarioWebAI.Aplicacion.Interfaces.Context;
using inventarioWebAI.Aplicacion.Interfaces.IPermmisoServicios;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Infraestructura.AccesoDatos;

namespace inventarioWebAI.Aplicacion.Servicios;

public class ProductoServicio : IProductoServicio
{
    private readonly IProductoRepositorio _productoRepositorio;
    private readonly ICategoriaRepositorio _categoriaRepositorio;
    private readonly IPermisoServicio _permisoServicio;
    private readonly IUsuarioContext _usuarioContext;
    private readonly IUsuarioEmpresaRepositorio _usuarioEmpresaRepositorio;
    private readonly IEmpresaRepositorio _empresaRepositorio;
    private readonly IStockRepositorio _stockRepositorio;
    private readonly IUnitOfWork _unitOfWork;

    public ProductoServicio(
        IProductoRepositorio productoRepositorio,
        ICategoriaRepositorio categoriaRepositorio,
        IPermisoServicio permisoServicio,
        IUsuarioContext usuarioContext,
        IUsuarioEmpresaRepositorio usuarioEmpresaRepositorio,
        IEmpresaRepositorio empresaRepositorio,
        IUnitOfWork unitOfWork)
    {
        _productoRepositorio = productoRepositorio;
        _categoriaRepositorio = categoriaRepositorio;
        _permisoServicio = permisoServicio;
        _usuarioContext = usuarioContext;
        _usuarioEmpresaRepositorio = usuarioEmpresaRepositorio;
        _empresaRepositorio = empresaRepositorio;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> CrearProductoAsync(string nombre, string? codigoSku, decimal precioVenta, decimal? precioCompra, Guid categoriaId)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new InvalidOperationException("El nombre del producto es obligatorio.");

        if (precioVenta < 0)
            throw new InvalidOperationException("El precio de venta no puede ser negativo.");

        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        if (usuarioId == Guid.Empty)
            throw new InvalidOperationException("Usuario inválido.");

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null)
            throw new InvalidOperationException("El usuario no tiene empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        var empresa = await _empresaRepositorio.ObtenerEmpresaPorIdAsync(empresaId);

        if (empresa == null)
            throw new InvalidOperationException("La empresa no existe.");

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var categoria = await _categoriaRepositorio.ObtenerCategoriaPorIdAsync(categoriaId, empresaId);

        if (categoria == null)
            throw new InvalidOperationException("La categoría no existe o no pertenece a la empresa.");

        var producto = new Producto
        {
            EmpresaId = empresaId,
            CategoriaId = categoriaId,
            Nombre = nombre.Trim(),
            CodigoSku = codigoSku,
            PrecioVenta = precioVenta,
            PrecioCompra = precioCompra
        };

        return await _productoRepositorio.CrearProductoAsync(producto);
    }

    public async Task<IEnumerable<ProductoDTO>> ObtenerProductosActivosPorEmpresaAsync()
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        if (usuarioId == Guid.Empty)
            throw new InvalidOperationException("Usuario inválido.");

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null || !usuarioEmpresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var productos = await _productoRepositorio.ObtenerProductosActivosPorEmpresaAsync(empresaId);

        if (productos == null)
            return Enumerable.Empty<ProductoDTO>();

        return productos.Select(p => new ProductoDTO
        {
            Id = p!.Id,
            EmpresaId = p.EmpresaId,
            CategoriaId = p.CategoriaId,
            Nombre = p.Nombre,
            CodigoSku = p.CodigoSku,
            PrecioVenta = p.PrecioVenta,
            PrecioCompra = p.PrecioCompra,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            Activo = p.Activo
        });
    }

    public async Task<IEnumerable<ProductoDTO>> ObtenerProductosPorCategoriaAsync(Guid categoriaId)
    {
        if (categoriaId == Guid.Empty)
            throw new InvalidOperationException("Categoría inválida.");

        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null)
            throw new InvalidOperationException("No hay empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var productos = await _productoRepositorio.ObtenerProductosPorCategoriaAsync(empresaId, categoriaId);

        if (productos == null)
            return Enumerable.Empty<ProductoDTO>();

        return productos.Select(p => new ProductoDTO
        {
            Id = p!.Id,
            EmpresaId = p.EmpresaId,
            CategoriaId = p.CategoriaId,
            Nombre = p.Nombre,
            CodigoSku = p.CodigoSku,
            PrecioVenta = p.PrecioVenta,
            PrecioCompra = p.PrecioCompra,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            Activo = p.Activo
        });
    }

    public async Task<ProductoDTO?> ObtenerProductoPorIdAsync(Guid productoId)
    {
        if (productoId == Guid.Empty)
            throw new InvalidOperationException("Producto inválido.");

        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null)
            throw new InvalidOperationException("No hay empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var producto = await _productoRepositorio.ObtenerProductoPorIdAsync(productoId, empresaId);

        if (producto == null)
            return null;

        return new ProductoDTO
        {
            Id = producto.Id,
            EmpresaId = producto.EmpresaId,
            CategoriaId = producto.CategoriaId,
            Nombre = producto.Nombre,
            CodigoSku = producto.CodigoSku,
            PrecioVenta = producto.PrecioVenta,
            PrecioCompra = producto.PrecioCompra,
            CreatedAt = producto.CreatedAt,
            UpdatedAt = producto.UpdatedAt,
            Activo = producto.Activo
        };
    }

    public async Task<bool> ActualizarProductoAsync(Guid productoId, string nombre, string? codigoSku, decimal precioVenta, decimal? precioCompra, Guid categoriaId)
    {
        if (productoId == Guid.Empty)
            throw new InvalidOperationException("Producto inválido.");

        if (string.IsNullOrWhiteSpace(nombre))
            throw new InvalidOperationException("Nombre obligatorio.");

        if (precioVenta < 0)
            throw new InvalidOperationException("Precio inválido.");

        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null)
            throw new InvalidOperationException("No hay empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var categoria = await _categoriaRepositorio.ObtenerCategoriaPorIdAsync(categoriaId, empresaId);

        if (categoria == null)
            throw new InvalidOperationException("Categoría inválida.");

        var productoExistente = await _productoRepositorio.ObtenerProductoPorIdAsync(productoId, empresaId);

        if (productoExistente == null)
            throw new InvalidOperationException("El producto no existe.");

        var producto = new Producto
        {
            Id = productoId,
            EmpresaId = empresaId,
            CategoriaId = categoriaId,
            Nombre = nombre.Trim(),
            CodigoSku = codigoSku,
            PrecioVenta = precioVenta,
            PrecioCompra = precioCompra
        };

        return await _productoRepositorio.ActualizarProductoAsync(producto);
    }

    public async Task<bool> EliminarProductoAsync(Guid productoId)
    {
        if (productoId == Guid.Empty)
            throw new InvalidOperationException("Producto inválido.");

        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var usuarioEmpresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (usuarioEmpresa == null)
            throw new InvalidOperationException("No hay empresa activa.");

        var empresaId = usuarioEmpresa.EmpresaId;

        await _permisoServicio.ValidarAdminOSuperAdminAsync(usuarioId, empresaId);

        var producto = await _productoRepositorio.ObtenerProductoPorIdAsync(productoId, empresaId);

        if (producto == null)
            return false;

        return await _productoRepositorio.EliminarProductoAsync(productoId, empresaId);
    }

    /// Devuelve el stock actual de un producto en un almacén.
    public async Task<decimal> ObtenerStockAsync(Guid productoId, Guid almacenId)
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var empresa = await _usuarioEmpresaRepositorio.ObtenerEmpresaActivaAsync(usuarioId);

        if (empresa == null || !empresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        var stock = await _stockRepositorio.ObtenerCantidadAsync(
            _unitOfWork.Connection,
            _unitOfWork.Transaction,
            empresa.EmpresaId,
            productoId,
            almacenId
        );

        return stock ?? 0;
    }


}