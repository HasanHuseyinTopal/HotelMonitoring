using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class VisitorHistory
    {
        [Key]
        public int VisitorHistoryId { get; set; }
        public int VisitorId { get; set; }
        public Visitor Visitor { get; set; }
        public DateTime? VisitorUpdatedDate { get; set; }
        public bool? VisitorNamesIsChanged { get; set; }
        public int? VisitorNewRoomNumber { get; set; }
        public int? VisitorOldRoomNumber { get; set; }
        public DateTime? VisitorNewStartDate { get; set; }
        public DateTime? VisitorOldStartDate { get; set; }
        public DateTime? VisitorNewEndDate { get; set; }
        public DateTime? VisitorOldEndDate { get; set; }
        public double? VisitorNewRoomPrice { get; set; }
        public double? VisitorOldRoomPrice { get; set; }
        public Currency? VisitorNewCurrency { get; set; }
        public Currency? VisitorOldCurrency { get; set; }
        public string? VisitorCheckInTime { get; set; }
        public string? VisitorCheckOutTime { get; set; }
    }
}
