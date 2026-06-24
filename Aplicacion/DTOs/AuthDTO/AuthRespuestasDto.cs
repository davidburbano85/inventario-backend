namespace inventarioWebAI.Aplicacion.DTOs.AuthDTO
{
    public class AuthRespuestasDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string TokenInterno { get; set; }
        public Guid UserId { get; set; }
    }
}
