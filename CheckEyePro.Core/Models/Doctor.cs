using System.ComponentModel.DataAnnotations;

namespace CheckEyePro.Core.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        [StringLength(50)]
        public string Username { get; set; }
        public string? CareerInfo { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public List<Observation> Observations { get; set; }

        public byte[]? ProfileImg { get; set; }
    }
}
