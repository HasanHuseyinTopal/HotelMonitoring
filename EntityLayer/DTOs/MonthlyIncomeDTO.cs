using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs
{
    public class MonthlyIncomeDTO
    {
        public double? TurkishLiras { get; set; }
        public double? Dollar { get; set; }
        public double? Euro { get; set; }
        public int MonthNumber { get; set; }
    }
}
