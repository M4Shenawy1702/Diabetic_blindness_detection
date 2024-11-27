using System.ComponentModel.DataAnnotations;

namespace CheckEyePro.Core.Dtos.PatientDtos
{
    public class FeedbackDto
    {
        [Required, MaxLength(250)]
        public string Feedback { get; set; }

    }
}
