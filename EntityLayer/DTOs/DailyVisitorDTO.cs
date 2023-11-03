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
            listOfVisitors = new List<DailyVisitorDTO>();
            roomNumbers =   new int []{ 101, 102, 103, 104, 201, 202, 203, 204, 301, 302, 303, 304, 401, 402, 403, 404, 501, 502, 503, 601, 602, 603, 701, 702 };
            roomStates = new Dictionary<int, string>();
            paymentStates = new Dictionary<int, bool>();
            VisitorTransferNext = new Dictionary<int, int?>();
            VisitorTransferPrevius = new Dictionary<int, int?>();
            foreach (var roomNumber in roomNumbers)
            {
                paymentStates.Add(roomNumber, true);
                roomStates.Add(roomNumber, "");
            }
        }
        public int [] roomNumbers { get; set; }
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
        public string VisitorState { get; set; }
        public bool IsPaymentDone { get; set; }
        public int resultOfP { get; set; } = 0;
        public int? VisitorNextRoomId { get; set; }
        public int? VisitorPreviusRoomId { get; set; }

    }
}
