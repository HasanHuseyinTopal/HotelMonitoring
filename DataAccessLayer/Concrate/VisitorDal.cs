using DataAccessLayer.Abstract;
using EntityLayer.DTOs;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32.SafeHandles;
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

        public IQueryable<Visitor> GetAllVisitorsWithPaymentAndProperties(Expression<Func<Visitor, bool>> filter = null)
        {
            var query = _cpContext.Visitors.Include(x => x.Payments).Include(x => x.VisitorProperties).AsQueryable();
            if (filter != null)
                query = query.Where(filter);
            return query;
        }
        public IQueryable<Visitor> GetAllVisitorsWithProperties(Expression<Func<Visitor, bool>> filter = null)
        {
            var query = _cpContext.Visitors.Include(x => x.VisitorProperties).AsQueryable();
            if (filter != null)
                query = query.Where(filter);
            return query;
        }
        public Visitor GetOneWithProperties(Expression<Func<Visitor, bool>> filter = null)
        {
            return _cpContext.Visitors.Include(x => x.VisitorProperties).FirstOrDefault(filter);
        }
        public Visitor GetOneWithPropertiesAndHistories(Expression<Func<Visitor, bool>> filter = null)
        {
            return _cpContext.Visitors.Include(x => x.VisitorProperties).Include(x=> x.VisitorHistories).FirstOrDefault(filter);
        }
        public void GetChangedVisitorProperties(int VisitorId, UpdateVisitorDTO UpdatedVisitor)
        {
            var history = new VisitorHistory();

            var visitor = _cpContext.Visitors.Include(x => x.VisitorProperties).FirstOrDefault(x => x.VisitorId == VisitorId);

            UpdatedVisitor.VisitorRoomPrice = UpdatedVisitor.VisitorTotalRoomPrice != null ? UpdatedVisitor.VisitorTotalRoomPrice / (UpdatedVisitor.VisitorEndDate.Date - UpdatedVisitor.VisitorStartDate.Date).Days : UpdatedVisitor.VisitorRoomPrice;
            bool? result = false;
            if (visitor != null)
            {
                history.VisitorId = VisitorId;
                history.VisitorUpdatedDate = DateTime.Now;



                for (int i = 0; i < visitor.VisitorProperties!.Count(); i++)
                {
                    if (UpdatedVisitor.VisitorProperties!.Count() > i)
                    {
                        if (visitor.VisitorProperties[i].VisitorName != UpdatedVisitor.VisitorProperties![i].VisitorName || visitor.VisitorProperties[i].VisitorSurName != UpdatedVisitor.VisitorProperties![i].VisitorSurName)
                        {
                            history.VisitorNamesIsChanged = true;
                            result = true;
                            break;
                        }
                    }
                    else
                    {
                        history.VisitorNamesIsChanged = true;
                        result = true;
                        break;
                    }
                }

                if (visitor.VisitorStartDate != UpdatedVisitor.VisitorStartDate)
                {
                    history.VisitorNewStartDate = UpdatedVisitor.VisitorStartDate;
                    history.VisitorOldStartDate = visitor.VisitorStartDate;
                    result = true;
                }


                if (visitor.VisitorEndDate != UpdatedVisitor.VisitorEndDate)
                {
                    history.VisitorNewEndDate = UpdatedVisitor.VisitorEndDate;
                    history.VisitorOldEndDate = visitor.VisitorEndDate;
                    result = true;
                }


                if (visitor.VisitorRoomNumber != UpdatedVisitor.VisitorRoomNumber)
                {
                    history.VisitorNewRoomNumber = UpdatedVisitor.VisitorRoomNumber;
                    history.VisitorOldRoomNumber = visitor.VisitorRoomNumber;
                    result = true;
                }


                if (visitor.VisitorRoomPrice != UpdatedVisitor.VisitorRoomPrice)
                {
                    history.VisitorNewRoomPrice = UpdatedVisitor.VisitorRoomPrice;
                    history.VisitorOldRoomPrice = visitor.VisitorRoomPrice;
                    if (UpdatedVisitor.VisitorPaymentCurrency != null && (UpdatedVisitor.VisitorPaymentCurrency != visitor.VisitorPaymentCurrency))
                    {
                        history.VisitorNewCurrency = UpdatedVisitor.VisitorPaymentCurrency;
                        history.VisitorOldCurrency = visitor.VisitorPaymentCurrency;
                    }
                    result = true;
                }


                if (visitor.VisitorPaymentCurrency != UpdatedVisitor.VisitorPaymentCurrency)
                {
                    history.VisitorNewCurrency = UpdatedVisitor.VisitorPaymentCurrency;
                    history.VisitorOldCurrency = visitor.VisitorPaymentCurrency;
                    if (UpdatedVisitor.VisitorRoomPrice != null && (UpdatedVisitor.VisitorRoomPrice != visitor.VisitorRoomPrice))
                    {
                        history.VisitorNewRoomPrice = UpdatedVisitor.VisitorRoomPrice;
                        history.VisitorOldRoomPrice = visitor.VisitorRoomPrice;
                    }
                    result = true;
                }

                if (result == true)
                {
                    _cpContext.VisitorHistories.Add(history);
                    _cpContext.SaveChanges();
                }
            }
        }
    }
}
