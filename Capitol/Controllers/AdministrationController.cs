using DataAccessLayer.Abstract;
using DataAccessLayer.Concrate;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Capitol.Controllers
{
    [Authorize(Policy = "ADMN")]
    public class AdministrationController : Controller
    {
        IUnitOfWorkDal _unitOfWork;
        public AdministrationController(IUnitOfWorkDal unitOfWorkDal)
        {
            _unitOfWork = unitOfWorkDal;
        }

        [HttpGet]
        public IActionResult AddAgency()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddAgency(Agency agency)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.agencyDal.Add(agency);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(ShowAllAgencies));

        }
        [HttpGet]
        public IActionResult UpdateAgency(int AgencyId)
        {
            var agency = _unitOfWork.agencyDal.GetOne(x => x.AgencyId == AgencyId);
            if (agency != null)
            {
                return View(agency);
            }
            return View();
        }
        [HttpPost]
        public IActionResult UpdateAgency(Agency agency)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.agencyDal.Update(agency);
                _unitOfWork.Save();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Hatalı Girdi");
                return View();
            }
            return RedirectToAction(nameof(ShowAllAgencies));

        }
        [HttpGet]
        public IActionResult ChangeAgencyStatus(int AgencyId)
        {
            var agency = _unitOfWork.agencyDal.GetOne(x => x.AgencyId == AgencyId);
            if (agency != null)
            {
                if (agency.AgencyStatus == true)
                    agency.AgencyStatus = false;
                else
                    agency.AgencyStatus = true;
                _unitOfWork.agencyDal.Update(agency);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(ShowAllAgencies));
        }

        [HttpGet]
        public IActionResult DeleteAgency(int AgencyId)
        {
            var agency = _unitOfWork.agencyDal.GetOne(x => x.AgencyId == AgencyId);
            if (agency != null)
            {
                _unitOfWork.agencyDal.Delete(agency);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(ShowAllAgencies));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAgencies()
        {
            var allAgencies = await _unitOfWork.agencyDal.GetAll(x => x.AgencyStatus == true).ToListAsync();
            return Ok(allAgencies);
        }
        [HttpGet]
        public async Task<IActionResult> ShowAllAgencies()
        {
            var allAgencies = await _unitOfWork.agencyDal.GetAll().ToListAsync();
            return View(allAgencies);
        }
        [HttpGet]
        public IActionResult Management()
        {
            return View();
        }
    }
}
