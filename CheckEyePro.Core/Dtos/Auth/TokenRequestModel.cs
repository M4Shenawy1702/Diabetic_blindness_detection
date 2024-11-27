using System.ComponentModel.DataAnnotations;

namespace CheckEyePro.Core.Dtos.Auth
{
    public class TokenRequestModel
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}