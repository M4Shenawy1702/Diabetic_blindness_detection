using System.ComponentModel.DataAnnotations;
namespace CheckEyePro.Core.Dtos.PatientDtos
{
    public class PatientProfileDto
    {
        [Required, MaxLength(250)]
        public string? FirstName { get; set; }
        [StringLength(100)]
        public string? LastName { get; set; }
        [StringLength(50)]
        public string? Username { get; set; }
        public string? FullName { get; set; }


        [StringLength(128)]
        public string? Email { get; set; }

        [StringLength(256)]
        public string? PhoneNumber { get; set; }
        public byte[]? ProfileImg { get; set; }
    }
}
