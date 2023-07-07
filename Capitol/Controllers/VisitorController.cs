using AutoMapper;
using DataAccessLayer.Abstract;
using EntityLayer.DTOs;
using EntityLayer.Entities;
using EntityLayer.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Capitol.Controllers
{
    public class VisitorController : Controller
    {
        IUnitOfWorkDal _unitOfWork;
        IMapper _mapper;
        public VisitorController(IUnitOfWorkDal unitOfWorkDal, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWorkDal;
        }
        [HttpGet]
        public IActionResult AddVisitor()
        {
            ViewBag.previusUrl = HttpContext.Request.Headers["Referer"];
            string isFromRoom = HttpContext.Request.Headers["Referer"];
            if (isFromRoom.Contains("roomNumber"))
            {
                isFromRoom = isFromRoom.Split("=")[1];
                ViewBag.Blokaj = isFromRoom;
            }
            return View();
        }
        [HttpPost]
        public IActionResult AddVisitor(AddVisitorDTO addVisitorDTO, bool DoPayment, string? previusUrl, List<string> names, List<string> surnames, List<int> roomnumbers)
        {
            if (ModelState.IsValid)
            {
                string pattern = "\\s+";
                Visitor visitorPayment = null;
                for (int i = 0; i < names.Count(); i++)
                {
                    Visitor visitor = _mapper.Map<Visitor>(addVisitorDTO);
                    if (i == 0)
                        visitorPayment = visitor;
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
                    visitor.VisitorName = names[i];
                    visitor.VisitorSurName = surnames[i];
                    visitor.VisitorRoomNumber = roomnumbers[i];
                    _unitOfWork.visitorDal.Add(visitor);
                }


                _unitOfWork.Save();
                if (DoPayment == true)
                {
                    if (visitorPayment.VisitorRoomPrice == null || visitorPayment.VisitorPaymentCurrency == null)
                    {
                        ModelState.AddModelError(string.Empty, "Oda fiyatı ve para birimi seçilmelidir");
                        return View(addVisitorDTO);
                    }
                    if (_unitOfWork.paymentDal.GetOne(x => x.VisitorId == visitorPayment.VisitorId) == null)
                    {
                        _unitOfWork.paymentDal.Add(new Payment() { VisitorId = visitorPayment.VisitorId });
                        _unitOfWork.Save();
                    }
                    return RedirectToAction(nameof(UpdateVisitorPayment), new { VisitorId = visitorPayment.VisitorId });
                }
            }
            else
            {
                return View(addVisitorDTO);
            }
            if (previusUrl == null)
                return RedirectToAction(nameof(DailyVisitors));
            else
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
                return RedirectToAction(nameof(UpdateVisitorPayment), new { VisitorId = deletePay.VisitorId });
            }
            else
            {
                throw new Exception("Böyle bir ödeme mevcut değil");
            }
        }
        [HttpGet]
        public IActionResult UpdateVisitorPayment([FromQuery] int VisitorId)
        {
            var payments = _unitOfWork.paymentDal.GetAll(x => x.VisitorId == VisitorId).ToList();
            if (payments.Count() <= 0)
            {
                return RedirectToAction(nameof(DailyVisitors));
            }
            var paymentDTOs = new List<PaymentDTO>();
            foreach (var payment in payments)
            {
                paymentDTOs.Add(_mapper.Map<PaymentDTO>(payment));
            }
            return View(paymentDTOs);
        }
        [HttpGet]
        public IActionResult AddNewPaymentField(int VisitorId)
        {
            if (_unitOfWork.visitorDal.GetOne(x => x.VisitorId == VisitorId) != null)
            {
                _unitOfWork.paymentDal.Add(new Payment() { VisitorId = VisitorId });
                _unitOfWork.Save();
                return RedirectToAction(nameof(UpdateVisitorPayment), new { VisitorId = VisitorId });
            }
            else
                throw new Exception("Böyle bir ziyaretçi yok");
        }
        [HttpPost]
        public IActionResult UpdateVisitorPayment(List<PaymentDTO> paymentDTOs, int? visitorId)
        {
            foreach (var paymentDTO in paymentDTOs)
            {
                var payment = _mapper.Map<Payment>(paymentDTO);
                _unitOfWork.paymentDal.Update(payment);
                paymentDTO.Visitor = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == payment.VisitorId);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(SelectedVisitors), new { visitorId = visitorId });
        }
        [HttpGet]
        public IActionResult DeleteVisitor(int id)
        {
            string previusUrl = HttpContext.Request.Headers["Referer"];
            var deleteVisitor = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == id);
            if (deleteVisitor != null)
            {
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
                return Redirect(previusUrl);
        }
        [HttpGet]
        public IActionResult UpdateVisitor(int id)
        {
            ViewBag.previusUrl = HttpContext.Request.Headers["Referer"];
            var visitor = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == id);
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
        public IActionResult UpdateVisitor(UpdateVisitorDTO updateVisitorDTO, bool DoPayment, string? previusUrl)
        {
            if (ModelState.IsValid)
            {
                string pattern = "\\s+";
                updateVisitorDTO.VisitorName = Regex.Replace(updateVisitorDTO.VisitorName, pattern, " ");
                updateVisitorDTO.VisitorName = updateVisitorDTO.VisitorName.Trim(' ');
                var visitorNameLength = updateVisitorDTO.VisitorName.Split(" ").Length;
                string name = "";
                if (visitorNameLength > 1)
                {
                    for (int i = 0; i < visitorNameLength; i++)
                    {
                        name += updateVisitorDTO.VisitorName.Split(" ")[i].Substring(0, 1).ToUpper() + updateVisitorDTO.VisitorName.Split(" ")[i].Substring(1, updateVisitorDTO.VisitorName.Split(" ")[i].Length - 1).ToLower() + " ";
                    }
                }
                else
                {
                    name = updateVisitorDTO.VisitorName.Substring(0, 1).ToUpper() + updateVisitorDTO.VisitorName.Substring(1, updateVisitorDTO.VisitorName.Length - 1).ToLower();
                }
                updateVisitorDTO.VisitorName = name.Trim(' ');
                updateVisitorDTO.VisitorSurName = updateVisitorDTO.VisitorSurName != null ? updateVisitorDTO.VisitorSurName.ToUpper() : "";

                var visitor = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == updateVisitorDTO.VisitorId);
                if (DoPayment == true)
                {
                    if (updateVisitorDTO.VisitorRoomPrice == null || updateVisitorDTO.VisitorPaymentCurrency == null)
                    {
                        ModelState.AddModelError(string.Empty, "Oda fiyatı ve para birimi seçilmelidir");
                        return View(updateVisitorDTO);
                    }
                    if (_unitOfWork.paymentDal.GetOne(x => x.VisitorId == visitor.VisitorId) == null)
                    {
                        _unitOfWork.paymentDal.Add(new Payment() { VisitorId = visitor.VisitorId });
                    }

                    visitor = _mapper.Map<Visitor>(updateVisitorDTO);
                    _unitOfWork.visitorDal.Update(visitor);
                    _unitOfWork.Save();
                    return RedirectToAction(nameof(UpdateVisitorPayment), new { VisitorId = visitor.VisitorId, previusUrl = previusUrl });
                }
                else
                {
                    if (visitor != null)
                    {
                        visitor = _mapper.Map<Visitor>(updateVisitorDTO);
                        _unitOfWork.visitorDal.Update(visitor);
                        _unitOfWork.Save();
                    }
                }
                if (previusUrl == null)
                    return RedirectToAction(nameof(DailyVisitors));
                else
                    return Redirect(previusUrl);
            }
            else
            {
                return View(updateVisitorDTO);
            }
        }
        [HttpGet]
        public IActionResult MonthlyView(int monthNumber)
        {

            ViewBag.totalCount = 0;
            var visitors = _unitOfWork.visitorDal.GetAll(x => x.VisitorStartDate.Date >= DateTime.Now.AddMonths(monthNumber - 1).Date && x.VisitorStartDate.Date <= DateTime.Now.AddMonths(monthNumber + 1).Date);



            var monthlyVisitorDTO = new MonthlyVisitorDTO();
            monthlyVisitorDTO.tableValues = new (bool CellState, string Name, int Identity, int count)[25, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.AddMonths(monthNumber).Month) + 1];
            monthlyVisitorDTO.monthNumber = monthNumber;

            int balancer = DateTime.Now.AddMonths(monthNumber).Month == 3 ? 3 : 1;

            foreach (var visitor in visitors)
            {
                if (monthlyVisitorDTO.roomNumbers.Contains(visitor.VisitorRoomNumber))
                {
                    for (int i = 0; i < (visitor.VisitorEndDate - visitor.VisitorStartDate).TotalDays; i++)
                    {
                        if (visitor.VisitorStartDate.Day + i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.AddMonths(monthNumber).Month) &&
                            DateTime.Now.AddMonths(monthNumber).Month == visitor.VisitorStartDate.AddDays(i - (i > 0 ? balancer : i)).Month)
                        {
                            if (monthlyVisitorDTO.tableValues[Array.IndexOf(monthlyVisitorDTO.roomNumbers, visitor.VisitorRoomNumber), visitor.VisitorStartDate.Day + i].CellState != true)
                            {
                                monthlyVisitorDTO.tableValues[24, visitor.VisitorStartDate.Day + i].count += 1;
                                ViewBag.totalCount += 1;
                            }
                            if (visitor.VisitorState != 5)
                            {
                                if (i == 0)
                                {
                                    monthlyVisitorDTO.tableValues[Array.IndexOf(monthlyVisitorDTO.roomNumbers, visitor.VisitorRoomNumber), visitor.VisitorStartDate.Day + i].Name = visitor.VisitorName.Split(" ").Length > 1 ? visitor.VisitorName.Split(" ")[0] : visitor.VisitorName;
                                    monthlyVisitorDTO.tableValues[Array.IndexOf(monthlyVisitorDTO.roomNumbers, visitor.VisitorRoomNumber), visitor.VisitorStartDate.Day + i].Identity = visitor.VisitorId;
                                }
                                monthlyVisitorDTO.tableValues[Array.IndexOf(monthlyVisitorDTO.roomNumbers, visitor.VisitorRoomNumber), visitor.VisitorStartDate.Day + i].CellState = true;
                            }

                        }
                        else if (visitor.VisitorStartDate.Day + i + (balancer == 3 ? 1 : 0) >= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.AddMonths(monthNumber + 1).Month) &&
                            DateTime.Now.AddMonths(monthNumber).Month == visitor.VisitorStartDate.AddDays(i).Month)
                        {
                            if (monthlyVisitorDTO.tableValues[Array.IndexOf(monthlyVisitorDTO.roomNumbers, visitor.VisitorRoomNumber), visitor.VisitorStartDate.Day + i - DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.AddMonths(monthNumber - 1).Month)].CellState != true)
                            {
                                monthlyVisitorDTO.tableValues[24, visitor.VisitorStartDate.Day + i - DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.AddMonths(monthNumber - 1).Month)].count += 1;
                                ViewBag.totalCount += 1;
                            }
                            if (visitor.VisitorState != 5)
                            {
                                monthlyVisitorDTO.tableValues[Array.IndexOf(monthlyVisitorDTO.roomNumbers, visitor.VisitorRoomNumber), visitor.VisitorStartDate.Day + i - DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.AddMonths(monthNumber - 1).Month)].CellState = true;
                            }
                        }
                    }
                }
            }
            return View(monthlyVisitorDTO);
        }
        [HttpGet]
        public IActionResult DailyVisitors()
        {

            var visitors = _unitOfWork.visitorDal.
                GetAll(x => x.VisitorEndDate.Date >= DateTime.Now.Date && x.VisitorStartDate.Date <= DateTime.Now.Date && x.VisitorState != 5);

            DailyVisitorDTOs dailyVisitorDTOs = new();


            foreach (var visitor in visitors)
            {
                var customVisitor = new DailyVisitorDTO();
                customVisitor.VisitorRoomNumber = visitor.VisitorRoomNumber;
                customVisitor.VisitorState = visitor.VisitorState == 2 && visitor.VisitorEndDate.Date != DateTime.Now.Date ? "green" : DateTime.Now.Date >= visitor.VisitorStartDate.Date && DateTime.Now < visitor.VisitorStartDate.Date.AddDays(1).AddHours(12) ? "blue" : visitor.VisitorEndDate.Date == DateTime.Now.Date ? "red" : "green";
                customVisitor.IsPaymentDone = visitor.VisitorPaymentIsDone;
                dailyVisitorDTOs.listOfVisitors.Add(customVisitor);
            }
            dailyVisitorDTOs.listOfVisitors.Sort((c1, c2) => c1.VisitorState.CompareTo(c2.VisitorState));
            dailyVisitorDTOs.CheckInCount = dailyVisitorDTOs.listOfVisitors.Count(x => x.VisitorState == "blue");
            dailyVisitorDTOs.StayInCount = dailyVisitorDTOs.listOfVisitors.Count(x => x.VisitorState == "green") + dailyVisitorDTOs.listOfVisitors.Count(x => x.VisitorState == "red");
            dailyVisitorDTOs.CheckOutCount = dailyVisitorDTOs.listOfVisitors.Count(x => x.VisitorState == "red");

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
                        dailyVisitorDTO.resultOfP += 1;
                    }
                }
            }
            return View(dailyVisitorDTOs);
        }
        [HttpGet]
        public IActionResult AllVisitors()
        {
            var visitors = _unitOfWork.visitorDal.GetAll(x => x.VisitorEndDate.Date >= DateTime.Now.AddDays(-3).Date && x.VisitorState != 5);
            List<AllVisitorDTO> getAllCustomVisitorDTOs = new List<AllVisitorDTO>();
            foreach (var visitor in visitors)
            {
                var customVisitor = _mapper.Map<AllVisitorDTO>(visitor);
                customVisitor.VisitorState = visitor.VisitorState == 2 && visitor.VisitorEndDate.Date != DateTime.Now.Date && visitor.VisitorStartDate.Date <= DateTime.Now.Date ? 2 : DateTime.Now.Date >= visitor.VisitorStartDate.Date && DateTime.Now < visitor.VisitorStartDate.Date.AddDays(1).AddHours(12) ? 1 : visitor.VisitorEndDate.Date == DateTime.Now.Date ? 0 : visitor.VisitorStartDate.Date < DateTime.Now.Date && visitor.VisitorEndDate.Date > DateTime.Now.Date ? 2 : visitor.VisitorStartDate.Date > DateTime.Now.Date ? 3 : 5;
                getAllCustomVisitorDTOs.Add(customVisitor);
            }

            return View(getAllCustomVisitorDTOs.OrderBy(x => x.VisitorStartDate).ThenBy(x => x.VisitorRoomNumber).ToList());
        }
        [HttpGet]
        public IActionResult CheckForCrashDate()
        {
            var visitors = _unitOfWork.visitorDal.GetAll(x => x.VisitorEndDate.Date >= DateTime.Now.AddDays(-3).Date && x.VisitorState != 5).Select(visitor => new
            {
                visitorId = visitor.VisitorId,
                visitorStartDate = visitor.VisitorStartDate,
                visitorEndDate = visitor.VisitorEndDate,
                visitorRoomNumber = visitor.VisitorRoomNumber,
                visitorCount = visitor.VisitorCount
            }).ToList();
            return Ok(visitors);
        }
        [HttpGet]
        public IActionResult CheckOutVisitor(int id)
        {
            string previusUrl = HttpContext.Request.Headers["Referer"];
            var visitor = _unitOfWork.visitorDal.GetOne(x => x.VisitorId == id);


            if (visitor.VisitorEndDate.Date <= DateTime.Now.Date && _unitOfWork.visitorDal.GetAll(x => x.VisitorRoomNumber == visitor.VisitorRoomNumber).Where(x => x.VisitorStartDate.Date < DateTime.Now.Date && x.VisitorEndDate >= DateTime.Now.Date && x.VisitorState != 5).Count() < 2)
            {
                var checkoutRoom = _unitOfWork.roomDal.GetOne(room => room.RoomNumber == visitor.VisitorRoomNumber);
                checkoutRoom.RoomIsClean = false;
                _unitOfWork.roomDal.Update(checkoutRoom);
            }
            if (visitor.VisitorEndDate.Date <= DateTime.Now.Date.AddDays(1))
            {
                visitor.VisitorState = 5;
                _unitOfWork.visitorDal.Update(visitor);
                _unitOfWork.Save();
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
            ViewBag.VisitorId = visitorId;
            var visitors = new List<Visitor>().AsQueryable();
            if (visitorId >= 1)
            {
                visitors = _unitOfWork.visitorDal.GetAllVisitorsWithPayment(x => x.VisitorId == visitorId);
            }
            else if (roomNumber >= 99)
            {
                visitors = _unitOfWork.visitorDal.
              GetAllVisitorsWithPayment(x => x.VisitorEndDate.Date >= DateTime.Now.Date && x.VisitorState != 5 && x.VisitorStartDate.Date <= DateTime.Now.Date && x.VisitorRoomNumber == roomNumber);
            }
            List<VisitorDetailsDTO> visitorDetailsDTOs = new();
            foreach (var visitor in visitors)
            {
                VisitorDetailsDTO visitorDetailsDTO = new VisitorDetailsDTO();
                visitorDetailsDTO.VisitorId = visitor.VisitorId;
                visitorDetailsDTO.VisitorRezervation = visitor.VisitorRezervation;
                visitorDetailsDTO.VisitorPaymentIsDone = visitor.VisitorPaymentIsDone;
                visitorDetailsDTO.VisitorRoomPrice = visitor.VisitorRoomPrice;
                visitorDetailsDTO.VisitorPhoneNumber = visitor.VisitorPhoneNumber;
                visitorDetailsDTO.VisitorName = visitor.VisitorName;
                visitorDetailsDTO.VisitorSurName = visitor.VisitorSurName;
                visitorDetailsDTO.VisitorDescription = visitor.VisitorDescription;

                visitorDetailsDTO.VisitorState = visitor.VisitorState == 2 && visitor.VisitorEndDate.Date != DateTime.Now.Date ? 2 : DateTime.Now.Date == visitor.VisitorStartDate.Date || DateTime.Now < visitor.VisitorStartDate.Date.AddDays(1).AddHours(12) ? 1 : visitor.VisitorEndDate.Date == DateTime.Now.Date ? 0 : visitor.VisitorStartDate.Date < DateTime.Now.Date && visitor.VisitorEndDate.Date > DateTime.Now.Date ? 2 : visitor.VisitorStartDate.Date > DateTime.Now.Date ? 3 : 5;


                visitorDetailsDTO.VisitorTotalPrice = visitor.VisitorRoomPrice == null ? 0 : (visitor.VisitorEndDate.Date - visitor.VisitorStartDate.Date).Days * visitor.VisitorRoomPrice.Value;
                visitorDetailsDTO.VisitorPaymentCurrency = visitor.VisitorPaymentCurrency;
                visitorDetailsDTO.VisitorCount = visitor.VisitorCount;
                visitorDetailsDTO.VisitorRoomNumber = visitor.VisitorRoomNumber;
                visitorDetailsDTO.VisitorStartDate = visitor.VisitorStartDate;
                visitorDetailsDTO.VisitorEndDate = visitor.VisitorEndDate;
                visitorDetailsDTO.VisitorTotalVisitDay = (visitor.VisitorEndDate.Date - visitor.VisitorStartDate.Date).Days;
                visitorDetailsDTO.VisitorLeftDays = (visitor.VisitorEndDate.Date - DateTime.Now.Date).Days <= 0 ? 0 : (visitor.VisitorEndDate.Date - DateTime.Now.Date).Days >= visitorDetailsDTO.VisitorTotalVisitDay ? (visitor.VisitorEndDate.Date - visitor.VisitorStartDate.Date).Days : (visitor.VisitorEndDate.Date - DateTime.Now.Date).Days;
                foreach (var payment in visitor.Payments)
                {
                    VisitorDetailsPaymentDTO visitorDetailsPaymentDTO = new();
                    visitorDetailsPaymentDTO.VisitorPaymentDate = payment.VisitorPaymentDate;
                    visitorDetailsPaymentDTO.VisitorPaymentCurreny = payment.VisitorPaymentCurreny;
                    visitorDetailsPaymentDTO.VisitorPayment = payment.VisitorPayment;
                    visitorDetailsPaymentDTO.VisitorPaymentType = payment.VisitorPaymentType;
                    visitorDetailsDTO.Payments.Add(visitorDetailsPaymentDTO);
                }
                visitorDetailsDTOs.Add(visitorDetailsDTO);

            }
            return View(visitorDetailsDTOs);
        }
        [HttpGet]
        public IActionResult Questions()
        {
            return View();
        }
        [HttpGet]
        public IActionResult VisitorSearch()
        {
            return View();
        }
        [HttpPost]
        public IActionResult VisitorSearch([FromBody] VisitorSearchOptions visitorSearchOptions)
        {
            var visitors = _unitOfWork.visitorDal.GetAll(x => x.VisitorName.Contains(visitorSearchOptions.VisitorName) && x.VisitorSurName.Contains(visitorSearchOptions.VisitorSurName));
            List<VisitorSearchDTO> VisitorSearchDTOs = new();
            foreach (var visitor in visitors)
            {
                var VisitorSearchDTO = new VisitorSearchDTO();
                VisitorSearchDTO.VisitorId = visitor.VisitorId;
                VisitorSearchDTO.VisitorName = visitor.VisitorName;
                VisitorSearchDTO.VisitorSurName = visitor.VisitorSurName;
                VisitorSearchDTO.VisitorStartDate = visitor.VisitorStartDate.ToShortDateString();
                VisitorSearchDTO.VisitorEndDate = visitor.VisitorEndDate.ToShortDateString();
                VisitorSearchDTO.VisitorRoomNumber = visitor.VisitorRoomNumber;
                VisitorSearchDTO.VisitorPaymentCurrency = visitor.VisitorPaymentCurrency == null ? " -- " : visitor.VisitorPaymentCurrency.Value.ToString();
                VisitorSearchDTO.VisitorRoomPrice = visitor.VisitorRoomPrice == null ? " -- " : visitor.VisitorRoomPrice.Value.ToString();
                VisitorSearchDTO.VisitorDescription = visitor.VisitorDescription == null ? " -- " : visitor.VisitorDescription;
                VisitorSearchDTOs.Add(VisitorSearchDTO);

            }
            return Ok(JsonSerializer.Serialize(VisitorSearchDTOs.OrderBy(x => x.VisitorSurName)));
        }
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
            var visitors = _unitOfWork.visitorDal.GetAll(x => (x.VisitorStartDate.Date == DateTime.Now.Date || x.VisitorStartDate.Date == DateTime.Now.AddDays(-1).Date) && x.VisitorState != 5 && x.VisitorState != 2 && x.VisitorEndDate.Date > DateTime.Now.Date);

            List<AllVisitorDTO> getAllCustomVisitorDTOs = new List<AllVisitorDTO>();
            foreach (var visitor in visitors)
            {
                if (visitor.VisitorStartDate.Date.AddHours(DateTime.Now.Hour) < DateTime.Now.Date.AddDays(1).AddHours(12))
                {
                    var customVisitor = _mapper.Map<AllVisitorDTO>(visitor);
                    customVisitor.VisitorTotalVisitDay = (visitor.VisitorEndDate.Date - visitor.VisitorStartDate.Date).Days;
                    customVisitor.VisitorLeftDays = (visitor.VisitorEndDate.Date - DateTime.Now.Date).Days <= 0 ? 0 : (visitor.VisitorEndDate.Date - DateTime.Now.Date).Days >= customVisitor.VisitorTotalVisitDay ? (visitor.VisitorEndDate.Date - visitor.VisitorStartDate.Date).Days : (visitor.VisitorEndDate.Date - DateTime.Now.Date).Days;
                    customVisitor.VisitorTotalPrice = customVisitor.VisitorTotalVisitDay * customVisitor.VisitorRoomPrice;
                    customVisitor.VisitorState = 1;
                    if (DateTime.Now.Date >= visitor.VisitorStartDate.Date && DateTime.Now < visitor.VisitorStartDate.Date.AddDays(1).AddHours(12))
                    {
                        getAllCustomVisitorDTOs.Add(customVisitor);
                    }
                }
            }

            return View(getAllCustomVisitorDTOs.ToList());
        }
        [HttpGet]
        public IActionResult AllCheckOutVisitors()
        {
            var visitors = _unitOfWork.visitorDal.GetAll(x => x.VisitorEndDate.Date <= DateTime.Now.Date && x.VisitorState != 5);

            List<AllVisitorDTO> getAllCustomVisitorDTOs = new List<AllVisitorDTO>();
            foreach (var visitor in visitors)
            {
                var customVisitor = _mapper.Map<AllVisitorDTO>(visitor);
                customVisitor.VisitorTotalVisitDay = (visitor.VisitorEndDate.Date - visitor.VisitorStartDate.Date).Days;
                customVisitor.VisitorLeftDays = (visitor.VisitorEndDate.Date - DateTime.Now.Date).Days <= 0 ? 0 : (visitor.VisitorEndDate.Date - DateTime.Now.Date).Days >= customVisitor.VisitorTotalVisitDay ? (visitor.VisitorEndDate.Date - visitor.VisitorStartDate.Date).Days : (visitor.VisitorEndDate.Date - DateTime.Now.Date).Days;
                customVisitor.VisitorState = 0;
                customVisitor.VisitorTotalPrice = visitor.VisitorRoomPrice * customVisitor.VisitorTotalVisitDay;
                getAllCustomVisitorDTOs.Add(customVisitor);
            }

            return View(getAllCustomVisitorDTOs.ToList());
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
    }
}
