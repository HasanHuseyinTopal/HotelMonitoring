using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public DateTime? VisitorPaymentDate { get; set; }
        public PaymentType? VisitorPaymentType { get; set; }
        public Currency? VisitorPaymentCurreny { get; set; }
        public double? VisitorPayment { get; set; }
        public int VisitorId { get; set; }
        public Visitor Visitor { get; set; }
    }
    public enum Currency
    {
        EURO,
        USD,
        TL
    }
    public enum PaymentType
    {
        Nakit,
        KrediKartı
    }
}
