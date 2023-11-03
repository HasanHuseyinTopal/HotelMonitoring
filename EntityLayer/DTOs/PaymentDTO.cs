using EntityLayer.Entities;
using System.ComponentModel.DataAnnotations;

namespace EntityLayer.DTOs
{
    public class PaymentDTO
    {
        public int PaymentId { get; set; }
        [Required(ErrorMessage ="Ödeme tarihi seçilmelidir")]
        public DateTime? VisitorPaymentDate { get; set; }
        [Required(ErrorMessage = "Ödeme türü seçilmelidir")]
        public PaymentType? VisitorPaymentType { get; set; }
        [Required(ErrorMessage = "Para türü seçilmelidir")]
        public Currency? VisitorPaymentCurreny { get; set; }
        [Required(ErrorMessage = "Ödenen tutar girilmelidir")]
        public double? VisitorPayment { get; set; }
        public int VisitorId { get; set; }
        public string? VisitorName { get; set; }
        public double? VisitorTotalPrice { get; set; }
        public Currency? VisitorTotalPriceCurrency { get; set; }
    }
}
