using CheckEyePro.Core.Models;
using System.IdentityModel.Tokens.Jwt;

namespace CheckEyePro.Core.IServices
{
    public interface IJWTTokenGenerator
    {
        Task<JwtSecurityToken> CreateJwtTokenAsync(ApplicationUser user);
        //RefreshToken GenerateRefreshToken();
    }
}
