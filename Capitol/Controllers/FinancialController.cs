using AutoMapper;
using DataAccessLayer.Abstract;
using EntityLayer.DTOs;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace Capitol.Controllers
{
    public class FinancialController : Controller
    {
        IUnitOfWorkDal _unitOfWork;
        IMapper _mapper;
        public FinancialController(IUnitOfWorkDal unitOfWorkDal, IMapper mapper)
        {
            _unitOfWork = unitOfWorkDal;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult DailyFinancialInOutCome()
        {

            return View();
        }
        [HttpPost]
        public IActionResult DailyFinancialInOutCome(FinancialManagement financialManagement)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.financialDal.Add(financialManagement);
                _unitOfWork.Save();
                return RedirectToAction(nameof(DailyResult), new { DayNumber = 0 });
            }
            return View();
        }
        [HttpGet]
        public IActionResult DeleteFinancial(int FinancialId)
        {
            string previusUrl = HttpContext.Request.Headers["Referer"];

            var delete = _unitOfWork.financialDal.GetOne(x => x.FinancialId == FinancialId);
            if (delete != null)
            {
                _unitOfWork.financialDal.Delete(delete);
                _unitOfWork.Save();
            }
            return Redirect(previusUrl);
        }
        [HttpGet]
        public IActionResult UpdateFinancial(int FinancialId)
        {
            var finance = _unitOfWork.financialDal.GetOne(x => x.FinancialId == FinancialId);
            if (finance != null)
            {
                return View(finance);
            }
            else
            {
                throw new Exception("Böyle bir girdi mevcut değil");
            }
        }
        [HttpPost]
        public IActionResult UpdateFinancial(FinancialManagement financialManagement)
        {
            string previusUrl = HttpContext.Request.Headers["Referer"];
            financialManagement.FinancialUpdateCount += 1;
            _unitOfWork.financialDal.Update(financialManagement);
            _unitOfWork.Save();
            return Redirect(previusUrl);
        }
        [HttpGet]
        public IActionResult DailyResult(int DayNumber)
        {
            var result = new DailyResultDTO();
            result.DailyEncrytpon = Guid.NewGuid();
            result.DayNumber = DayNumber;
            result.FinancialManagements = new();
            var payments = _unitOfWork.paymentDal.GetAll();
            var finances = _unitOfWork.financialDal.GetAll();
            foreach (var finance in finances)
            {
                if (finance.FinancialDate >= DateTime.Now.Date.AddDays(DayNumber).AddHours(12) && finance.FinancialDate <= DateTime.Now.Date.AddDays(DayNumber + 1).AddHours(12))
                {
                    result.FinancialManagements.Add(finance);
                }
            }
            foreach (var payment in payments)
            {
                if (payment.VisitorPaymentDate >= DateTime.Now.Date.AddDays(DayNumber).AddHours(12) && payment.VisitorPaymentDate <= DateTime.Now.Date.AddDays(DayNumber + 1).AddHours(12))
                {
                    if (payment.VisitorPaymentType.Value.ToString() == "Nakit" && payment.VisitorPaymentCurreny.Value.ToString() == "TL")
                    {
                        result.TotalCashTL += payment.VisitorPayment;
                    }
                    else if (payment.VisitorPaymentType.Value.ToString() == "Nakit" && payment.VisitorPaymentCurreny.Value.ToString() == "EURO")
                    {
                        result.TotalCashEURO += payment.VisitorPayment;
                    }
                    else if (payment.VisitorPaymentType.Value.ToString() == "Nakit" && payment.VisitorPaymentCurreny.Value.ToString() == "USD")
                    {
                        result.TotalCashUSD += payment.VisitorPayment;
                    }
                    else if (payment.VisitorPaymentType.Value.ToString() == "KrediKartı" && payment.VisitorPaymentCurreny.Value.ToString() == "TL")
                    {
                        result.TotalKreditTL += payment.VisitorPayment;
                    }
                    else if (payment.VisitorPaymentType.Value.ToString() == "KrediKartı" && payment.VisitorPaymentCurreny.Value.ToString() == "EURO")
                    {
                        result.TotalKreditEURO += payment.VisitorPayment;
                    }
                    else if (payment.VisitorPaymentType.Value.ToString() == "KrediKartı" && payment.VisitorPaymentCurreny.Value.ToString() == "USD")
                    {
                        result.TotalKreditUSD += payment.VisitorPayment;
                    }
                }
            }
            return View(result);
        }
    }
}
