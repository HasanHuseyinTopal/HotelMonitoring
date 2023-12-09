using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Queries
{
    public class VisitorSearchOptions
    {
        public string? VisitorName { get; set; } = "";
        public string? VisitorSurName { get; set; } = "";
        public DateTime? VisitorStartDate { get; set; }
        public DateTime? VisitorEndDate { get; set; }
        public int? VisitorRoomNumber { get; set; }
        public int PageNumber { get; set; }
    }
}
