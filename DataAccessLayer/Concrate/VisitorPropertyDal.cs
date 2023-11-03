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
    public class VisitorPropertyDal : GenericDal<VisitorProperty>, IVisitorPropertyDal
    {
        CpContext _cpContext;
        public VisitorPropertyDal(CpContext context) : base(context)
        {
            _cpContext = context;
        }

        public IQueryable<VisitorProperty> GetAllPropertiesWithVisitors(Expression<Func<VisitorProperty, bool>> filter = null)
        {
            var query = _cpContext.VisitorProperties.Include(x => x.Visitor).AsQueryable();
            if (filter != null)
                query = query.Where(filter);
            return query;
        }

        public VisitorProperty GetOneWithVisitor(Expression<Func<VisitorProperty, bool>> filter)
        {
            return _cpContext.VisitorProperties.Include(x => x.Visitor).FirstOrDefault(filter);
        }
    }
}
