using inventarioWebAI.Aplicacion.Interfaces.IAuth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
[Obsolete("Se elimina. Autenticación ahora es Supabase JWT")]

public class JwtServicio : IJwtServicio
{
    private readonly IConfiguration _config;

    public JwtServicio(IConfiguration config)
    {
        _config = config;
    }
    [Obsolete("Se elimina. Autenticación ahora es Supabase JWT")]

    public string generarToken(Guid usuarioId, Guid empresaId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
            new Claim("empresaId", empresaId.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"])
        );

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    
}