using AutoMapper;
using DataAccessLayer.Abstract;
using EntityLayer.DTOs;
using EntityLayer.Entities;
using EntityLayer.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Capitol.Controllers
{

    public class VisitorController : Controller
    {
        IUnitOfWorkDal _unitOfWork;
        IMapper _mapper;
        IWebHostEnvironment _webHostEnvironment;
        public VisitorController(IUnitOfWorkDal unitOfWorkDal, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWorkDal;
            _webHostEnvironment = webHostEnvironment;

        }
        [HttpGet]
        public IActionResult AddVisitor(int VisitorId, int RoomNumber, DateTime CheckInDate)
        {
            ViewBag.CheckInDate = CheckInDate;
            if (VisitorId == 0)
            {
                ViewBag.previusUrl = HttpContext.Request.Headers["Referer"];
                string isFromRoom = HttpContext.Request.Headers["Referer"];

                if (isFromRoom.Contains("roomNumber"))
                {
                    isFromRoom = isFromRoom.Split("=")[1];
                    ViewBag.Blokaj = isFromRoom;
                }
                if (RoomNumber != 0)
                {
                    ViewBag.Blokaj = RoomNumber;
                }
                return View();
            }
            else
            {
                var visitor = _unitOfWork.visitorDal.GetOneWithProperties(x => x.VisitorId == VisitorId);
                AddVisitorDTO shiftedVisiter = new AddVisitorDTO();
                shiftedVisiter.VisitorPropertyDTOs = visitor.VisitorProperties;
                shiftedVisiter.VisitorPaymentCurrency = visitor.VisitorPaymentCurrency;
                shiftedVisiter.VisitorRoomPrice = visitor.VisitorRoomPrice;
                shiftedVisiter.VisitorPhoneNumber = visitor.VisitorPhoneNumber;
                shiftedVisiter.VisitorCount = visitor.VisitorCount;
                shiftedVisiter.VisitorRezervation = visitor.VisitorRezervation;
                shiftedVisiter.VisitorDescription = visitor.VisitorDescription?.ToUpper();
                shiftedVisiter.VisitorStartDate = visitor.VisitorEndDate.Date;
                shiftedVisiter.VisitorEndDate = visitor.VisitorEndDate.AddDays(1).Date;
                shiftedVisiter.VisitorDontChangeRoom = visitor.VisitorDontChangeRoom;
                shiftedVisiter.VisitorPreviusId = VisitorId;
                return View(shiftedVisiter);
            }
        }
        [HttpPost]
        public IActionResult AddVisitor(AddVisitorDTO addVisitorDTO, bool DoPayment, bool DontChangeRoom, string? previusUrl, List<string> names, List<string> surnames, IFormFile? File)
        {
            Visitor visitor = null;
            if (ModelState.IsValid)
            {
                if (File != null)
                {
                    addVisitorDTO.VisitorFileUrl = FileChangeProcess(File, addVisitorDTO.VisitorFileUrl);
                }

                string pattern = "\\s+";
                visitor = _mapper.Map<Visitor>(addVisitorDTO);
                if (addVisitorDTO.VisitorPreviusId != null)
                    visitor.VisitorPreviusId = addVisitorDTO.VisitorPreviusId;
                visitor.VisitorDescription = addVisitorDTO.VisitorDescription?.ToUpper();
                visitor.VisitorCount = visitor.VisitorCount == null || visitor.VisitorCount == 0 ? names.Count() : visitor.VisitorCount < names.Count() ? names.Count() : visitor.VisitorCount;

                for (int i = 0; i < names.Count(); i++)
                {
                    VisitorProperty visitorProperty = new();
                    if (i == 0)
                        names[i] = Regex.Replace(names[i], pattern, " ");
                    names[i] = names[i].Trim(' ');
                    var visitorNameLength = names[i].Split(" ").Length;
                    string name = "";
                    if (visitorNameLength > 1)
                    {
                        for (int j = 0; j < visitorNameLength; j++)
                        {
                            name += names[i].Split(" ")[j].Substring(0, 1).ToUpper() + names[i].Split(" ")[j].Substring(1, names[i].Split(" ")[j].Length - 1).ToLower() + " ";
                        }
                    }
                    else
                    {
                        name = names[i].Substring(0, 1).ToUpper() + names[i].Substring(1, names[i].Length - 1).ToLower();
                    }
                    names[i] = name.Trim(' ');
                    surnames[i] = surnames[i] != null ? surnames[i].ToUpper() : "";
                    visitorProperty.VisitorName = names[i];
                    visitorProperty.VisitorSurName = surnames[i];
                    visitorProperty.VisitorId = visitor.VisitorId;
                    visitorProperty.VisitorActive = true;
                    visitor.VisitorProperties.Add(visitorProperty);
                    visitor.VisitorDescription = visitor.VisitorDescription?.ToUpper();
                    visitor.VisitorDontChangeRoom = DontChangeRoom;
                }

                visitor.VisitorPreviusId = addVisitorDTO.VisitorPreviusId == null ? null : addVisitorDTO.VisitorPreviusId;
                visitor.VisitorRoomPrice = addVisitorDTO.VisitorTotalRoomPrice != null ? addVisitorDTO.VisitorTotalRoomPrice / (visitor.VisitorEndDate.Date - visitor.VisitorStartDate.Date).Days : addVisitorDTO.VisitorRoomPrice;
                visitor.VisitorTotalRoomPrice = null;
                if (DoPayment == true)
                {
                    if (visitor.VisitorRoomPrice == null || visitor.VisitorPaymentCurrency == null)
                    {
                        ViewBag.Blokaj = addVisitorDTO.VisitorRoomNumber;
                        ViewBag.CheckInDate = addVisitorDTO.VisitorStartDate;
                        ModelState.AddModelError(string.Empty, "Oda fiyatı ve para birimi seçilmelidir");
                        return View(addVisitorDTO);
                    }

                    _unitOfWork.visitorDal.Add(visitor);
                    _unitOfWork.Save();
                    if (visitor.VisitorPreviusId != null)
                    {
                        var nextVisitor = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == visitor.VisitorPreviusId);
                        nextVisitor.VisitorNextId = visitor.VisitorId;
                        _unitOfWork.visitorDal.Update(nextVisitor);
                    }
                    _unitOfWork.Save();
                    return RedirectToAction(nameof(AddPaymentToVisitor), new { VisitorId = visitor.VisitorId });
                }
                else
                {
                    _unitOfWork.visitorDal.Add(visitor);
                    _unitOfWork.Save();
                }
            }
            else
            {
                return View();
            }
            if (visitor.VisitorPreviusId != null)
            {
                var nextVisitor = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == visitor.VisitorPreviusId);
                nextVisitor.VisitorNextId = visitor.VisitorId;
                _unitOfWork.visitorDal.Update(nextVisitor);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(SelectedVisitors), new { visitorId = visitor.VisitorId });
        }
        public void FileDeleteProcess(string imageUrl, string wwwRoot)
        {
            string oldFilePath = Path.Combine(wwwRoot, imageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
        }
        public IActionResult FileDeleteAction(string imageUrl, int? visitorId)
        {
            string wwwRoot = _webHostEnvironment.WebRootPath;
            string oldFilePath = Path.Combine(wwwRoot, imageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
            if (visitorId != null)
            {
                var visitor = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == visitorId);
                visitor.VisitorFileUrl = null;
                _unitOfWork.visitorDal.Update(visitor);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(SelectedVisitors), new { VisitorId = visitorId });
        }
        public string FileChangeProcess(IFormFile file, string imageUrl)
        {
            string wwwRoot = _webHostEnvironment.WebRootPath;
            string fileHashedName = Guid.NewGuid().ToString() + "--" + file.FileName;
            string fileUploadPath = Path.Combine(wwwRoot, @"Files");
            string fileStreamPath = Path.Combine(fileUploadPath, fileHashedName);

            if (imageUrl != null)
            {
                FileDeleteProcess(imageUrl, wwwRoot);
            }
            using (var fileStream = new FileStream(fileStreamPath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            string returnUrl = @"\Files\" + fileHashedName;
            return returnUrl;
        }
        [HttpGet]
        public IActionResult RemoveVisitorName([FromQuery] int VisitorPropertyId)
        {
            string previusUrl = HttpContext.Request.Headers["Referer"];
            var name = _unitOfWork.visitorPropertyDal.GetOneWithVisitor(x => x.VisitorPropertiyId == VisitorPropertyId);
            name.Visitor.VisitorCount = _unitOfWork.visitorPropertyDal.GetAll(x => x.VisitorId == name.VisitorId).Count() - 1;

            _unitOfWork.visitorDal.Update(name.Visitor);
            if (_unitOfWork.visitorPropertyDal.GetAll(x => x.VisitorId == name.VisitorId).Count() <= 1)
            {
                return Redirect(previusUrl);
            }
            _unitOfWork.visitorPropertyDal.Delete(name);
            _unitOfWork.Save();
            return Redirect(previusUrl);
        }

        [HttpGet]
        public IActionResult DeletePaymentSection([FromQuery] int PaymentId)
        {
            var deletePay = _unitOfWork.paymentDal.GetOne(x => x.PaymentId == PaymentId);
            if (deletePay != null)
            {
                _unitOfWork.paymentDal.Delete(deletePay);
                _unitOfWork.Save();
                var paymentCount = _unitOfWork.paymentDal.GetAll(x => x.VisitorId == deletePay.VisitorId).Count();
                if (paymentCount >= 1)
                {
                    return RedirectToAction(nameof(AddPaymentToVisitor), new { VisitorId = deletePay.VisitorId });
                }
                return RedirectToAction(nameof(SelectedVisitors), new { VisitorId = deletePay.VisitorId });
            }
            else
            {
                throw new Exception("Böyle bir ödeme mevcut değil");
            }
        }

        [HttpGet]
        public IActionResult AddPaymentToVisitor(int VisitorId)
        {
            var payment = new PaymentDTO();
            var visitor = _unitOfWork.visitorDal.GetOneWithProperties(x => x.VisitorId == VisitorId);
            payment.VisitorId = VisitorId;
            payment.VisitorName = visitor.VisitorProperties[0].VisitorName; ;
            payment.VisitorName = payment.VisitorName.Split(" ").Length > 1 ? payment.VisitorName.Split(" ")[0] : payment.VisitorName;
            payment.VisitorTotalPrice = (visitor.VisitorEndDate.Date - visitor.VisitorStartDate.Date).Days * visitor.VisitorRoomPrice;
            payment.VisitorTotalPriceCurrency = visitor.VisitorPaymentCurrency;
            return View(payment);
        }
        [HttpPost]
        public IActionResult AddPaymentToVisitor(PaymentDTO paymentDTO)
        {
            Payment payment = new()
            {
                VisitorPayment = paymentDTO.VisitorPayment,
                VisitorPaymentCurreny = paymentDTO.VisitorPaymentCurreny,
                VisitorPaymentType = paymentDTO.VisitorPaymentType,
                VisitorPaymentDate = paymentDTO.VisitorPaymentDate,
                VisitorId = paymentDTO.VisitorId
            };
            _unitOfWork.paymentDal.Add(payment);
            _unitOfWork.Save();

            return RedirectToAction(nameof(SelectedVisitors), new { visitorId = paymentDTO.VisitorId });
        }
        [HttpGet]
        public IActionResult DeleteVisitor(int id)
        {
            string? previusUrl = HttpContext.Request.Headers["Referer"];
            var deleteVisitor = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == id);
            if (deleteVisitor.VisitorFileUrl != null)
            {
                FileDeleteProcess(deleteVisitor.VisitorFileUrl, _webHostEnvironment.WebRootPath);
            }
            if (deleteVisitor != null)
            {
                if (deleteVisitor.VisitorNextId != null)
                {
                    var deletedNext = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == deleteVisitor.VisitorNextId);
                    if (deletedNext != null)
                    {
                        deletedNext.VisitorPreviusId = null;
                        _unitOfWork.visitorDal.Update(deletedNext);
                    }
                }
                if (deleteVisitor.VisitorPreviusId != null)
                {
                    var deletePrevius = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == deleteVisitor.VisitorPreviusId);
                    if (deletePrevius != null)
                    {
                        deletePrevius.VisitorNextId = null;
                        _unitOfWork.visitorDal.Update(deletePrevius);
                    }
                }
                _unitOfWork.visitorDal.Delete(deleteVisitor);
                _unitOfWork.Save();
            }
            else
            {
                throw new Exception("Böyle bir ziyaretçi mevcut değil");
            }
            if (previusUrl == null)
                return RedirectToAction(nameof(DailyVisitors));
            else
            {
                int monthNumber = ((deleteVisitor.VisitorStartDate.Year - DateTime.Now.Year) * 12) + (deleteVisitor.VisitorStartDate.Month - DateTime.Now.Month);
                return RedirectToAction(nameof(MonthlyView), new { monthNumber = monthNumber });
            }
        }
        [HttpGet]
        public IActionResult UpdateVisitor(int id)
        {
            ViewBag.previusUrl = HttpContext.Request.Headers["Referer"];
            var visitor = _unitOfWork.visitorDal.GetOneWithProperties(x => x.VisitorId == id);
            if (visitor != null)
            {
                var updateVisitorDto = _mapper.Map<UpdateVisitorDTO>(visitor);
                return View(updateVisitorDto);
            }
            else
            {
                return View(nameof(DailyVisitors));
            }
        }
        [HttpPost]
        public IActionResult UpdateVisitor(UpdateVisitorDTO updateVisitorDTO, List<string> names, List<string> surnames, bool DoPayment, bool DontChangeRoom, string? previusUrl, IFormFile? File)
        {
            if (ModelState.IsValid)
            {


                string pattern = "\\s+";
                var visitor = _unitOfWork.visitorDal.GetOneWithProperties(x => x.VisitorId == updateVisitorDTO.VisitorId);

                if (File != null)
                    updateVisitorDTO.VisitorFileUrl = FileChangeProcess(File, updateVisitorDTO.VisitorFileUrl);
                else
                    updateVisitorDTO.VisitorFileUrl = updateVisitorDTO.VisitorFileUrl;

                updateVisitorDTO.VisitorCount = updateVisitorDTO.VisitorCount == null || updateVisitorDTO.VisitorCount == 0 ? names.Count() : updateVisitorDTO.VisitorCount > names.Count() ? updateVisitorDTO.VisitorCount : names.Count();
                updateVisitorDTO.VisitorProperties = visitor.VisitorProperties;

                for (int i = 0; i < names.Count(); i++)
                {
                    if (i == 0)
                        names[i] = Regex.Replace(names[i], pattern, " ");
                    names[i] = names[i].Trim(' ');
                    var visitorNameLength = names[i].Split(" ").Length;
                    string name = "";
                    if (visitorNameLength > 1)
                    {
                        for (int j = 0; j < visitorNameLength; j++)
                        {
                            name += names[i].Split(" ")[j].Substring(0, 1).ToUpper() + names[i].Split(" ")[j].Substring(1, names[i].Split(" ")[j].Length - 1).ToLower() + " ";
                        }
                    }
                    else
                    {
                        name = names[i].Substring(0, 1).ToUpper() + names[i].Substring(1, names[i].Length - 1).ToLower();
                    }
                    names[i] = name.Trim(' ');
                    surnames[i] = surnames[i] != null ? surnames[i].ToUpper() : "";

                    if ((i + 1) > updateVisitorDTO.VisitorProperties.Count())
                    {
                        VisitorProperty visitorProperty = new VisitorProperty();
                        visitorProperty.VisitorName = names[i];
                        visitorProperty.VisitorSurName = surnames[i];
                        visitorProperty.VisitorId = updateVisitorDTO.VisitorProperties[0].VisitorId;
                        visitorProperty.VisitorActive = true;
                        visitor.VisitorDontChangeRoom = DontChangeRoom;
                        _unitOfWork.visitorPropertyDal.Add(visitorProperty);
                        _unitOfWork.Save();
                    }
                    else
                    {
                        updateVisitorDTO.VisitorProperties[i].VisitorName = names[i];
                        updateVisitorDTO.VisitorProperties[i].VisitorSurName = surnames[i];
                    }
                }
                _unitOfWork.visitorDal.GetChangedVisitorProperties(visitor.VisitorId, updateVisitorDTO);

                visitor = _mapper.Map<Visitor>(updateVisitorDTO);
                visitor.VisitorDescription = visitor.VisitorDescription?.ToUpper();
                visitor.VisitorDontChangeRoom = DontChangeRoom;
                visitor.VisitorRoomPrice = updateVisitorDTO.VisitorTotalRoomPrice != null ? updateVisitorDTO.VisitorTotalRoomPrice / (visitor.VisitorEndDate.Date - visitor.VisitorStartDate.Date).Days : updateVisitorDTO.VisitorRoomPrice;
                visitor.VisitorTotalRoomPrice = null;

                if (DoPayment == true)
                {
                    if (updateVisitorDTO.VisitorRoomPrice == null || updateVisitorDTO.VisitorPaymentCurrency == null)
                    {
                        ModelState.AddModelError(string.Empty, "Oda fiyatı ve para birimi seçilmelidir");
                        return View(updateVisitorDTO);
                    }
                    _unitOfWork.visitorDal.Update(visitor);
                    _unitOfWork.Save();
                    return RedirectToAction(nameof(AddPaymentToVisitor), new { VisitorId = visitor.VisitorId });
                }
                else
                {
                    _unitOfWork.visitorDal.Update(visitor);
                    _unitOfWork.Save();

                }
                if (previusUrl == null)
                    return RedirectToAction(nameof(DailyVisitors));
                else
                    return RedirectToAction(nameof(SelectedVisitors), new { VisitorId = visitor.VisitorId });
            }
            else
            {
                return View(updateVisitorDTO);
            }
        }
        public int DaysInMounth(int monthNumber)
        {
            var Date = DateTime.Now.AddMonths(monthNumber);
            return DateTime.DaysInMonth(Date.Year, Date.Month);
        }
        [Authorize(Policy = "ADMN")]
        [HttpGet]
        public IActionResult HistoryCheck(int VisitorId, string? previusUrl)
        {
            ViewBag.backUrl = previusUrl;
            var VisitorHistory = _unitOfWork.visitorHistoryDal.GetAll(x => x.VisitorId == VisitorId);
            if (VisitorHistory != null && VisitorHistory.Count() >= 1)
            {
                return View(VisitorHistory.OrderBy(x => x.VisitorUpdatedDate).ToList());
            }
            return RedirectToAction(nameof(SelectedVisitors), new { VisitorId = VisitorId });
        }

        [HttpGet]
        public async Task<IActionResult> MonthlyView(int monthNumber, bool showAll, CancellationToken cancellationToken)
        {
            ViewBag.TotalSoldRoomsCount = 0;
            var visitors = await _unitOfWork.visitorDal.GetAllVisitorsWithProperties(x => x.VisitorStartDate.Date >= DateTime.Now.AddMonths(monthNumber - 2).Date && x.VisitorEndDate.Date <= DateTime.Now.AddMonths(monthNumber + 2).Date).ToListAsync();
            var monthlyVisitorDTO = new MonthlyVisitorDTO();

            monthlyVisitorDTO.tableValues = new (int CellState, string Name, int Identity, int count, bool? DontChange, string? TransferDatas)[25, DaysInMounth(monthNumber) + 1];
            monthlyVisitorDTO.monthNumber = monthNumber;

            int balancer = DateTime.Now.AddMonths(monthNumber).Month == 3 ? 3 : 1;

            foreach (var visitor in visitors)
            {
                if (monthlyVisitorDTO.roomNumbers.Contains(visitor.VisitorRoomNumber))
                {
                    for (int i = 0; i < (visitor.VisitorEndDate - visitor.VisitorStartDate).TotalDays; i++)
                    {
                        if (visitor.VisitorStartDate.Day + i <= DaysInMounth(monthNumber) &&
                            DateTime.Now.AddMonths(monthNumber).Month == visitor.VisitorStartDate.AddDays(i - (i > 0 ? balancer - i > 0 ? i : balancer : i)).Month)
                        {
                            var States = monthlyVisitorDTO.tableValues[Array.IndexOf(monthlyVisitorDTO.roomNumbers, visitor.VisitorRoomNumber), visitor.VisitorStartDate.Day + i];

                            monthlyVisitorDTO.tableValues[24, visitor.VisitorStartDate.Day + i].count += 1;
                            ViewBag.TotalSoldRoomsCount += 1;


                            if (States.CellState != 0)
                                States.CellState = 3;
                            if (visitor.VisitorState != 5 || showAll)
                            {
                                if (i == 0)
                                {
                                    States.Name = visitor.VisitorProperties[0].VisitorName.Split(" ").Length > 1 ? visitor.VisitorProperties[0].VisitorName.Split(" ")[0] : visitor.VisitorProperties[0].VisitorName;
                                    States.Identity = visitor.VisitorId;
                                    if (visitor.VisitorPreviusId != null)
                                    {
                                        States.TransferDatas = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == visitor.VisitorPreviusId).VisitorRoomNumber.ToString();
                                    }
                                }
                                States.DontChange = visitor.VisitorDontChangeRoom;
                                if (States.CellState != 3)
                                    States.CellState = 1;
                            }
                            else if (visitor.VisitorState == 5)
                                States.CellState = 2;

                            monthlyVisitorDTO.tableValues[Array.IndexOf(monthlyVisitorDTO.roomNumbers, visitor.VisitorRoomNumber), visitor.VisitorStartDate.Day + i] = States;
                        }
                        else if (visitor.VisitorStartDate.Day + i + (balancer == 3 ? 1 : 0) >= DaysInMounth(monthNumber + 1) &&
                            DateTime.Now.AddMonths(monthNumber).Month == visitor.VisitorStartDate.AddDays(i).Month)
                        {
                            monthlyVisitorDTO.tableValues[24, visitor.VisitorStartDate.Day + i - DaysInMounth(monthNumber - 1)].count += 1;

                            ViewBag.TotalSoldRoomsCount += 1;

                            var States = monthlyVisitorDTO.tableValues[Array.IndexOf(monthlyVisitorDTO.roomNumbers, visitor.VisitorRoomNumber), visitor.VisitorStartDate.Day + i - DaysInMounth(monthNumber - 1)];

                            if (States.CellState != 0)
                                States.CellState = 3;

                            if (visitor.VisitorState != 5 || showAll)
                            {
                                States.DontChange = visitor.VisitorDontChangeRoom;

                                if (States.CellState != 3)
                                    States.CellState = 1;
                            }
                            else if (visitor.VisitorState == 5)
                                States.CellState = 2;

                            monthlyVisitorDTO.tableValues[Array.IndexOf(monthlyVisitorDTO.roomNumbers, visitor.VisitorRoomNumber), visitor.VisitorStartDate.Day + i - DaysInMounth(monthNumber - 1)] = States;
                        }
                    }
                }
            }
            double percent = ViewBag.TotalSoldRoomsCount;
            monthlyVisitorDTO.TotalRoomCount = DaysInMounth(monthNumber) * 24;

            for (int i = 1; i <= DaysInMounth(monthNumber); i++)
            {
                if (DateTime.Now.Day <= i)
                {
                    monthlyVisitorDTO.RestOfRoomsCount += monthlyVisitorDTO.tableValues[24, i].count;
                }
            }
            monthlyVisitorDTO.RestOfRoomsCount = (monthlyVisitorDTO.TotalRoomCount - ((DateTime.Now.Day - 1) * 24)) - monthlyVisitorDTO.RestOfRoomsCount;

            if (DateTime.Now.Month != DateTime.Now.AddMonths(monthNumber).Month || DateTime.Now.Year != DateTime.Now.AddMonths(monthNumber).Year)
            {
                monthlyVisitorDTO.RestOfRoomsCount = DaysInMounth(monthNumber) * 24 - Convert.ToInt32(percent);
            }
            monthlyVisitorDTO.MaxMonthlyPercent = (percent + monthlyVisitorDTO.RestOfRoomsCount) * 100 / (DaysInMounth(monthNumber) * 24);

            monthlyVisitorDTO.MonthlyPercent = percent * 100 / (DaysInMounth(monthNumber) * 24);

            if (DateTime.Now.AddMonths(monthNumber).Date < DateTime.Now.Date)
            {
                monthlyVisitorDTO.MaxMonthlyPercent = monthlyVisitorDTO.MonthlyPercent;
            }
            monthlyVisitorDTO.ShowAll = showAll;
            return View(monthlyVisitorDTO);
        }

        public async Task<IActionResult> AvailabilityCheck(int monthNumber)
        {
            var visitors = await _unitOfWork.visitorDal.GetAll(x => x.VisitorStartDate.Date >= DateTime.Now.AddMonths(monthNumber - 2).Date && x.VisitorStartDate.Date <= DateTime.Now.AddMonths(monthNumber + 2).Date).ToListAsync();

            var availabilityDTO = new AvailabilityDTO();
            availabilityDTO.availabilty = new (int RoomCount, string SameRoomControl)[4, DaysInMounth(monthNumber) + 1];
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < DaysInMounth(monthNumber) + 1; i++)
                {
                    availabilityDTO.availabilty[j, i].SameRoomControl = "";
                }
            }
            availabilityDTO.monthNumber = monthNumber;

            int balancer = DateTime.Now.AddMonths(monthNumber).Month == 3 ? 3 : 1;

            foreach (var visitor in visitors)
            {

                if (availabilityDTO.roomNumbers.Contains(visitor.VisitorRoomNumber))
                {
                    for (int i = 0; i < (visitor.VisitorEndDate - visitor.VisitorStartDate).TotalDays; i++)
                    {
                        if (visitor.VisitorStartDate.Day + i <= DaysInMounth(monthNumber) &&
                            DateTime.Now.AddMonths(monthNumber).Month == visitor.VisitorStartDate.AddDays(i - (i > 0 ? balancer - i > 0 ? i : balancer : i)).Month)
                        {
                            if (availabilityDTO.SingleRooms.Contains(visitor.VisitorRoomNumber))
                            {
                                if (!availabilityDTO.availabilty[0, visitor.VisitorStartDate.Day + i].SameRoomControl.Contains(visitor.VisitorRoomNumber.ToString())
)
                                {
                                    availabilityDTO.availabilty[0, visitor.VisitorStartDate.Day + i].RoomCount += 1;
                                    availabilityDTO.availabilty[0, visitor.VisitorStartDate.Day + i].SameRoomControl += visitor.VisitorRoomNumber.ToString();
                                }
                            }
                            else if (availabilityDTO.DoubleRooms.Contains(visitor.VisitorRoomNumber))
                            {
                                if (!availabilityDTO.availabilty[1, visitor.VisitorStartDate.Day + i].SameRoomControl.Contains(visitor.VisitorRoomNumber.ToString())
 )
                                {
                                    availabilityDTO.availabilty[1, visitor.VisitorStartDate.Day + i].RoomCount += 1;
                                    availabilityDTO.availabilty[1, visitor.VisitorStartDate.Day + i].SameRoomControl += visitor.VisitorRoomNumber.ToString();
                                }
                            }
                            else if (availabilityDTO.DoubleRoomsWithBalcony.Contains(visitor.VisitorRoomNumber))
                            {
                                if (!availabilityDTO.availabilty[2, visitor.VisitorStartDate.Day + i].SameRoomControl.Contains(visitor.VisitorRoomNumber.ToString())
)
                                {
                                    availabilityDTO.availabilty[2, visitor.VisitorStartDate.Day + i].RoomCount += 1;
                                    availabilityDTO.availabilty[2, visitor.VisitorStartDate.Day + i].SameRoomControl += visitor.VisitorRoomNumber.ToString();
                                }
                            }
                            else if (availabilityDTO.TripleRooms.Contains(visitor.VisitorRoomNumber))
                            {

                                if (!availabilityDTO.availabilty[3, visitor.VisitorStartDate.Day + i].SameRoomControl.Contains(visitor.VisitorRoomNumber.ToString())
 )
                                {
                                    availabilityDTO.availabilty[3, visitor.VisitorStartDate.Day + i].RoomCount += 1;
                                    availabilityDTO.availabilty[3, visitor.VisitorStartDate.Day + i].SameRoomControl += visitor.VisitorRoomNumber.ToString();
                                }
                            }
                        }
                        else if (visitor.VisitorStartDate.Day + i >= DaysInMounth(monthNumber) &&
                            DateTime.Now.AddMonths(monthNumber).Month == visitor.VisitorStartDate.AddDays(i).Month)
                        {
                            if (availabilityDTO.SingleRooms.Contains(visitor.VisitorRoomNumber))
                            {
                                if (!availabilityDTO.availabilty[0, visitor.VisitorStartDate.Day + i - DaysInMounth(monthNumber - 1)].SameRoomControl.Contains(visitor.VisitorRoomNumber.ToString())
)
                                {
                                    availabilityDTO.availabilty[0, visitor.VisitorStartDate.Day + i - DaysInMounth(monthNumber - 1)].RoomCount += 1;
                                    availabilityDTO.availabilty[0, visitor.VisitorStartDate.Day + i - DaysInMounth(monthNumber - 1)].SameRoomControl += visitor.VisitorRoomNumber.ToString();
                                }
                            }
                            else if (availabilityDTO.DoubleRooms.Contains(visitor.VisitorRoomNumber))
                            {
                                if (!availabilityDTO.availabilty[1, visitor.VisitorStartDate.Day + i - DaysInMounth(monthNumber - 1)].SameRoomControl.Contains(visitor.VisitorRoomNumber.ToString())
)
                                {
                                    availabilityDTO.availabilty[1, visitor.VisitorStartDate.Day + i - DaysInMounth(monthNumber - 1)].RoomCount += 1;
                                    availabilityDTO.availabilty[1, visitor.VisitorStartDate.Day + i - DaysInMounth(monthNumber - 1)].SameRoomControl += visitor.VisitorRoomNumber.ToString();
                                }
                            }
                            else if (availabilityDTO.DoubleRoomsWithBalcony.Contains(visitor.VisitorRoomNumber))
                            {
                                if (!availabilityDTO.availabilty[2, visitor.VisitorStartDate.Day + i - DaysInMounth(monthNumber - 1)].SameRoomControl.Contains(visitor.VisitorRoomNumber.ToString())
)
                                {
                                    availabilityDTO.availabilty[2, visitor.VisitorStartDate.Day + i - DaysInMounth(monthNumber - 1)].RoomCount += 1;
                                    availabilityDTO.availabilty[2, visitor.VisitorStartDate.Day + i - DaysInMounth(monthNumber - 1)].SameRoomControl += visitor.VisitorRoomNumber.ToString();
                                }
                            }
                            else if (availabilityDTO.TripleRooms.Contains(visitor.VisitorRoomNumber))
                            {
                                if (!availabilityDTO.availabilty[3, visitor.VisitorStartDate.Day + i - DaysInMounth(monthNumber - 1)].SameRoomControl.Contains(visitor.VisitorRoomNumber.ToString())
)
                                {
                                    availabilityDTO.availabilty[3, visitor.VisitorStartDate.Day + i - DaysInMounth(monthNumber - 1)].RoomCount += 1;
                                    availabilityDTO.availabilty[3, visitor.VisitorStartDate.Day + i - DaysInMounth(monthNumber - 1)].SameRoomControl += visitor.VisitorRoomNumber.ToString();
                                }
                            }
                        }
                    }
                }
            }
            return View(availabilityDTO);
        }

        [HttpGet]
        public async Task<IActionResult> DailyVisitors(CancellationToken cancellationToken)
        {
            var visitors = await _unitOfWork.visitorDal.
                GetAllVisitorsWithPaymentAndProperties(x => x.VisitorEndDate.Date >= DateTime.Now.Date && x.VisitorStartDate.Date <= DateTime.Now.Date && x.VisitorState != 5).ToListAsync();
            var roomState = _unitOfWork.roomDal.GetAll();
            DailyVisitorDTOs dailyVisitorDTOs = new();


            foreach (var visitor in visitors)
            {
                var customVisitor = new DailyVisitorDTO();
                customVisitor.VisitorRoomNumber = visitor.VisitorRoomNumber;
                customVisitor.VisitorStartDate = visitor.VisitorStartDate;
                customVisitor.VisitorEndDate = visitor.VisitorEndDate;
                customVisitor.VisitorState = visitor.VisitorState == 2 && visitor.VisitorEndDate.Date != DateTime.Now.Date ? "green" : DateTime.Now.Date >= visitor.VisitorStartDate.Date && DateTime.Now < visitor.VisitorStartDate.Date.AddDays(1).AddHours(12) && visitor.VisitorState != 2 ? "blue" : visitor.VisitorEndDate.Date == DateTime.Now.Date ? "red" : "green";
                customVisitor.IsPaymentDone = visitor.VisitorPaymentIsDone;
                customVisitor.VisitorPreviusRoomId = visitor.VisitorPreviusId;
                customVisitor.VisitorNextRoomId = visitor.VisitorNextId;
                if (customVisitor.VisitorState == "blue")
                {
                    dailyVisitorDTOs.CheckInCount += visitor.VisitorCount.Value;
                }
                else if (customVisitor.VisitorState == "green" || customVisitor.VisitorState == "red")
                {
                    dailyVisitorDTOs.StayInCount += visitor.VisitorCount.Value;
                    foreach (var props in visitor.VisitorProperties)
                    {
                        if (!props.VisitorActive)
                        {
                            dailyVisitorDTOs.StayInCount -= 1;
                        }
                    }
                }
                if (customVisitor.VisitorState == "red")
                {
                    dailyVisitorDTOs.CheckOutCount += visitor.VisitorCount.Value;
                    foreach (var props in visitor.VisitorProperties)
                    {
                        if (!props.VisitorActive)
                        {
                            dailyVisitorDTOs.CheckOutCount -= 1;
                        }
                    }
                }
                dailyVisitorDTOs.listOfVisitors.Add(customVisitor);
            }
            dailyVisitorDTOs.listOfVisitors.Sort((c1, c2) => c1.VisitorState.CompareTo(c2.VisitorState));
            dailyVisitorDTOs.CheckInRoomCount = dailyVisitorDTOs.listOfVisitors.Count(x => x.VisitorState == "blue");
            dailyVisitorDTOs.StayInRoomCount = dailyVisitorDTOs.listOfVisitors.Count(x => x.VisitorState == "green") + dailyVisitorDTOs.listOfVisitors.Count(x => x.VisitorState == "red");

            dailyVisitorDTOs.CheckOutRoomCount = dailyVisitorDTOs.listOfVisitors.Count(x => x.VisitorState == "red");

            foreach (var dailyVisitorDTO in dailyVisitorDTOs.roomNumbers)
            {
                dailyVisitorDTOs.VisitorTransferNext[dailyVisitorDTO] = 0;
                dailyVisitorDTOs.VisitorTransferPrevius[dailyVisitorDTO] = 0;
            }

            foreach (var dailyVisitorDTO in dailyVisitorDTOs.listOfVisitors)
            {
                if (!dailyVisitorDTOs.roomStates[dailyVisitorDTO.VisitorRoomNumber].Contains(dailyVisitorDTO.VisitorState))
                {
                    dailyVisitorDTOs.roomStates[dailyVisitorDTO.VisitorRoomNumber] += dailyVisitorDTO.VisitorState;
                }
                if (dailyVisitorDTO.VisitorState == "red" || dailyVisitorDTO.VisitorState == "green")
                {
                    if (dailyVisitorDTO.IsPaymentDone == false && dailyVisitorDTO.resultOfP == 0)
                    {
                        dailyVisitorDTOs.paymentStates[dailyVisitorDTO.VisitorRoomNumber] = false;
                    }

                    dailyVisitorDTO.resultOfP += 1;
                }

                if (dailyVisitorDTO.VisitorNextRoomId != null && dailyVisitorDTO.VisitorNextRoomId != 0)
                {
                    var nextVisitor = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == dailyVisitorDTO.VisitorNextRoomId);
                    if (nextVisitor != null)
                    {
                        if (dailyVisitorDTO.VisitorEndDate == DateTime.Now.Date)
                            dailyVisitorDTOs.VisitorTransferNext[dailyVisitorDTO.VisitorRoomNumber] = dailyVisitorDTO.VisitorNextRoomId == null ? 0 : _unitOfWork.visitorDal.GetOne(x => x.VisitorId == dailyVisitorDTO.VisitorNextRoomId).VisitorRoomNumber;
                    }
                }
                if (dailyVisitorDTO.VisitorPreviusRoomId != null && dailyVisitorDTO.VisitorPreviusRoomId != 0)
                {
                    var previusVisitor = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == dailyVisitorDTO.VisitorPreviusRoomId);
                    if (previusVisitor != null)
                    {
                        if (dailyVisitorDTO.VisitorStartDate == DateTime.Now.Date)
                            dailyVisitorDTOs.VisitorTransferPrevius[dailyVisitorDTO.VisitorRoomNumber] = dailyVisitorDTO.VisitorPreviusRoomId == null ? 0 : _unitOfWork.visitorDal.GetOne(x => x.VisitorId == dailyVisitorDTO.VisitorPreviusRoomId).VisitorRoomNumber;
                    }
                }
            }
            foreach (var room in roomState)
            {
                if (room.RoomState)
                {
                    dailyVisitorDTOs.roomStates[room.RoomNumber] = "black";
                }
            }

            return View(dailyVisitorDTOs);
        }



        [HttpGet]
        public IActionResult AllVisitors()
        {
            var visitors = _unitOfWork.visitorDal.GetAllVisitorsWithProperties(x => x.VisitorEndDate.Date >= DateTime.Now.AddDays(-3).Date && x.VisitorState != 5);
            List<AllVisitorDTO> getAllCustomVisitorDTOs = new List<AllVisitorDTO>();
            foreach (var visitor in visitors)
            {
                foreach (var names in visitor.VisitorProperties)
                {

                    var customVisitor = _mapper.Map<AllVisitorDTO>(visitor);
                    customVisitor.VisitorName = names.VisitorName;
                    customVisitor.VisitorSurName = names.VisitorSurName;
                    customVisitor.VisitorState = visitor.VisitorState == 2 && visitor.VisitorEndDate.Date != DateTime.Now.Date && visitor.VisitorStartDate.Date <= DateTime.Now.Date ? 2 : DateTime.Now.Date >= visitor.VisitorStartDate.Date && DateTime.Now < visitor.VisitorStartDate.Date.AddDays(1).AddHours(12) && visitor.VisitorState != 2 ? 1 : visitor.VisitorEndDate.Date == DateTime.Now.Date ? 0 : visitor.VisitorStartDate.Date < DateTime.Now.Date && visitor.VisitorEndDate.Date > DateTime.Now.Date ? 2 : visitor.VisitorStartDate.Date > DateTime.Now.Date ? 3 : 5;
                    getAllCustomVisitorDTOs.Add(customVisitor);
                }

            }

            return View(getAllCustomVisitorDTOs.OrderBy(x => x.VisitorStartDate).ThenBy(x => x.VisitorRoomNumber).ToList());
        }
        [HttpGet]
        public IActionResult BlockedRoom()
        {
            var blockedRooms = _unitOfWork.roomDal.GetAll();
            List<string> blockedRoomList = new List<string>();
            foreach (var blockedRoom in blockedRooms)
            {
                if (blockedRoom.RoomState == true)
                {
                    blockedRoomList.Add(blockedRoom.RoomNumber.ToString());
                }
            }
            return Ok(blockedRoomList);
        }
        [HttpPost]
        public IActionResult RoomStateChange([FromBody] RoomStateDTO roomStateDTO)
        {
            var room = _unitOfWork.roomDal.GetOne(x => x.RoomNumber == roomStateDTO.RoomNumber);
            room.RoomState = roomStateDTO.RoomState;
            _unitOfWork.roomDal.Update(room);
            _unitOfWork.Save();
            return Ok();
        }
        [HttpPost]
        public IActionResult GetRoomState([FromBody] int roomNumber)
        {
            if (roomNumber > 100)
            {
                var roomState = _unitOfWork.roomDal.GetOne(x => x.RoomNumber == roomNumber).RoomState;
                return Ok(roomState);
            }
            return Ok();
        }
        [HttpGet]
        public IActionResult CheckForCrashDate(string startDate, string endDate, string roomNumber)
        {

            var StartedDate = DateTime.Parse(startDate);
            var EndedDate = DateTime.Parse(endDate);

            var visitors = _unitOfWork.visitorDal.GetAll(x => x.VisitorStartDate.Date >= StartedDate.AddMonths(-2).Date && x.VisitorStartDate.Date <= EndedDate.AddMonths(2).Date && x.VisitorRoomNumber.ToString() == roomNumber)
                .Select(visitor => new
                {
                    visitorId = visitor.VisitorId,
                    visitorStartDate = visitor.VisitorStartDate,
                    visitorEndDate = visitor.VisitorEndDate,
                    visitorRoomNumber = visitor.VisitorRoomNumber
                }).ToList();
            return Ok(visitors);
        }
        [HttpGet]
        public IActionResult CheckOutVisitor(int id)
        {
            string previusUrl = HttpContext.Request.Headers["Referer"];
            var visitor = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == id);


            if (visitor.VisitorState != 5)
            {
                if (visitor.VisitorEndDate.Date <= DateTime.Now.Date.AddDays(1))
                {
                    var checkoutRoom = _unitOfWork.roomDal.GetOne(room => room.RoomNumber == visitor.VisitorRoomNumber);
                    checkoutRoom.RoomIsClean = false;
                    _unitOfWork.roomDal.Update(checkoutRoom);

                    visitor.VisitorState = 5;
                    var visitorHistories = _unitOfWork.visitorHistoryDal.GetAll(x => x.VisitorId == id);
                    if (visitorHistories.All(x => x.VisitorCheckOutTime == null) || visitorHistories == null)
                    {
                        var CheckinTime = new VisitorHistory()
                        {
                            VisitorUpdatedDate = DateTime.Now,
                            VisitorCheckOutTime = "Çıkış Yapıldı",
                            VisitorId = id
                        };
                        _unitOfWork.visitorHistoryDal.Add(CheckinTime);
                    }
                    if (visitor.VisitorFileUrl != null)
                    {
                        FileDeleteProcess(visitor.VisitorFileUrl, _webHostEnvironment.WebRootPath);
                        visitor.VisitorFileUrl = "";
                    }
                    _unitOfWork.visitorDal.Update(visitor);
                    _unitOfWork.Save();
                }
            }
            if (previusUrl == null)
                return RedirectToAction(nameof(DailyVisitors));
            else
                return Redirect(previusUrl);
        }
        [HttpGet]
        public IActionResult CheckInVisitor(int id)
        {
            string previusUrl = HttpContext.Request.Headers["Referer"];
            var visitor = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == id);
            if (DateTime.Now >= visitor.VisitorStartDate.Date.AddHours(0) && DateTime.Now < visitor.VisitorStartDate.Date.AddDays(1).AddHours(12))
            {
                visitor.VisitorState = 2;

                var visitorHistories = _unitOfWork.visitorHistoryDal.GetAll(x => x.VisitorId == id);
                if (visitorHistories.All(x => x.VisitorCheckInTime == null) || visitorHistories == null)
                {
                    var CheckinTime = new VisitorHistory()
                    {
                        VisitorUpdatedDate = DateTime.Now,
                        VisitorCheckInTime = "Giriş Yapıldı",
                        VisitorId = id
                    };
                    _unitOfWork.visitorHistoryDal.Add(CheckinTime);
                }

                visitor.VisitorDontChangeRoom = false;
                _unitOfWork.visitorDal.Update(visitor);
            }
            _unitOfWork.Save();
            if (previusUrl == null)
                return RedirectToAction(nameof(DailyVisitors));
            else
                return Redirect(previusUrl);
        }
        [HttpGet]
        public IActionResult SelectedVisitors(int roomNumber, int visitorId)
        {
            ViewBag.RoomNumber = roomNumber;
            ViewBag.ListOfRooms = new int[] { 101, 102, 103, 104, 201, 202, 203, 204, 301, 302, 303, 304, 401, 402, 403, 404, 501, 502, 503, 601, 602, 603, 701, 702 };
            ViewBag.VisitorId = visitorId;
            string path = HttpContext.Request.Path;
            string query = HttpContext.Request.QueryString.Value;
            ViewBag.currentUrl = path + query;
            var visitors = new List<Visitor>().AsQueryable();
            if (visitorId >= 1)
            {
                visitors = _unitOfWork.visitorDal.GetAllVisitorsWithPaymentAndProperties(x => x.VisitorId == visitorId);
            }
            else if (roomNumber >= 99)
            {
                visitors = _unitOfWork.visitorDal.
              GetAllVisitorsWithPaymentAndProperties(x => x.VisitorEndDate.Date >= DateTime.Now.Date && x.VisitorState != 5 && x.VisitorStartDate.Date <= DateTime.Now.Date && x.VisitorRoomNumber == roomNumber);
            }
            List<VisitorDetailsDTO> visitorDetailsDTOs = new();
            foreach (var visitor in visitors)
            {

                var visitorDetailsDTO = _mapper.Map<VisitorDetailsDTO>(visitor);

                visitorDetailsDTO.VisitorState = visitor.VisitorState == 2 && visitor.VisitorEndDate.Date != DateTime.Now.Date && visitor.VisitorStartDate.Date <= DateTime.Now.Date ? 2 : DateTime.Now.Date >= visitor.VisitorStartDate.Date && DateTime.Now < visitor.VisitorStartDate.Date.AddDays(1).AddHours(12) && visitor.VisitorState != 2 ? 1 : visitor.VisitorEndDate.Date == DateTime.Now.Date ? 0 : visitor.VisitorStartDate.Date < DateTime.Now.Date && visitor.VisitorEndDate.Date > DateTime.Now.Date ? 2 : visitor.VisitorStartDate.Date > DateTime.Now.Date ? 3 : 5;

                if (visitor.VisitorPreviusId != 0)
                {
                    var previusVisitor = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == visitor.VisitorPreviusId);
                    if (previusVisitor == null)
                    {
                        visitorDetailsDTO.VisitorPreviusId = 0;
                    }
                    else
                    {
                        visitorDetailsDTO.VisitorPreviusId = visitor.VisitorPreviusId;
                        visitorDetailsDTO.VisitorPreviusRoomNumber = previusVisitor.VisitorRoomNumber;
                        visitorDetailsDTO.VisitorPreviusPaymentIsDone = PreviousPaymentsCheck(previusVisitor.VisitorId);

                    }
                }
                if (visitor.VisitorNextId != 0)
                {
                    var nextVisitor = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == visitor.VisitorNextId);
                    if (nextVisitor == null)
                    {
                        visitorDetailsDTO.VisitorNextId = 0;
                    }
                    else
                    {
                        visitorDetailsDTO.VisitorNextId = visitor.VisitorNextId;
                        visitorDetailsDTO.VisitorNextRoomNumber = nextVisitor.VisitorRoomNumber;
                    }
                }


                if (visitor.VisitorTotalRoomPrice != null)
                {
                    visitorDetailsDTO.VisitorTotalPrice = visitor.VisitorTotalRoomPrice;
                }
                else
                {
                    visitorDetailsDTO.VisitorTotalPrice = visitor.VisitorRoomPrice == null ? 0 : (visitor.VisitorEndDate.Date - visitor.VisitorStartDate.Date).Days * visitor.VisitorRoomPrice.Value;
                }

                visitorDetailsDTO.VisitorTotalVisitDay = (visitor.VisitorEndDate.Date - visitor.VisitorStartDate.Date).Days;

                visitorDetailsDTO.VisitorLeftDays = (visitor.VisitorEndDate.Date - DateTime.Now.Date).Days <= 0 ? 0 : (visitor.VisitorEndDate.Date - DateTime.Now.Date).Days >= visitorDetailsDTO.VisitorTotalVisitDay ? (visitor.VisitorEndDate.Date - visitor.VisitorStartDate.Date).Days : (visitor.VisitorEndDate.Date - DateTime.Now.Date).Days;

                foreach (var payment in visitor.Payments)
                {
                    VisitorDetailsPaymentDTO visitorDetailsPaymentDTO = new();
                    visitorDetailsPaymentDTO.VisitorPaymentId = payment.PaymentId;
                    visitorDetailsPaymentDTO.VisitorPaymentDate = payment.VisitorPaymentDate;
                    visitorDetailsPaymentDTO.VisitorPaymentCurreny = payment.VisitorPaymentCurreny;
                    visitorDetailsPaymentDTO.VisitorPayment = payment.VisitorPayment;
                    visitorDetailsPaymentDTO.VisitorPaymentType = payment.VisitorPaymentType;
                    visitorDetailsDTO.Payments.Add(visitorDetailsPaymentDTO);
                }
                visitorDetailsDTO.Payments = visitorDetailsDTO.Payments.OrderBy(x => x.VisitorPaymentDate).ToList();
                visitorDetailsDTOs.Add(visitorDetailsDTO);

            }
            return View(visitorDetailsDTOs.OrderBy(x => x.VisitorEndDate).ToList());
        }

        public bool PreviousPaymentsCheck(int? previousId)
        {
            var previousVisitor = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == previousId);

            if (previousVisitor.VisitorPaymentIsDone)
            {
                var previousVisitorCheck = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == previousVisitor.VisitorPreviusId);
                if (previousVisitorCheck == null)
                    return true;
                return PreviousPaymentsCheck(previousVisitor.VisitorPreviusId);
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public IActionResult ActiveStateChange([FromBody] ActivateChangeDTO activateChangeDTO)
        {
            var name = _unitOfWork.visitorPropertyDal.GetOne(x => x.VisitorPropertiyId == activateChangeDTO.PropertyId);
            name.VisitorActive = activateChangeDTO.StateOfVisitor;
            _unitOfWork.visitorPropertyDal.Update(name);
            _unitOfWork.Save();
            return Ok();
        }
        [HttpGet]
        public IActionResult ActiveStateChangeGet()
        {

            return Ok();
        }
        [Authorize(Policy = "ADMN")]
        [HttpGet]
        public IActionResult GuestDates()
        {
            var latestVisitors = _unitOfWork.visitorDal.GetAllVisitorsWithProperties(null).OrderByDescending(x => x.VisitorAddedDate).Take(40);
            return View(latestVisitors.ToList());
        }
        [HttpGet]
        public IActionResult VisitorSearch()
        {
            return View();
        }
        [HttpPost]
        public IActionResult VisitorSearch([FromBody] VisitorSearchOptions visitorSearchOptions)
        {
            var visitors = _unitOfWork.visitorPropertyDal.GetAllPropertiesWithVisitors(x => x.VisitorName.Contains(visitorSearchOptions.VisitorName) && x.VisitorSurName.Contains(visitorSearchOptions.VisitorSurName));

            int RoomCheckOutCount = 0;

            if (visitorSearchOptions.VisitorRoomNumber is >= 101 and <= 702)
                visitors = visitors.Where(x => x.Visitor.VisitorRoomNumber == visitorSearchOptions.VisitorRoomNumber);
            if (visitorSearchOptions.VisitorStartDate != null)
            {
                visitors = visitors.Where(x => x.Visitor.VisitorStartDate == visitorSearchOptions.VisitorStartDate);
            }
            if (visitorSearchOptions.VisitorEndDate != null)
            {
                visitors = visitors.Where(x => x.Visitor.VisitorEndDate == visitorSearchOptions.VisitorEndDate);
                if (visitorSearchOptions.VisitorStartDate == null)
                {
                    RoomCheckOutCount = visitors.Select(x => x.Visitor.VisitorRoomNumber).Distinct().Count();
                }
            }
            List<int> rooms = new List<int>();
            List<VisitorSearchDTO> VisitorSearchDTOs = new();
            foreach (var visitor in visitors)
            {
                var VisitorSearchDTO = new VisitorSearchDTO();
                VisitorSearchDTO.VisitorId = visitor.VisitorId;
                VisitorSearchDTO.VisitorName = visitor.VisitorName;
                if (!rooms.Contains(visitor.Visitor.VisitorRoomNumber))
                    VisitorSearchDTO.VisitorCount = visitor.Visitor.VisitorCount.Value;
                rooms.Add(visitor.Visitor.VisitorRoomNumber);
                VisitorSearchDTO.VisitorSurName = visitor.VisitorSurName;
                VisitorSearchDTO.VisitorStartDate = visitor.Visitor.VisitorStartDate.ToShortDateString();
                VisitorSearchDTO.VisitorEndDate = visitor.Visitor.VisitorEndDate.ToShortDateString();
                VisitorSearchDTO.VisitorRoomNumber = visitor.Visitor.VisitorRoomNumber;
                VisitorSearchDTO.VisitorPaymentCurrency = visitor.Visitor.VisitorPaymentCurrency == null ? " -- " : visitor.Visitor.VisitorPaymentCurrency.Value.ToString();
                VisitorSearchDTO.VisitorRoomPrice = visitor.Visitor.VisitorRoomPrice == null ? " -- " : visitor.Visitor.VisitorRoomPrice.Value.ToString();
                VisitorSearchDTO.VisitorRezervation = visitor.Visitor.VisitorRezervation == null ? " -- " : visitor.Visitor.VisitorRezervation;
                if (!visitor.VisitorActive)
                    VisitorSearchDTO.VisitorCount -= 1;
                VisitorSearchDTOs.Add(VisitorSearchDTO);
            }
            return Ok(JsonSerializer.Serialize(VisitorSearchDTOs));
        }

        [Authorize(Policy = "ADMN")]
        [HttpGet]
        public IActionResult MonthlyIncome(int monthNumber)
        {
            var Payments = _unitOfWork.paymentDal.GetAll(x => x.VisitorPaymentDate.Value.Month == DateTime.Now.AddMonths(monthNumber).Month && x.VisitorPaymentDate.Value.Date.Year == DateTime.Now.AddMonths(monthNumber).Year);
            MonthlyIncomeDTO MonthlyIncome = new MonthlyIncomeDTO();
            MonthlyIncome.Euro = 0;
            MonthlyIncome.Dollar = 0;
            MonthlyIncome.TurkishLiras = 0;
            MonthlyIncome.MonthNumber = monthNumber;

            foreach (var Payment in Payments)
            {
                if (Payment.VisitorPaymentCurreny.ToString() == "TL")
                    MonthlyIncome.TurkishLiras += Payment.VisitorPayment;
                else if (Payment.VisitorPaymentCurreny.ToString() == "EURO")
                    MonthlyIncome.Euro += Payment.VisitorPayment;
                else if (Payment.VisitorPaymentCurreny.ToString() == "USD")
                    MonthlyIncome.Dollar += Payment.VisitorPayment;
            }

            return View(MonthlyIncome);
        }
        [HttpGet]
        public IActionResult AllRooms()
        {
            var rooms = _unitOfWork.roomDal.GetAll().Select(x => new { isClean = x.RoomIsClean, roomNumber = x.RoomNumber });
            return Ok(rooms);
        }
        [HttpPost]
        public IActionResult IsRoomClean([FromBody] IsRoomCleanDTO isRoomCleanDTO)
        {
            var cleaningStatusChange = _unitOfWork.roomDal.GetOne(x => x.RoomNumber == isRoomCleanDTO.RoomNumber);
            cleaningStatusChange.RoomIsClean = isRoomCleanDTO.RoomIsClean;
            _unitOfWork.roomDal.Update(cleaningStatusChange);
            _unitOfWork.Save();
            return Ok();
        }
        [HttpGet]
        public IActionResult EmployeeNotes()
        {
            var reports = _unitOfWork.ReportDal.GetAll(x => x.ReportDate.Date >= DateTime.Now.AddDays(-4).Date).ToList();
            ReportsDTO reportsDTO = new();
            reportsDTO.Report = new();
            reportsDTO.ReportList = reports;
            return View(reportsDTO);
        }
        [HttpPost]
        public IActionResult EmployeeNotes(Report report)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ReportDal.Add(report);
                _unitOfWork.Save();
                return RedirectToAction(nameof(EmployeeNotes));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Mesaj ve gönderici ismi girmelisiniz");
                return View(new ReportsDTO());
            }
        }
        [HttpGet]
        public IActionResult NoteDelete(int id)
        {
            _unitOfWork.ReportDal.Delete(_unitOfWork.ReportDal.GetOne(x => x.ReportId == id));
            _unitOfWork.Save();
            return RedirectToAction(nameof(EmployeeNotes));
        }
        [HttpGet]
        public IActionResult AllCheckInVisitors()
        {
            var visitors = _unitOfWork.visitorDal.GetAllVisitorsWithProperties(x => (x.VisitorStartDate.Date == DateTime.Now.Date || x.VisitorStartDate.Date == DateTime.Now.AddDays(-1).Date) && x.VisitorState != 5 && x.VisitorState != 2 && x.VisitorEndDate.Date > DateTime.Now.Date);

            List<AllVisitorDTO> getAllCustomVisitorDTOs = new List<AllVisitorDTO>();
            foreach (var visitor in visitors)
            {
                if (visitor.VisitorStartDate.Date.AddHours(DateTime.Now.Hour) < DateTime.Now.Date.AddDays(1).AddHours(12))
                {
                    var customVisitor = _mapper.Map<AllVisitorDTO>(visitor);
                    customVisitor.VisitorName = visitor.VisitorProperties[0].VisitorName;
                    customVisitor.VisitorSurName = visitor.VisitorProperties[0].VisitorSurName;
                    customVisitor.VisitorRezervation = visitor.VisitorRezervation;
                    customVisitor.VisitorDescription = visitor.VisitorDescription;
                    customVisitor.VisitorTotalVisitDay = (visitor.VisitorEndDate.Date - visitor.VisitorStartDate.Date).Days;
                    customVisitor.VisitorTotalPrice = customVisitor.VisitorTotalVisitDay * customVisitor.VisitorRoomPrice;
                    customVisitor.VisitorState = 1;
                    if (DateTime.Now.Date >= visitor.VisitorStartDate.Date && DateTime.Now < visitor.VisitorStartDate.Date.AddDays(1).AddHours(12))
                    {
                        getAllCustomVisitorDTOs.Add(customVisitor);
                    }
                }
            }

            return View(getAllCustomVisitorDTOs.OrderBy(x => x.VisitorRoomNumber).ToList());
        }
        [HttpGet]
        public IActionResult AllCheckOutVisitors()
        {
            var visitors = _unitOfWork.visitorDal.GetAllVisitorsWithProperties(x => x.VisitorEndDate.Date <= DateTime.Now.Date && x.VisitorState != 5);

            List<AllVisitorDTO> getAllCustomVisitorDTOs = new List<AllVisitorDTO>();
            foreach (var visitor in visitors)
            {
                var customVisitor = _mapper.Map<AllVisitorDTO>(visitor);
                customVisitor.VisitorName = visitor.VisitorProperties[0].VisitorName;
                customVisitor.VisitorSurName = visitor.VisitorProperties[0].VisitorSurName;
                customVisitor.VisitorRezervation = visitor.VisitorRezervation;
                customVisitor.VisitorTotalVisitDay = (visitor.VisitorEndDate.Date - visitor.VisitorStartDate.Date).Days;
                customVisitor.VisitorState = 0;
                customVisitor.VisitorTotalPrice = visitor.VisitorRoomPrice * customVisitor.VisitorTotalVisitDay;
                getAllCustomVisitorDTOs.Add(customVisitor);
            }

            return View(getAllCustomVisitorDTOs.OrderBy(x => x.VisitorRoomNumber).ToList());
        }
        [HttpPost]
        public IActionResult PaymentIsDone([FromBody] PaymentIsDoneDTO paymentIsDoneDTO)
        {
            var visitor = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == paymentIsDoneDTO.VisitorId);
            if (visitor != null)
            {
                visitor.VisitorPaymentIsDone = paymentIsDoneDTO.paymentStatus;
                _unitOfWork.visitorDal.Update(visitor);
                _unitOfWork.Save();
            }
            return Ok();
        }
        [HttpGet]
        public IActionResult DeletePayment(int paymentId, string previusUrl)
        {
            var deletePayment = _unitOfWork.paymentDal.GetOne(x => x.PaymentId == paymentId);
            if (deletePayment != null)
            {
                _unitOfWork.paymentDal.Delete(deletePayment);
                _unitOfWork.Save();
            }
            return Redirect(previusUrl);
        }
        public IActionResult UnpaidVisitors()
        {
            var visitors = _unitOfWork.visitorDal.GetAllVisitorsWithProperties(x => x.VisitorPaymentIsDone == false && x.VisitorState == 5);
            List<AllVisitorDTO> getAllCustomVisitorDTOs = new List<AllVisitorDTO>();
            foreach (var visitor in visitors)
            {
                foreach (var names in visitor.VisitorProperties)
                {

                    var customVisitor = _mapper.Map<AllVisitorDTO>(visitor);
                    customVisitor.VisitorName = names.VisitorName;
                    customVisitor.VisitorSurName = names.VisitorSurName;
                    customVisitor.VisitorState = visitor.VisitorState == 2 && visitor.VisitorEndDate.Date != DateTime.Now.Date && visitor.VisitorStartDate.Date <= DateTime.Now.Date ? 2 : DateTime.Now.Date >= visitor.VisitorStartDate.Date && DateTime.Now < visitor.VisitorStartDate.Date.AddDays(1).AddHours(12) && visitor.VisitorState != 2 ? 1 : visitor.VisitorEndDate.Date == DateTime.Now.Date ? 0 : visitor.VisitorStartDate.Date < DateTime.Now.Date && visitor.VisitorEndDate.Date > DateTime.Now.Date ? 2 : visitor.VisitorStartDate.Date > DateTime.Now.Date ? 3 : 5;
                    getAllCustomVisitorDTOs.Add(customVisitor);
                }

            }
            return View(getAllCustomVisitorDTOs.OrderBy(x => x.VisitorStartDate).ThenBy(x => x.VisitorRoomNumber).ToList());
        }
    }
}
