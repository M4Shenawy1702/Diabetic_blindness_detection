using CheckEyePro.Core.Dtos.Auth;
using CheckEyePro.Core.Models;

namespace CheckEyePro.Core.Interfaces
{
    public interface IAuthInterface
    {
        Task<AuthModel> RegisterAsync(RegistrationDto model, string Role);
        Task<ApplicationUser> UpdateInfo(string UserId, UpdateInfoDto dto);
        Task<ApplicationUser> ChangePassword(string id, string TheNewPass);
        Task<AuthModel> GetTokenAsync(TokenRequestModel model);
        Task<AuthModel> Delete(string UserId);

    }
}