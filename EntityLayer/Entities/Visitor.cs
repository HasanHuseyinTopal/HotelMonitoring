using EntityLayer.DTOs;
using System.ComponentModel.DataAnnotations;

namespace EntityLayer.Entities
{
    public class Visitor
    {
        public Visitor()
        {
            VisitorProperties = new();
        }
        [Key]
        public int VisitorId { get; set; }
        public List<VisitorProperty> VisitorProperties { get; set; }
        public double? VisitorRoomPrice { get; set; }
        public double? VisitorTotalRoomPrice { get; set; }
        public DateTime VisitorStartDate { get; set; }
        public string? VisitorPhoneNumber { get; set; }
        public int VisitorState { get; set; } = 0;
        public DateTime VisitorEndDate { get; set; }
        public int VisitorRoomNumber { get; set; }
        public Currency? VisitorPaymentCurrency { get; set; }
        public string? VisitorDescription { get; set; }
        public List<Payment> Payments { get; set; }
        public int? VisitorCount { get; set; }
        public string? VisitorRezervation { get; set; }
        public bool VisitorPaymentIsDone { get; set; }
        public DateTime? VisitorAddedDate { get; set; }
        public bool VisitorDontChangeRoom { get; set; } = false;
        public string? VisitorFileUrl { get; set; }
        public int? VisitorPreviusId { get; set; }
        public int? VisitorNextId { get; set; }
        public List<VisitorHistory> VisitorHistories { get; set; }
    }
}
