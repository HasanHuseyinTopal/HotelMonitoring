using EntityLayer.DTOs;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IVisitorDal : IGenericDAL<Visitor>
    {
        IQueryable<Visitor> GetAllVisitorsWithPaymentAndProperties(Expression<Func<Visitor, bool>> filter = null);
        IQueryable<Visitor> GetAllVisitorsWithProperties(Expression<Func<Visitor, bool>> filter = null);
        Visitor GetOneWithProperties(Expression<Func<Visitor, bool>> filter = null);
        Visitor GetOneWithPropertiesAndHistories(Expression<Func<Visitor, bool>> filter = null);
        void GetChangedVisitorProperties(int VisitorId, UpdateVisitorDTO NewVisitor);
    }
}
