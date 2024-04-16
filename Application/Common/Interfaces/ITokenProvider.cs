using Domain.Contracts;
using Microsoft.IdentityModel.Tokens;

namespace Application.Common.Interfaces;

public interface ITokenProvider 
{
    string CreateToken(JwtUserContract user, DateTime expiry);

    // TokenValidationParameters is from Microsoft.IdentityModel.Tokens
    TokenValidationParameters GetValidationParameters();
}
