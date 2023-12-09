using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs
{
    public class VisitorSearchDTOs
    {
        public VisitorSearchDTOs()
        {
            visitorDetails = new();
        }
        public List<VisitorSearchDTO> visitorDetails;
        public int TotalPageCount { get; set; }
    }
    public class VisitorSearchDTO
    {
        public int VisitorId { get; set; }
        public string VisitorName { get; set; }
        public string? VisitorSurName { get; set; }
        public string VisitorStartDate { get; set; }
        public int VisitorCount { get; set; }
        public string VisitorEndDate { get; set; }
        public int VisitorRoomNumber { get; set; }
        public string? VisitorRezervation { get; set; }
        public string? VisitorPaymentCurrency { get; set; }
        public string? VisitorRoomPrice { get; set; }
        public int TotalPageCount { get; set; }

    }
}
