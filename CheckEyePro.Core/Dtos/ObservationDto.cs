using CheckEyePro.Core.Models;
using Microsoft.AspNetCore.Http;

namespace CheckEyePro.Core.Dtos
{
    public class ObservationDto
    {

        public string DId { get; set; }
        public Gender? Gender { get; set; }
        public int Age { get; set; }
        public string MedicalRecord { get; set; }
        public IFormFile MedicalRedation { get; set; }
        public string? DoctorMessage { get; set; }

    }
}
