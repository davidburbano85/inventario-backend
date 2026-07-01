using inventarioWebAI.Aplicacion.Interfaces.Context;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Servicios;

public class LogSistemaServicio : ILogSistemaServicio
{
    private readonly ILogSistemaRepositorio _logRepositorio;
    private readonly IUsuarioContext _usuarioContext;
    private readonly IUsuarioEmpresaRepositorio _usuarioEmpresaRepositorio;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LogSistemaServicio> _logger;

    public LogSistemaServicio(
        ILogSistemaRepositorio logRepositorio,
        IUsuarioContext usuarioContext,
        IUsuarioEmpresaRepositorio usuarioEmpresaRepositorio,
        IUnitOfWork unitOfWork,
        ILogger<LogSistemaServicio> logger)
    {
        _logRepositorio = logRepositorio;
        _usuarioContext = usuarioContext;
        _usuarioEmpresaRepositorio = usuarioEmpresaRepositorio;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }


    public async Task RegistrarAsync(
        string accion,
        string detalle)
    {
        if (string.IsNullOrWhiteSpace(accion))
            throw new InvalidOperationException("La acción del log es obligatoria.");

        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var empresa = await _usuarioEmpresaRepositorio
            .ObtenerEmpresaActivaAsync(usuarioId);

        if (empresa == null || !empresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        var log = new LogSistema
        {
            EmpresaId = empresa.EmpresaId,
            UsuarioId = usuarioId,
            Accion = accion.Trim(),
            Detalle = detalle?.Trim()
        };

        try
        {
            await _logRepositorio.RegistrarAsync(
                _unitOfWork.Connection,
                _unitOfWork.Transaction,
                log);
        }
        catch (Exception ex)
        {
            // El fallo del log no debe ocultar el error original
            // de una operación crítica si se integra dentro de una transacción.
            _logger.LogError(
                ex,
                "Error registrando auditoría del sistema. Acción: {Accion}",
                accion);

            throw;
        }
    }


    public async Task<IEnumerable<LogSistema>> ObtenerPorEmpresaAsync()
    {
        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var empresa = await _usuarioEmpresaRepositorio
            .ObtenerEmpresaActivaAsync(usuarioId);

        if (empresa == null || !empresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        return await _logRepositorio.ObtenerPorEmpresaAsync(
            _unitOfWork.Connection,
            _unitOfWork.Transaction,
            empresa.EmpresaId);
    }


    public async Task<LogSistema?> ObtenerPorIdAsync(Guid logId)
    {
        if (logId == Guid.Empty)
            throw new InvalidOperationException("Id de log inválido.");

        var usuarioId = _usuarioContext.ObtenerAuthUserId();

        var empresa = await _usuarioEmpresaRepositorio
            .ObtenerEmpresaActivaAsync(usuarioId);

        if (empresa == null || !empresa.Activo)
            throw new InvalidOperationException("No hay empresa activa.");

        return await _logRepositorio.ObtenerPorIdAsync(
            _unitOfWork.Connection,
            _unitOfWork.Transaction,
            logId,
            empresa.EmpresaId);
    }
}