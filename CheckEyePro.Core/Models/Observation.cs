using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace CheckEyePro.Core.Models
{
    [PrimaryKey("ObservationId")]
    public class Observation
    {
        public int ObservationId { get; set; }
        public string DId { get; set; }
        public Doctor Doctor { get; set; }
        public string PId { get; set; }
        public Patient Patient { get; set; }
        public string? Report { get; set; }
        [MaxLength(250)]
        public string? Feedback { get; set; }
        public string? Diagnosis { get; set; }
        public bool Status { get; set; }
        public byte[]? MedicalRedation { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime FinishedOn { get; set; }
        public Payment Payment { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string? MedicalRecord { get; set; }
        public string? DoctorMessage { get; set; }
        public Gender? Gender { get; set; }
        public int Age { get; set; }
    }
    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed
    }
    public enum Gender
    {
        male,
        female
    }
}
