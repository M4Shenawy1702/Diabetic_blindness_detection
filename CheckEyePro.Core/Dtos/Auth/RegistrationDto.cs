using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CheckEyePro.Core.Dtos.Auth
{
    public class RegistrationDto
    {


        [Required, MaxLength(250)]
        public string FirstName { get; set; }
        [StringLength(100)]
        public string LastName { get; set; }
        [StringLength(50)]
        public string Username { get; set; }

        [StringLength(128)]
        public string Email { get; set; }

        [StringLength(256)]
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public IFormFile ProfileImg { get; set; }
        public string? CareerInfo { get; set; }

    }
}
