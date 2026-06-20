using inventarioWebAI.Aplicacion.DTOs.Usuario;
using inventarioWebAI.Aplicacion.Interfaces.IPermmisoServicios;
using inventarioWebAI.Aplicacion.Interfaces.Irepositorios;
using inventarioWebAI.Aplicacion.Interfaces.Iservicios;
using inventarioWebAI.Dominio.Entidades;

namespace inventarioWebAI.Aplicacion.Servicios.ServiciosDto
{
    public class UsuarioServicio : IUsuarioServicio
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly ILogger<UsuarioServicio> _logger;
        private readonly IPermisoServicio _permisoServicio;
        public UsuarioServicio(IUsuarioRepositorio usuarioRepositorio, 
                                ILogger<UsuarioServicio> logger,
                                IPermisoServicio permisoServicio)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _logger = logger;
            _permisoServicio = permisoServicio;
        }

        // ================================
        // CREAR USUARIO
        // ================================

        public async Task<UsuarioDTO> CrearAsync(UsuarioDTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Nombre))
                    throw new Exception("El nombre del usuario es obligatorio.");
                if (string.IsNullOrEmpty(dto.Telefono))
                    throw new Exception("El teléfono del usuario es obligatorio.");
                var telefonoExiste = await _usuarioRepositorio.ExisteTelefonoAsync(dto.Telefono);
                if (telefonoExiste)
                    throw new Exception("El teléfono ya está registrado para otro usuario.");

                var nuevoUsuario = new Usuario
                {
                    Nombre = dto.Nombre,
                    Telefono = dto.Telefono,
                    CreatedAt = DateTime.UtcNow,
                    Id = Guid.Empty // Este valor debería ser asignado correctamente según la lógica de autenticación de tu aplicación

                };
                var usuarioCreado = await _usuarioRepositorio.CrearAsync(nuevoUsuario);
                return new UsuarioDTO();

            }
            catch (Exception ex)
            {
                // Manejar excepciones y errores
                throw new Exception($"Error al crear el usuario: {ex.Message}");
            }
        }


        public async Task<IEnumerable<UsuarioDTO>> ListarAsync()
        {
            try
            {
                var usuarios = await _usuarioRepositorio.ListarAsync();
                return usuarios.Select(u => new UsuarioDTO
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Telefono = u.Telefono,
                    CreatedAt = u.CreatedAt
                }).ToList();

            }
            catch (Exception ex)
            {
                // Manejar excepciones y errores
                throw new Exception($"Error al listar los usuarios: {ex.Message}");
            }
        }

        public async Task<UsuarioDTO> ObtenerPorAuthUserIdAsync(Guid Id)
        {
            var usuario = await _usuarioRepositorio.ObtenerPorAuthUserIdAsync(Id);
            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Telefono = usuario.Telefono,
                CreatedAt = usuario.CreatedAt
            };
        }

        public async Task<UsuarioDTO?> ObtenerPorIdAsync(Guid idUsuario)
        {
            _logger.LogInformation(
                "Iniciando búsqueda de usuario {IdUsuario}",
                idUsuario);

            if (idUsuario == Guid.Empty)
            {
                _logger.LogWarning("Se recibió un Guid vacío");
                return null;
            }

            var usuario = await _usuarioRepositorio.ObtenerPorIdAsync(idUsuario);

            if (usuario == null)
            {
                _logger.LogWarning(
                    "Usuario no encontrado {IdUsuario}",
                    idUsuario);

                return null;
            }

            _logger.LogInformation(
                "Usuario encontrado {IdUsuario}",
                idUsuario);

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Telefono = usuario.Telefono,
                CreatedAt = usuario.CreatedAt
            };
        }
        public async Task<UsuarioDTO> ActualizarAsync(UsuarioDTO dto)
        {
            try
            {
                var usuario = await ObtenerPorTelefonoAsync(dto.Telefono);

                if (usuario == null)
                    throw new Exception("El usuario no existe.");

                // validar teléfono nuevo
                if (!string.IsNullOrWhiteSpace(dto.TelefonoNuevo) &&
                    !dto.TelefonoNuevo.Equals(usuario.Telefono, StringComparison.OrdinalIgnoreCase))
                {
                    var existe = await ObtenerPorTelefonoAsync(dto.TelefonoNuevo);
                    if (existe != null)
                        throw new Exception("El nuevo teléfono ya está registrado.");

                    usuario.Telefono = dto.TelefonoNuevo;
                }

                //  ACTUALIZAR NOMBRE
                if (!string.IsNullOrWhiteSpace(dto.Nombre))
                {
                    usuario.Nombre = dto.Nombre;
                }

                var actualizado = await _usuarioRepositorio.ActualizarAsync(new Usuario
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Telefono = usuario.Telefono,
                    CreatedAt = usuario.CreatedAt
                });

                if (!actualizado)
                    throw new Exception("No se pudo actualizar el usuario.");

                return new UsuarioDTO
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Telefono = usuario.Telefono,
                    CreatedAt = usuario.CreatedAt
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar el usuario: {ex.Message}");
            }
        }


        public async Task<UsuarioDTO> EliminarAsync(Guid idUsuario)
        {
            try
            {
                if (idUsuario == Guid.Empty)//
                    throw new Exception("El ID del usuario es inválido.");
                var existe = await _usuarioRepositorio.ObtenerPorIdAsync(idUsuario);
                if (existe == null)
                    throw new Exception("El usuario no existe.");
                var eliminado = await _usuarioRepositorio.EliminarAsync(idUsuario);
                if (!eliminado)
                    throw new Exception("No se pudo eliminar el usuario.");
                return new UsuarioDTO
                {
                    Id = existe.Id,
                    Nombre = existe.Nombre,
                    Telefono = existe.Telefono,
                    CreatedAt = existe.CreatedAt
                };
            }
            catch (Exception ex)
            {
                // Manejar excepciones y errores
                throw new Exception($"Error al eliminar el usuario: {ex.Message}");
            }
        }


        public async Task<UsuarioDTO?> ObtenerPorTelefonoAsync(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
                return null;
            var usuario = await _usuarioRepositorio.ObtenerPorTelefonoAsync(telefono);
            Console.WriteLine("Usuario encontrado: " + (usuario != null ? usuario.Nombre : "null"));
            if (usuario == null)
                return null;
            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Telefono = usuario.Telefono,
                CreatedAt = usuario.CreatedAt
            };
        }

        public async Task<bool> ActualizarPorAuthAsync(Guid authId, UsuarioDTO dto)
        {
            try
            {
                _logger.LogInformation(
                    "INICIO Servicio ActualizarPorAuthAsync. AuthId: {AuthId}",
                    authId);

                if (authId == Guid.Empty)
                {
                    _logger.LogWarning("AuthId recibido vacío.");
                    return false;
                }

                if (dto == null)
                {
                    _logger.LogWarning("DTO recibido null.");
                    return false;
                }

                _logger.LogInformation(
                    "Datos recibidos. Nombre: {Nombre}. Telefono: {Telefono}",
                    dto.Nombre,
                    dto.Telefono);

                var usuarioExistente =
                    await _usuarioRepositorio.ObtenerPorAuthUserIdAsync(authId);

                if (usuarioExistente == null)
                {
                    _logger.LogWarning(
                        "No existe usuario con auth_user_id={AuthId}",
                        authId);

                    return false;
                }

                _logger.LogInformation(
                    "Usuario encontrado. Id BD: {Id}",
                    usuarioExistente.Id);

                if (!string.IsNullOrWhiteSpace(dto.Telefono) &&
                    dto.Telefono != usuarioExistente.Telefono)
                {
                    var telefonoExiste =
                        await _usuarioRepositorio.ExisteTelefonoAsync(dto.Telefono);

                    if (telefonoExiste)
                    {
                        _logger.LogWarning(
                            "Teléfono duplicado detectado: {Telefono}",
                            dto.Telefono);

                        throw new Exception("El teléfono ya está registrado.");
                    }
                }

                usuarioExistente.Nombre = dto.Nombre;
                usuarioExistente.Telefono = dto.Telefono;

                _logger.LogInformation(
                    "Enviando actualización al repositorio. AuthId: {AuthId}",
                    authId);

                var resultado =
                    await _usuarioRepositorio.ActualizarPorAuthAsync(
                        authId,
                        usuarioExistente);

                _logger.LogInformation(
                    "Resultado actualización: {Resultado}",
                    resultado);

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "ERROR Servicio ActualizarPorAuthAsync. AuthId: {AuthId}",
                    authId);

                throw;
            }
        }

    }
}
