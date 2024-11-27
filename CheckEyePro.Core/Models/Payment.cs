using System.ComponentModel.DataAnnotations.Schema;

namespace CheckEyePro.Core.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public float Amount { get; set; }
        public DateTime Date { get; set; }
        public int ObservationId { get; set; }
        [ForeignKey("ObservationId")]
        public Observation Observation { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

    }

    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        BankTransfer
    }
}
