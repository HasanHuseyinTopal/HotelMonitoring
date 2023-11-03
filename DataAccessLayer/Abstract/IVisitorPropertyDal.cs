using EntityLayer.Entities;
using System.Linq.Expressions;

namespace DataAccessLayer.Abstract
{
    public interface IVisitorPropertyDal : IGenericDAL<VisitorProperty>
    {
        IQueryable<VisitorProperty> GetAllPropertiesWithVisitors(Expression<Func<VisitorProperty, bool>> filter = null);
        VisitorProperty GetOneWithVisitor(Expression<Func<VisitorProperty, bool>> filter);

    }

}
