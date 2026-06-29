using Microsoft.AspNetCore.Mvc;
using inventarioWebAI.Aplicacion.DTOs.Almacen;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;

[ApiController]
[Route("api/almacenes")]
public class AlmacenController : ControllerBase
{
    private readonly IAlmacenServicio _service;

    public AlmacenController(IAlmacenServicio service)
    {
        _service = service;
    }

    [HttpPost("seleccionar")]
    public async Task<IActionResult> Seleccionar([FromBody] SeleccionarAlmacenRequest request)
    {
        await _service.SeleccionarAlmacenAsync(request.AlmacenId);
        return Ok("Almacén seleccionado");
    }

    [HttpGet("activo")]
    public async Task<IActionResult> ObtenerActivo()
    {
        var id = await _service.ObtenerAlmacenActivoAsync();
        return Ok(id);
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var data = await _service.ObtenerAlmacenesActivosPorEmpresaAsync();
        return Ok(data);
    }
}