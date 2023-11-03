using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs
{
    public class AvailabilityDTO
    {
        public int monthNumber { get; set; }
        public (int RoomCount, string SameRoomControl)[,] availabilty { get; set; }
        public int[] roomNumbers { get; set; } = { 101, 102, 103, 104, 201, 202, 203, 204, 301, 302, 303, 304, 401, 402, 403, 404, 501, 502, 503, 601, 602, 603, 701, 702 };
        public int[] SingleRooms { get; set; } = { 102, 202, 302, 402, 104, 204, 304, 404 };
        public int[] DoubleRooms { get; set; } = { 101, 201, 301, 401, 103, 203, 303, 403, 502 };
        public int[] DoubleRoomsWithBalcony { get; set; } = { 601, 602, 603, 701 };
        public int[] TripleRooms { get; set; } = { 501, 503, 702 };
    }
}
