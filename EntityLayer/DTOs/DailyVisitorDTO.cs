using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs
{
    public class DailyVisitorDTOs
    {
        public DailyVisitorDTOs()
        {
            listOfVisitors = new();
            roomStates = new();
            paymentStates = new();
            VisitorTransferNext = new();
            VisitorTransferPrevius = new();
        }
        public int[] roomNumbers { get; set; }
        public List<DailyVisitorDTO> listOfVisitors { get; set; }
        public Dictionary<int, string> roomStates { get; set; }
        public Dictionary<int, bool> paymentStates { get; set; }
        public Dictionary<int, int?> VisitorTransferNext { get; set; }
        public Dictionary<int, int?> VisitorTransferPrevius { get; set; }
        public int CheckInCount { get; set; }
        public int CheckOutCount { get; set; }
        public int StayInCount { get; set; }
        public int CheckInRoomCount { get; set; }
        public int CheckOutRoomCount { get; set; }
        public int StayInRoomCount { get; set; }

    }
    public class DailyVisitorDTO
    {
        public DateTime VisitorStartDate { get; set; }
        public DateTime VisitorEndDate { get; set; }
        public int VisitorRoomNumber { get; set; }
        public string? VisitorState { get; set; }
        public bool IsPaymentDone { get; set; }
        public int resultOfP { get; set; } = 0;
        public int? VisitorNextRoomId { get; set; }
        public int? VisitorPreviusRoomId { get; set; }

    }
}
