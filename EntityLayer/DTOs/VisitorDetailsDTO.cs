using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs
{
    public class VisitorDetailsDTO
    {
        public int VisitorId { get; set; }
        public string VisitorName { get; set; }
        public string? VisitorSurName { get; set; }
        public int VisitorState { get; set; }
        public int VisitorLeftDays { get; set; }
        public string? VisitorPhoneNumber { get; set; }
        public double? VisitorTotalPrice { get; set; }
        public double? VisitorRoomPrice { get; set; }
        public DateTime VisitorStartDate { get; set; }
        public DateTime VisitorEndDate { get; set; }
        public int VisitorRoomNumber { get; set; }
        public int VisitorTotalVisitDay { get; set; }
        public Currency? VisitorPaymentCurrency { get; set; }
        public Rezervation? VisitorRezervation { get; set; }
        public string VisitorDescription { get; set; }
        public int? VisitorCount { get; set; }
        public bool VisitorPaymentIsDone { get; set; } = false;
        public List<VisitorDetailsPaymentDTO> Payments { get; set; } = new();
    }
    public class VisitorDetailsPaymentDTO
    {
        public DateTime? VisitorPaymentDate { get; set; }
        public PaymentType? VisitorPaymentType { get; set; }
        public Currency? VisitorPaymentCurreny { get; set; }
        public double? VisitorPayment { get; set; }
    }
}
