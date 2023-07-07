using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }
        public int RoomNumber { get; set; }
        public bool RoomHasBalcony { get; set; }
        public string RoomType { get; set; }
        public bool RoomIsClean { get; set; }
        public int RoomSize { get; set; }
        public int RoomFloor { get; set; }
    }
}
