using DataAccessLayer.Abstract;


namespace DataAccessLayer.Concrate
{
    public class UnitOfWorkDal : IUnitOfWorkDal
    {
        private readonly CpContext _CpContext;
        private readonly Lazy<IVisitorDal> _VisitorDal;
        private readonly Lazy<IRoomDal> _RoomDal;
        private readonly Lazy<IPaymentDal> _PaymentDal;
        private readonly Lazy<IFinancialDal> _FinancialDal;
        private readonly Lazy<IReportDal> _ReportDal;
        private readonly Lazy<IVisitorPropertyDal> _VisitorPropertyDal;
        private readonly Lazy<IVisitorHistoryDal> _VisitorHistoryDal;
        private readonly Lazy<IAgencyDal> _AgencyDal;

        public UnitOfWorkDal(CpContext cpContext)
        {
            _CpContext = cpContext;
            _VisitorDal = new Lazy<IVisitorDal>(() => new VisitorDal(_CpContext));
            _RoomDal = new Lazy<IRoomDal>(() => new RoomDal(_CpContext));
            _PaymentDal = new Lazy<IPaymentDal>(() => new PaymentDal(_CpContext));
            _FinancialDal = new Lazy<IFinancialDal>(() => new FinancialDal(_CpContext));
            _ReportDal = new Lazy<IReportDal>(() => new ReportDal(_CpContext));
            _VisitorPropertyDal = new Lazy<IVisitorPropertyDal>(() => new VisitorPropertyDal(_CpContext));
            _VisitorHistoryDal = new Lazy<IVisitorHistoryDal>(() => new VisitorHistoryDal(_CpContext));
            _AgencyDal = new Lazy<IAgencyDal>(() => new AgencyDal(_CpContext));

        }
        public void Save()
        {
            _CpContext.SaveChanges();
        }
        public IVisitorDal visitorDal => _VisitorDal.Value;
        public IRoomDal roomDal => _RoomDal.Value;
        public IPaymentDal paymentDal => _PaymentDal.Value;
        public IFinancialDal financialDal => _FinancialDal.Value;
        public IReportDal ReportDal => _ReportDal.Value;
        public IVisitorPropertyDal visitorPropertyDal => _VisitorPropertyDal.Value;
        public IVisitorHistoryDal visitorHistoryDal => _VisitorHistoryDal.Value;
        public IAgencyDal agencyDal => _AgencyDal.Value;
    }
}


