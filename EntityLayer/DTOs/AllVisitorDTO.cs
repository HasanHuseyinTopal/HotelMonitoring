using EntityLayer.Entities;


namespace EntityLayer.DTOs
{
    public class AllVisitorDTO
    {
        public int VisitorId { get; set; }
        public string VisitorName { get; set; }
        public string? VisitorSurName { get; set; }
        public int VisitorLeftDays { get; set; }
        public int VisitorTotalVisitDay { get; set; }
        public int VisitorState { get; set; }
        public DateTime VisitorStartDate { get; set; }
        public DateTime VisitorEndDate { get; set; }
        public int VisitorRoomNumber { get; set; }
        public int? VisitorCount { get; set; }
        public double? VisitorRoomPrice { get; set; }
        public string? VisitorDescription { get; set; }
        public double? VisitorTotalPrice { get; set; }
        public Currency? VisitorPaymentCurrency { get; set; }
        public bool VisitorPaymentIsDone { get; set; }
    }
}

