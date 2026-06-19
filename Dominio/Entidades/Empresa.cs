using System.ComponentModel.DataAnnotations;

namespace inventarioWebAI.Dominio.Entidades
{
    public class Empresa
    {
        public Guid Id { get; set; }
        /// Nombre de la empresa.
        [Required]
        public string Nombre { get; set; } = string.Empty;
        /// Fecha de creación del registro.
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
