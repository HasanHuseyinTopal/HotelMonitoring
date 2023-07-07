using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrate
{
    public class PaymentDal : GenericDal<Payment>, IPaymentDal
    {
        public PaymentDal(CpContext context) : base(context)
        {
        }
    }
}
