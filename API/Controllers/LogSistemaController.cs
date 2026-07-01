using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inventarioWebAI.API.Controllers;

[ApiController]
[Route("api/logs-sistema")]
[Authorize]
public class LogSistemaController : ControllerBase
{
    private readonly ILogSistemaServicio _logServicio;

    public LogSistemaController(
        ILogSistemaServicio logServicio)
    {
        _logServicio = logServicio;
    }


    // Consulta los logs de la empresa activa.
    // La empresa se determina desde el contexto JWT,
    // nunca desde parámetros enviados por el cliente.
    [HttpGet]
    public async Task<IActionResult> ObtenerPorEmpresa()
    {
        var logs = await _logServicio.ObtenerPorEmpresaAsync();

        return Ok(logs);
    }


    // Consulta un log específico validando internamente
    // que pertenezca a la empresa activa.
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var log = await _logServicio.ObtenerPorIdAsync(id);

        if (log == null)
            return NotFound();

        return Ok(log);
    }
}