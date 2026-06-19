using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using inventarioWebAI.Aplicacion.DTOs.Empresa;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Infraestructura.AccesoDatos;
using Dapper;

namespace inventarioWebAI.API.Controllers;

[ApiController]
[Route("api/empresas")]
//[Authorize]
public class EmpresaController : ControllerBase
{
    private readonly IEmpresaServicio _servicio;
    private readonly IDbConnectionFactory _db;

    public EmpresaController(IEmpresaServicio servicio, IDbConnectionFactory db)
    {
        _servicio = servicio;
        _db = db;
    }
    [HttpPost]
    public async Task<IActionResult> Crear([FromQuery]Guid usuarioId, [FromBody] EmpresaDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);// Validación de modelo  el modelo no es válido,
                                                               // se devuelve un error 400 con los detalles de la validación el modelo es la representación de los datos que se esperan en la solicitud, y ModelState contiene la información sobre si esos datos son válidos o no. Si el modelo no es válido, se devuelve un error 400 (Bad Request) junto con los detalles de la validación para que el cliente pueda corregir los datos enviados.
        var id = await _servicio.CrearEmpresaAsync(usuarioId,dto);// id espera el resultado de la creación de la empresa, que generalmente sería un identificador único (GUID) generado para la nueva empresa creada en la base de datos. Este ID se utiliza para identificar de manera única a la empresa recién creada y puede ser útil para futuras operaciones relacionadas con esa empresa, como actualizaciones o eliminaciones.
        Console.WriteLine($"Empresa creada con ID: {id}");
        return Ok(new { id });
    }


    [HttpGet("{usuarioId}")]
    public async Task<IActionResult> ObtenerEmpresaPorUsuario(Guid usuarioId)
    {
        var result = await _servicio.ObtenerEmpresaPorUsuarioAsync(usuarioId);
        return Ok(result);
    }
    [HttpGet("empresa/{empresaId}")]
    public async Task<IActionResult> ObtenerEmpresaPorId(Guid empresaId)
    {
        var result = await _servicio.ObtenerEmpresaPorIdAsync(empresaId);
        if (result == null) return NotFound();
        return Ok(result);
    }


    [HttpPut]
    public async Task<IActionResult> ActualizarEmpresa([FromQuery] Guid usuarioId, [FromBody] EmpresaDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _servicio.ActualizarEmpresaAsync(usuarioId, dto);
        return Ok(result);
    }

    [HttpDelete("{empresaId:guid}")]//ruta {} 
    public async Task<IActionResult> EliminarEmpresa([FromQuery] Guid usuarioId, [FromRoute] Guid empresaId)
    {
        var result = await _servicio.EliminarEmpresaAsync(usuarioId, empresaId);
        return Ok(result);
    }





    [HttpGet("db/diagnostic")]
    //[Authorize(Roles = "Admin")] // 👈 MUY IMPORTANTE
    public async Task<IActionResult> GetDatabaseDiagnostic()
    {
        using var connection = _db.CrearConexion();

        var sql = @"
        SELECT
            a.pid,
            a.usename,
            a.datname,
            a.state,
            a.wait_event_type,
            a.wait_event,
            now() - a.query_start AS duration,
            a.query,

            -- 🔴 BLOQUEOS
            COALESCE(b.blocking_pids, '{}') AS blocking_pids

        FROM pg_stat_activity a

        LEFT JOIN LATERAL (
            SELECT array_agg(bl.pid) AS blocking_pids
            FROM pg_locks bl
            JOIN pg_stat_activity ba ON ba.pid = bl.pid
            WHERE bl.locktype IN ('relation','tuple','transactionid')
              AND bl.granted = true
              AND EXISTS (
                  SELECT 1
                  FROM pg_locks wl
                  WHERE wl.pid = a.pid
                    AND wl.granted = false
              )
        ) b ON true

        WHERE a.state <> 'idle'
        ORDER BY duration DESC;
    ";

        var result = await connection.QueryAsync(sql);

        return Ok(result);
    }

}
