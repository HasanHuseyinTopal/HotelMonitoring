using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs
{
    public class DailyResultDTO
    {
        public List<FinancialManagement> FinancialManagements { get; set; }
        public double? TotalCashTL { get; set; } = 0;
        public double? TotalCashEURO { get; set; } = 0;
        public double? TotalCashUSD { get; set; } = 0;
        public double? TotalKreditTL { get; set; } = 0;
        public double? TotalKreditEURO { get; set; } = 0;
        public double? TotalKreditUSD { get; set; } = 0;
        public int DayNumber { get; set; }
        public Guid DailyEncrytpon { get; set; }
    }
}
