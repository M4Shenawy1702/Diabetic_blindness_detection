using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckEyePro.Core.Dtos.Auth
{
    public class UpdateInfoDto
    {
        [Required, MaxLength(250)]
        public string FirstName { get; set; }
        [StringLength(100)]
        public string LastName { get; set; }
        [StringLength(50)]
        public string Username { get; set; }
        [StringLength(128)]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public IFormFile ProfileImg { get; set; }
        public string? CareerInfo { get; set; }
    }
}
