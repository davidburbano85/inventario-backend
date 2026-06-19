//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using System.Security.Claims;
//using inventarioWebAI.Aplicacion.DTOs;
//using inventarioWebAI.Aplicacion.Interfaces.Iservicios;

//namespace inventarioWebAI.API.Controllers;

//[ApiController]
//[Route("api/stock")]
//[Authorize]
//public class StockController : ControllerBase
//{
//    private readonly IStockActualServicio _stockQuery;
//    private readonly IStockServicio _stockCmd;

//    public StockController(IStockActualServicio stockQuery, IStockServicio stockCmd)
//    {
//        _stockQuery = stockQuery;
//        _stockCmd = stockCmd;
//    }

//    private Guid? GetRequesterId()
//    {
//        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//        if (Guid.TryParse(sub, out var id)) return id;
//        return null;
//    }

//    [HttpGet("empresa/{empresaId}")]
//    public async Task<IActionResult> ObtenerPorEmpresa(Guid empresaId)
//    {
//        var result = await _stockQuery.ObtenerPorEmpresa(empresaId);
//        return Ok(result);
//    }

//    [HttpGet("producto/{empresaId}/{productoId}")]
//    public async Task<IActionResult> ObtenerPorProducto(Guid empresaId, Guid productoId)
//    {
//        var result = await _stockQuery.ObtenerPorProducto(empresaId, productoId);
//        return Ok(result);
//    }

//    [HttpPost("inicial")]
//    public async Task<IActionResult> CrearInicial([FromBody] CrearMovimientoDTO dto)
//    {
//        var requester = GetRequesterId();
//        if (requester is null) return Unauthorized();

//        if (!ModelState.IsValid) return BadRequest(ModelState);

//        dto.UsuarioId = requester.Value;
//        var id = await _stockCmd.AjustarStockManual(dto);
//        return Ok(new { id });
//    }
//}
