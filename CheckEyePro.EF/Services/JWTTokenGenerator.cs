using CheckEyePro.Core.IServices;
using CheckEyePro.Core.Models;
using CheckEyePro.Core.Settingse;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrderManagementSystem.EF.Services
{

    public class JWTTokenGenerator : IJWTTokenGenerator
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWT _jwtSettings;
        public JWTTokenGenerator(UserManager<ApplicationUser> userManager, IOptions<JWT> jwtSettings)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
        }
        public async Task<JwtSecurityToken> CreateJwtTokenAsync(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwtSettings.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        //public RefreshToken GenerateRefreshToken()
        //{
        //    var randomNumber = new byte[32];

        //    using var generator = new RNGCryptoServiceProvider();

        //    generator.GetBytes(randomNumber);

        //    return new RefreshToken
        //    {
        //        Token = Convert.ToBase64String(randomNumber),
        //        ExpiresOn = DateTime.UtcNow.AddDays(10),
        //        CreatedOn = DateTime.UtcNow
        //    };
        //}
    }
}
