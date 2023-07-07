using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccessLayer.Concrate
{
    public class VisitorDal : GenericDal<Visitor>, IVisitorDal
    {
        CpContext _cpContext;
        public VisitorDal(CpContext context) : base(context)
        {
            _cpContext = context;
        }

        public IQueryable<Visitor> GetAllVisitorsWithPayment(Expression<Func<Visitor, bool>> filter = null)
        {
            return _cpContext.Visitors.Include(x => x.Payments).AsQueryable().Where(filter);
        }
    }
}
