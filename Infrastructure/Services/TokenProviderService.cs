using Application.Common.Interfaces;
using Domain.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services;

public class TokenProviderService : ITokenProvider
{
    private SecurityKey _key;
    private string _algorithm;
    private string _issuer;
    private string _audience;
    public TokenProviderService(string issuer, string audience, string keyName)
    {
        _key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(keyName));
        _algorithm = SecurityAlgorithms.HmacSha256Signature;
        _issuer = issuer;
        _audience = audience;
    }

    public string CreateToken(JwtUserContract user, DateTime expiry)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        //ClaimsIdentity identity = new ClaimsIdentity(new GenericIdentity(user.UserName, "jwt"));

        // TODO: Add whatever claims the user may have...

        SecurityToken token = tokenHandler.CreateJwtSecurityToken(new SecurityTokenDescriptor
        {
            Audience = _audience,
            Issuer = _issuer,
            SigningCredentials = new SigningCredentials(_key, _algorithm),
            Expires = expiry.ToUniversalTime(),
            Subject = new ClaimsIdentity(new[]
            {
                    new Claim("Id", user.id),
                    new Claim("Email", user.email??"N/A"),
                    new Claim("LoginRole",user.loginRole.ToString()),
                    new Claim(ClaimTypes.NameIdentifier,user.id),
            })
        });

        return tokenHandler.WriteToken(token);
    }

    public TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters
        {
            IssuerSigningKey = _key,
            ValidAudience = _audience,
            ValidIssuer = _issuer,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(0) // Identity and resource servers are the same.
        };
    }
}
