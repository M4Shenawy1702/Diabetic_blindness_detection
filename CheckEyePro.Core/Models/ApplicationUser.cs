using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CheckEyePro.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string LastName { get; set; }
        public Doctor Doctor { get; set; }
        public Patient patient { get; set; }
        public Admin Admin { get; set; }
    }
}