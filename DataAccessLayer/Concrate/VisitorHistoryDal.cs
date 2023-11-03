using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrate
{
    public class VisitorHistoryDal : GenericDal<VisitorHistory>, IVisitorHistoryDal
    {
        public VisitorHistoryDal(CpContext context) : base(context)
        {
        }
    }
}
