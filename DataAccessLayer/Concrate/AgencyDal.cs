using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrate
{
    public class AgencyDal : GenericDal<Agency>, IAgencyDal
    {
        public AgencyDal(CpContext context) : base(context)
        {
        }
    }
}
