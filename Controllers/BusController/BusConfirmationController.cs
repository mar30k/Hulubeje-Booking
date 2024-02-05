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
    public class BusConfirmationController : Controller
    {
        private IHttpContextAccessor? _httpContextAccessor;

        public BusConfirmationController(IHttpContextAccessor? httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost]
        public IActionResult ConfirmationPage(List<PassengerModel> passengers, List <string> seatId,string tariff, string depatureCity, string destinationCity, string terminal, string operatorName,
            string date, string plateNumber, DateTime arrivalDate, DateTime departureDate, int vehicleOperatorId, int routeScheduleId)
        {
            var userDataCookie = _httpContextAccessor?.HttpContext?.Request.Cookies[CNET_WebConstants.IdentificationCookie];
            if (!string.IsNullOrEmpty(userDataCookie))
            {
                var user = JsonConvert.DeserializeObject<UserInformation>(userDataCookie);
                ViewBag.UserName = user?.firstName;
                ViewBag.LastName = user?.lastName;
                ViewBag.MiddleName = user?.middleName;
                ViewBag.Image = user?.personalattachment;
                ViewBag.SuccessCode = user?.successCode;
                ViewBag.Inumber = user?.idnumber;
                ViewBag.Idtype = user?.idtype;
                ViewBag.Dob = user?.dob;
                ViewBag.Idattachment = user?.idattachment;
                ViewBag.PhoneNumber = user?.phoneNumber;
                ViewBag.EmailAddress = user?.emailAddress;
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
            }
            var schedule = new VwRouteSchedule()
            {
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
