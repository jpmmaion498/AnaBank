using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AnaBank.BuildingBlocks.Web.Authentication;

public interface IJwtTokenService
{
    string GenerateToken(string accountId, string accountNumber);
    ClaimsPrincipal? ValidateToken(string token);
}

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(JwtSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public string GenerateToken(string accountId, string accountNumber)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_settings.SecretKey);
        
        var claims = new []
        {
            new Claim("sub", accountId),
            new Claim("numero", accountNumber),
            new Claim(ClaimTypes.NameIdentifier, accountId), // Fallback
            new Claim("accountId", accountId) // Outro fallback
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(_settings.ExpirationHours),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _settings.Issuer,
            Audience = _settings.Audience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        
        // Debug: Log do token gerado
        System.Diagnostics.Debug.WriteLine($"Token gerado para accountId: {accountId}");
        System.Diagnostics.Debug.WriteLine($"Claims no token: {string.Join(", ", claims.Select(c => $"{c.Type}={c.Value}"))}");
        
        return tokenString;
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_settings.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _settings.Issuer,
                ValidateAudience = true,
                ValidAudience = _settings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            
            // Debug: Log dos claims validados
            System.Diagnostics.Debug.WriteLine($"Token validado com sucesso. Claims: {string.Join(", ", principal.Claims.Select(c => $"{c.Type}={c.Value}"))}");
            
            return principal;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro validando token: {ex.Message}");
            return null;
        }
    }
}

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationHours { get; set; } = 24;
}