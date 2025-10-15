using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.Dominio.Entidades;

namespace MinimalApi.Infraestutura.Token;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GerarToken(Administrador admin)
    {
        var jwtConfig = _configuration.GetSection("Jwt");
        var key = jwtConfig["Key"] ?? throw new InvalidOperationException("Jwt:Key não configurado");
        var issuer = jwtConfig["Issuer"];
        var audience = jwtConfig["Audience"];
        var expireMinutes = int.Parse(jwtConfig["ExpireMinutes"] ?? "60");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, admin.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, admin.Email),
            new Claim("nome", admin.Nome),
            new Claim(ClaimTypes.Role, admin.Perfil ?? string.Empty)
        };

        var keyBytes = Encoding.UTF8.GetBytes(key);
        var securityKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}