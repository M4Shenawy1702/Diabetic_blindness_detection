using CheckEyePro.Core.Models;
namespace CheckEyePro.Core.Dtos
{
    public class ShowObservations
    {
        public int ObservationId { get; set; }
        public string? DoctorId { get; set; }
        public string? PatientId { get; set; }
        public bool Status { get; set; } = false;
        public byte[]? MedicalRedation { get; set; }
        public string? Diagnosis { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string? DoctorName { get; set; }
        public string? PatientName { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string? Report { get; set; }
        public string? Feedback { get; set; }
        public string? DoctorMessage { get; set; }
        public string? MedicalRecord { get; set; }
    }
}
