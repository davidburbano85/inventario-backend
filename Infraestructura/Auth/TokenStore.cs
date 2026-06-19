namespace inventarioWebAI.Infraestructura.Auth;
public class TokenStore
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}