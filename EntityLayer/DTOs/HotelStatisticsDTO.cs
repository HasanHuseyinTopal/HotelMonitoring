using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs
{
    public class HotelStatisticsDTO
    {
        public HotelStatisticsDTO()
        {
            RezervationCounts = new();
        }
        public Dictionary<string, int> RezervationCounts { get; set; }
    }
}
