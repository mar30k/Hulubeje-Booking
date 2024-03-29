using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.BusModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Globalization;

namespace HulubejeBooking.Controllers.BusController
{
    public class ConfirmationController : Controller
    {
        private readonly AuthenticationManager _authenticationManager;
        private IHttpContextAccessor? _httpContextAccessor;

        public ConfirmationController(IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
        }
        [HttpPost]
        public async Task<IActionResult> IndexAsync(List<PassengerModel> passengers, List <string> seatId,string tariff, string depatureCity, string destinationCity, string terminal, string operatorName, int distance,
            string date, string plateNumber, DateTime arrivalDate, DateTime departureDate, int vehicleOperatorId, int routeScheduleId, string destinationTermianl, string originTerminalName, List<string> seatName, string via)
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
            string dateFormat = "ddd, MMM d, yyyy";
            DateTime dateTime = DateTime.ParseExact(date, dateFormat, CultureInfo.InvariantCulture);
            string formattedDate = dateTime.ToString("yyyy-MM-dd");
            string[] parts = tariff.Split(' ');
            decimal tarrifDecimal = decimal.Parse(parts[0]);
            if (passengers.Count == seatId.Count)
            {
                for (int i = 0; i < passengers.Count; i++)
                {
                    passengers[i].SeatId = seatId[i];
                }
                for (int i = 0; i < passengers.Count; i++)
                {
                    passengers[i].SeatName = seatName[i];
                }
            }
            var schedule = new VwRouteSchedule()
            {
                DestinationTerminalName = destinationTermianl,
                OriginTerminalName = originTerminalName,
                DestCityName = destinationCity,
                Tariff = tarrifDecimal,
                DepatureCity = depatureCity,
                DepartureDate = departureDate,
                OperatorName = operatorName,
                Date = formattedDate,
                Terminal = terminal,
                PlateNumber = plateNumber,
                ArrivalDate = arrivalDate,
                SeatId = seatId,
                VehicleOperatorId = vehicleOperatorId,
                Id = routeScheduleId,
                Distance = distance,
                ViaDescription = via
            };
            var wrap = new Wrap()
            {
                VwRouteScheduleData = schedule,
                PassengerModelData = passengers,    
            }; 
            return View(wrap);
        }
    }
}
