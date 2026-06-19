//// Ubicación: /src/API/Controllers/ProductoController.cs

//using Microsoft.AspNetCore.Mvc; // Base para construir endpoints HTTP
//using Microsoft.AspNetCore.Authorization;
//using inventarioWebAI.Aplicacion.DTOs;
//using inventarioWebAI.Aplicacion.Interfaces.Iservicios; // DTOs usados en requests/responses

//namespace inventarioWebAI.API.Controllers; // Namespace del controlador

//// Este controlador expone endpoints HTTP para productos.
//// Responsabilidad:
//// - Recibir requests HTTP
//// - Validar entrada básica
//// - Delegar lógica al servicio
//// - Retornar respuestas HTTP
//// NO contiene lógica de negocio → eso vive en el servicio
//[ApiController] // Indica que es un controlador API
//[Route("api/productos")] // Ruta base
//[Authorize]
//public class ProductoController : ControllerBase
//{
//    private readonly IProductoServicio _servicio; // Dependencia del servicio

//    // Inyección de dependencia
//    public ProductoController(IProductoServicio servicio)
//    {
//        _servicio = servicio; // Guardamos servicio
//    }

//    // GET: api/productos/{empresaId}
//    // Obtiene todos los productos de una empresa
//    [HttpGet("{empresaId}")]
//    public async Task<IActionResult> Obtener(Guid empresaId)
//    {
//        var resultado = await _servicio.ObtenerPorEmpresa(empresaId); // Llamamos al servicio

//        return Ok(resultado); // Retornamos 200 con data
//    }

//    // GET: api/productos/{empresaId}/{productoId}
//    // Obtiene un producto específico
//    [HttpGet("{empresaId}/{productoId}")]
//    public async Task<IActionResult> ObtenerPorId(Guid empresaId, Guid productoId)
//    {
//        var producto = await _servicio.ObtenerPorId(empresaId, productoId); // Consultamos

//        if (producto == null) // Si no existe
//            return NotFound(); // 404

//        return Ok(producto); // 200 con data
//    }

//    // POST: api/productos
//    // Crea un nuevo producto
//    [HttpPost]
//    public async Task<IActionResult> Crear([FromBody] CrearProductoDTO dto)
//    {
//        if (!ModelState.IsValid) // Validación básica del modelo
//            return BadRequest(ModelState); // 400

//        if (string.IsNullOrWhiteSpace(dto.Nombre)) // FIX: evita violar CHECK (length(trim(nombre)) > 0) en BD
//            return BadRequest("El nombre del producto es obligatorio.");

//        var id = await _servicio.Crear(dto); // Creamos producto

//        return Ok(new { id }); // Retornamos ID creado
//    }
//}