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
        public async Task<IActionResult> Index(string? plateNumber, string? terminal, string? distance, string? tariff, string? level, string? route, string? operatorName,string? routeSchedule, string? originTerminalName, string? via,
            string? scheduleDate, string? scheduleTime, string? destinationCity, string? depatureCity, string? arrivalDate, string? departureDate, string? vehicleOperatorId , int? vehicle, string? destinationTermianl, string? sheduleId)
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
                    DepartureCity = depatureCity,
                    ArrivialDate = arrivalDate,
                    DepartureDate = departureDate,
                    VehicleOperatorId = vehicleOperatorId,
                    RouteScheduleId = routeSchedule,
                    DestinationTermianl=  destinationTermianl,
                    OriginTerminalName = originTerminalName,
                    Via = via,
                    SheduleId = sheduleId
                };
                var paramJson = JsonConvert.SerializeObject(param);
                HttpContext.Session.SetString("BusValues", paramJson);
                if (!identificationResult.isValid && !identificationResult.isLoggedIn)
                {
                    string validation = "Bus";
                    var validationJson = JsonConvert.SerializeObject(validation);
                    HttpContext.Session.SetString("SignInInformation", validationJson);
                    TempData["ErrorMessage"] = "Please login to proceed further.";
                    return RedirectToAction("Index", "SignIn");

                }

            }
            var seatValuesJson = HttpContext.Session.GetString("BusValues");
            HttpContext.Session.Remove("BusValues");
            if (seatValuesJson != null)
            {
                var seatValues = JsonConvert.DeserializeObject<BusSeatLayout>(seatValuesJson);

                plateNumber = seatValues?.PlateNumber;
                terminal = seatValues?.Terminal;
                level = seatValues?.Level;
                route = seatValues?.Route;
                tariff = seatValues?.Tariff;
                operatorName = seatValues?.OperatorName;
                scheduleDate = seatValues?.Date;
                scheduleTime = seatValues?.Time;
                destinationCity = seatValues?.DestinationCity;
                depatureCity = seatValues?.DepartureCity;
                distance = seatValues?.Distance;
                vehicleOperatorId = seatValues?.VehicleOperatorId;
                arrivalDate = seatValues?.ArrivialDate;
                departureDate = seatValues?.DepartureDate;
                routeSchedule = seatValues?.RouteScheduleId;
                vehicle = (int?)seatValues?.Vehicle;
                via = seatValues?.Via;
                originTerminalName = seatValues?.OriginTerminalName;
                destinationTermianl = seatValues?.DestinationTermianl;
                sheduleId = seatValues?.SheduleId;
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
                DepartureCity = depatureCity,
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
            HttpResponseMessage soldSeatsRespnse = await busSeatLayoutClient.GetAsync($"routeschedule/getsoldseats?RouteSchedule={sheduleId}");
            if (soldSeatsRespnse.IsSuccessStatusCode)
            {
                string soldSeatsResponseData = await soldSeatsRespnse.Content.ReadAsStringAsync();
                List<int>? soldSeats = JsonConvert.DeserializeObject<List<int>>(soldSeatsResponseData);
                schedueleInfo.SoldSeats = soldSeats;
            }
            return View(schedueleInfo);
        }
    }
}
