using System.ComponentModel.DataAnnotations;

namespace CheckEyePro.Core.Dtos
{
    public class GetAllDoctorsDto
    {
        public int DoctorId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string? CareerInfo { get; set; }
        public string UserId { get; set; }
        public byte[]? ProfileImg { get; set; }
    }
}
