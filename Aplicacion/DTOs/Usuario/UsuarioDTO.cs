namespace inventarioWebAI.Aplicacion.DTOs.Usuario
{
    public class UsuarioDTO
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string TelefonoNuevo { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
