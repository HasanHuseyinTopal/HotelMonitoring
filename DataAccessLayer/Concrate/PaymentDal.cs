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
        CpContext _cpContext;
        public PaymentDal(CpContext context) : base(context)
        {
            _cpContext = context;
        }

        public IQueryable<Payment> GetAllWithVisitorProperties(Expression<Func<Payment, bool>> filter = null)
        {
            var query = _cpContext.Payments.Include(x => x.Visitor.VisitorProperties).AsQueryable();
            if (filter != null)
                query = query.Where(filter);
            return query;
        }
    }
}
