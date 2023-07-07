using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs
{
    public class VisitorSearchDTO
    {
        public int VisitorId { get; set; }
        public string VisitorName { get; set; }
        public string? VisitorSurName { get; set; }
        public string VisitorStartDate { get; set; }
        public string VisitorEndDate { get; set; }
        public int VisitorRoomNumber { get; set; }
        public string? VisitorPaymentCurrency { get; set; }
        public string? VisitorRoomPrice { get; set; }
        public string? VisitorDescription { get; set; }

    }
}
