using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.BusModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HulubejeBooking.Controllers.BusController
{
    public class PassengerInfoController : Controller
    {
        private readonly AuthenticationManager _authenticationManager;
        private IHttpContextAccessor? _httpContextAccessor;
        public PassengerInfoController(IHttpContextAccessor httpContextAccessor, AuthenticationManager authenticationManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
        }
        public async Task<IActionResult> IndexAsync(int selectedButtonsCount, List<string> seatName, string plateNumber, string terminal, int distance, List<string> seatId, int vehicleOperatorId, int routeScheduleId, string destinationTermianl,
            string tariff, string level, string route, string operatorName, string scheduleDate, DateTime scheduleTime, string destinationCity, string depatureCity, DateTime arrivialDate, DateTime departureDate, string originTerminalName, string via)
        {
            var identificationResult = await _authenticationManager.identificationValid();
            if (identificationResult != null)
            {
                ViewBag.isVaild = identificationResult.isValid;
                ViewBag.isLoggedIn = identificationResult.isLoggedIn;
                ViewBag.FirstName = identificationResult?.UserData.FirstName;
                ViewBag.LastName = identificationResult?.UserData.LastName;
                ViewBag.MiddleName = identificationResult?.UserData.MiddleName;
                ViewBag.Personalattachment = identificationResult?.UserData.PersonalAttachment;
                ViewBag.Idnumber = identificationResult?.UserData.IdNumber;
                ViewBag.Idtype = identificationResult?.UserData.IdType;
                ViewBag.Dob = identificationResult?.UserData.Dob;
                ViewBag.Idattachment = identificationResult?.UserData.IdAttachment;
                ViewBag.PhoneNumber = identificationResult?.UserData.Code;
                ViewBag.EmailAddress = identificationResult?.UserData.Email;
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
