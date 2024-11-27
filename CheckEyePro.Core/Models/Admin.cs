using System.ComponentModel.DataAnnotations;

namespace CheckEyePro.Core.Models
{
    public class Admin
    {
        public int AdminId { get; set; }
        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        [StringLength(50)]
        public string Username { get; set; }
        public byte[]? ProfileImg { get; set; }
    }
}
