using Dapper;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Dominio.Entidades;
using System.Data;

namespace inventarioWebAI.Infraestructura.RepositoriosDapper;

public class LogSistemaRepositorioDapper : ILogSistemaRepositorio
{
    public async Task<Guid> RegistrarAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        LogSistema log)
    {
        var sql = @"
            INSERT INTO logs_sistema
            (
                empresa_id,
                usuario_id,
                accion,
                detalle,
                created_at,
                updated_at,
                activo
            )
            VALUES
            (
                @EmpresaId,
                @UsuarioId,
                @Accion,
                @Detalle,
                timezone('America/Bogota', now()),
                timezone('America/Bogota', now()),
                true
            )
            RETURNING id;";

        // Mantiene la auditoría dentro de la misma transacción
        // de la operación que generó el evento.
        return await connection.ExecuteScalarAsync<Guid>(
            sql,
            new
            {
                log.EmpresaId,
                log.UsuarioId,
                log.Accion,
                log.Detalle
            },
            transaction
        );
    }


    public async Task<IEnumerable<LogSistema>> ObtenerPorEmpresaAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid empresaId)
    {
        var sql = @"
            SELECT
                id,
                empresa_id AS EmpresaId,
                usuario_id AS UsuarioId,
                accion AS Accion,
                detalle AS Detalle,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt,
                activo AS Activo
            FROM logs_sistema
            WHERE empresa_id = @EmpresaId
              AND activo = true
            ORDER BY created_at DESC;";

        return await connection.QueryAsync<LogSistema>(
            sql,
            new
            {
                EmpresaId = empresaId
            },
            transaction
        );
    }


    public async Task<LogSistema?> ObtenerPorIdAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Guid id,
        Guid empresaId)
    {
        var sql = @"
            SELECT
                id,
                empresa_id AS EmpresaId,
                usuario_id AS UsuarioId,
                accion AS Accion,
                detalle AS Detalle,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt,
                activo AS Activo
            FROM logs_sistema
            WHERE id = @Id
              AND empresa_id = @EmpresaId
              AND activo = true;";

        return await connection.QueryFirstOrDefaultAsync<LogSistema>(
            sql,
            new
            {
                Id = id,
                EmpresaId = empresaId
            },
            transaction
        );
    }
}