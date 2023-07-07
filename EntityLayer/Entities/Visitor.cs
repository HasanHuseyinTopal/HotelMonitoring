using EntityLayer.DTOs;
using System.ComponentModel.DataAnnotations;

namespace EntityLayer.Entities
{
    public class Visitor
    {
        [Key]
        public int VisitorId { get; set; }
        public string VisitorName { get; set; }
        public string? VisitorSurName { get; set; }
        public double? VisitorRoomPrice { get; set; }
        public DateTime VisitorStartDate { get; set; }
        public string? VisitorPhoneNumber { get; set; }
        public int VisitorState { get; set; } = 0;
        public DateTime VisitorEndDate { get; set; }
        public int VisitorRoomNumber { get; set; }
        public Currency? VisitorPaymentCurrency { get; set; }
        public string? VisitorDescription { get; set; }
        public List<Payment> Payments { get; set; }
        public int? VisitorCount { get; set; }
        public Rezervation? VisitorRezervation { get; set; }
        public bool VisitorPaymentIsDone { get; set; }

    }
}
