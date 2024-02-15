using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.BusModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HulubejeBooking.Controllers.BusController
{
    public class PassengerInfoController : Controller
    {
        private IHttpContextAccessor? _httpContextAccessor;
        public PassengerInfoController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index(int selectedButtonsCount, List<string> seatName, string plateNumber, string terminal, int distance, List<string> seatId, int vehicleOperatorId, int routeScheduleId, string destinationTermianl,
            string tariff, string level, string route, string operatorName, string scheduleDate, DateTime scheduleTime, string destinationCity, string depatureCity, DateTime arrivialDate, DateTime departureDate, string originTerminalName, string via)
        {
            var userDataCookie = _httpContextAccessor?.HttpContext?.Request.Cookies[CNET_WebConstants.IdentificationCookie];
            if (!string.IsNullOrEmpty(userDataCookie))
            {
                var user = JsonConvert.DeserializeObject<UserInformation>(userDataCookie);
                ViewBag.FirstName = user?.firstName;
                ViewBag.LastName = user?.lastName;
                ViewBag.MiddleName = user?.middleName;
                ViewBag.Personalattachment = user?.personalattachment;
                ViewBag.SuccessCode = user?.successCode;
                ViewBag.Idnumber = user?.idnumber;
                ViewBag.Idtype = user?.idtype;
                ViewBag.Dob = user?.dob;
                ViewBag.Idattachment = user?.idattachment;
                ViewBag.PhoneNumber = user?.phoneNumber;
                ViewBag.EmailAddress = user?.emailAddress;
            }
            var schedule = new VwRouteSchedule()
            {
                Id = routeScheduleId,
                PlateNumber = plateNumber,
                Terminal = terminal,
                Distance = distance,
                Amount = tariff,
                LevelDesc = level,
                OperatorName = operatorName,
                RouteDescription = route,
                Date = scheduleDate,
                Time = scheduleTime,
                DestCityName = destinationCity,
                DepatureCity = depatureCity,
                ArrivalDate = arrivialDate,
                DepartureDate = departureDate,
                SeatId = seatId,
                VehicleOperatorId = vehicleOperatorId,
                NoOfSeat = selectedButtonsCount,
                SeatName = seatName,
                DestinationTerminalName = destinationTermianl,
                OriginTerminalName = originTerminalName,
                ViaDescription = via,
            };
          return View(schedule);
        }
    }
}
