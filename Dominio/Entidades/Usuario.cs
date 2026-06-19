namespace inventarioWebAI.Dominio.Entidades
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public  DateTime CreatedAt { get; set; }


    }
}
