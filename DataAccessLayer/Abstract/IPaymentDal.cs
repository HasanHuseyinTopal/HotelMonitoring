using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IPaymentDal : IGenericDAL<Payment>
    {
        IQueryable<Payment> GetAllWithVisitorProperties(Expression<Func<Payment, bool>> filter = null);

    }
}
