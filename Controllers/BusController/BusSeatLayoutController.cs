using HulubejeBooking.Models.BusModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.CInemaModels;
using HulubejeBooking.Models.HotelModels;
using Tweetinvi.Models;
using HulubejeBooking.Models.Authentication;

namespace HulubejeBooking.Controllers.BusController
{
    public class BusSeatLayoutController : Controller
    {
        private readonly AuthenticationManager _authenticationManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;

        public BusSeatLayoutController(IHttpClientFactory httpClientFactory, AuthenticationManager authenticationManager, IHttpContextAccessor? httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationManager = authenticationManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> SeatLayout(string plateNumber, string terminal, string distance, string tariff, string level, string route, string operatorName,string routeSchedule, string originTerminalName, string via,
            string scheduleDate, string scheduleTime, string destinationCity, string depatureCity, string arrivalDate, string departureDate, string vehicleOperatorId , int vehicle, string destinationTermianl)
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
            var b = await _authenticationManager.identificationValid();
            ViewBag.isVaild = b.isValid;
            ViewBag.isLoggedIn = b.isLoggedIn;
            var loginInfo = HttpContext.Session.GetString("IsLogin");
            var login = "";
            if (loginInfo != null)
            {
                login = JsonConvert.DeserializeObject<string>(loginInfo);
            }
            HttpContext.Session.Remove("IsLogin");
            if (login != "Yes")
            {
                var param = new BusSeatLayout
                {
                    Vehicle = vehicle,
                    Distance = distance,
                    Level = level,
                    Route = route,
                    PlateNumber = plateNumber,
                    Terminal = terminal,
                    Tariff = tariff,
                    OperatorName = operatorName,
                    Date = scheduleDate,
                    Time = scheduleTime,
                    DestinationCity = destinationCity,
                    DepatureCity = depatureCity,
                    ArrivialDate = arrivalDate,
                    DepartureDate = departureDate,
                    VehicleOperatorId = vehicleOperatorId,
                    RouteScheduleId = routeSchedule,
                    DestinationTermianl=  destinationTermianl,
                    OriginTerminalName = originTerminalName,
                    Via = via,
                };
                var paramJson = JsonConvert.SerializeObject(param);
                HttpContext.Session.SetString("BusValues", paramJson);
                var identificationResult = await _authenticationManager.identificationValid();
                if (!identificationResult.isValid && !identificationResult.isLoggedIn)
                {
                    string validation = "Bus";
                    var validationJson = JsonConvert.SerializeObject(validation);
                    HttpContext.Session.SetString("SignInInformation", validationJson);
                    return RedirectToAction("Index", "SignIn");

                }

            }
            var seatValuesJson = HttpContext.Session.GetString("BusValues");
            HttpContext.Session.Remove("BusValues");
            if (seatValuesJson != null)
            {
                var seatValues = JsonConvert.DeserializeObject<BusSeatLayout>(seatValuesJson);
                plateNumber = seatValues.PlateNumber;
                terminal = seatValues.Terminal;
                level = seatValues.Level;
                route = seatValues.Route;
                tariff = seatValues.Tariff;
                operatorName = seatValues.OperatorName;
                scheduleDate = seatValues.Date;
                scheduleTime = seatValues.Time;
                destinationCity = seatValues.DestinationCity;
                depatureCity = seatValues.DepatureCity;
                distance = seatValues.Distance;
                vehicleOperatorId = seatValues.VehicleOperatorId;
                arrivalDate = seatValues.ArrivialDate;
                departureDate = seatValues.DepartureDate;
                routeSchedule = seatValues.RouteScheduleId;
                vehicle = (int)seatValues.Vehicle;
                via = seatValues.Via;
                originTerminalName = seatValues.OriginTerminalName;
                destinationTermianl = seatValues.DestinationTermianl;
            }


            var busSeatLayoutClient = _httpClientFactory.CreateClient("BusBooking");
            var schedueleInfo = new BusSeatLayout
            {
                Vehicle = vehicle,
                Distance = distance,
                Level = level,
                Route = route,
                PlateNumber = plateNumber,
                Terminal = terminal,
                Tariff = tariff,
                OperatorName = operatorName,
                Date = scheduleDate,
                Time = scheduleTime,
                DestinationCity = destinationCity,
                DepatureCity = depatureCity,
                ArrivialDate = arrivalDate,
                DepartureDate = departureDate,
                VehicleOperatorId = vehicleOperatorId,
                RouteScheduleId = routeSchedule,
                DestinationTermianl = destinationTermianl,
                OriginTerminalName = originTerminalName,
                Via = via,
            };  
            HttpResponseMessage response = await busSeatLayoutClient.GetAsync($"vehicles/getvehicleseatlayout?id={vehicle}");
            if (response.IsSuccessStatusCode) { 
               string resopnseData = await response.Content.ReadAsStringAsync();
               var seatLayout = JsonConvert.DeserializeObject<SeatLayoutStructure>(resopnseData);
                
               schedueleInfo.SeatLayout = seatLayout;
            }
            else
            {
                HttpResponseMessage defaultResponse = await busSeatLayoutClient.GetAsync($"vehicles/getvehicleseatlayout?id={239}");
                if (defaultResponse.IsSuccessStatusCode)
                {
                    string resopnseData = await defaultResponse.Content.ReadAsStringAsync();
                    var seatLayout = JsonConvert.DeserializeObject<SeatLayoutStructure>(resopnseData);

                    schedueleInfo.SeatLayout = seatLayout;
                }
            }

            return View(schedueleInfo);
        }
    }
}
