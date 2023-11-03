using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IUnitOfWorkDal
    {
        IVisitorDal visitorDal { get; }
        IRoomDal roomDal { get; }
        IPaymentDal paymentDal { get; }
        IFinancialDal financialDal { get; } 
        IReportDal ReportDal { get; }   
        IVisitorPropertyDal visitorPropertyDal { get; }
        IVisitorHistoryDal visitorHistoryDal { get; }
        IAgencyDal agencyDal { get; }
        void Save();
    }
}
