using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CheckEyePro.Core.Models
{
    [PrimaryKey("PatientId")]
    public class Patient
    {
        public int PatientId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public List<Observation> Observations { get; set; }

        [StringLength(50)]
        public string Username { get; set; }
        public byte[]? ProfileImg { get; set; }
    }
}
