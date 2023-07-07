using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs
{
    public class IsRoomCleanDTO
    {
        public int RoomNumber { get; set; }
        public bool RoomIsClean { get; set; }
    }
}
