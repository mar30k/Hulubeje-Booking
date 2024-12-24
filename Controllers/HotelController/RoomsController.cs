using HulubejeBooking.Models.HotelModels;
using HulubejeBooking.Controllers.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace HulubejeBooking.Controllers.HotelController
{
    public class RoomsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationManager _authenticationManager;

        public RoomsController(
            IHttpClientFactory httpClientFactory,
            AuthenticationManager authenticationManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _authenticationManager = authenticationManager;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] RoomFormData roomFormData)
        {
            var dt = DateRangeParser.ParseDateRange(roomFormData.Date);
            var arrivalDateString = dt.startDateString.Trim();
            var departureDateString = dt.endDateString.Trim();
            DateTime arrivalDate = DateTime.ParseExact(arrivalDateString, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime departureDate = DateTime.ParseExact(departureDateString, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            roomFormData.ArrivalDate = arrivalDate;
            roomFormData.DepartureDate = departureDate;
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
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
                var roomFormDatajson = JsonConvert.SerializeObject(roomFormData);
                HttpContext.Session.SetString("RoomFormData", roomFormDatajson);
                if (identificationResult!=null && !identificationResult.isValid && !identificationResult.isLoggedIn)
                {
                    HttpContext.Session.SetString("SignInInformation", JsonConvert.SerializeObject("Hotel"));
                    TempData["ErrorMessage"] = "Please login to proceed further.";
                    return RedirectToAction("Index", "Signin");
                }
            }

            var roomFormDatajs = HttpContext.Session.GetString("RoomFormData");
            if (roomFormDatajs != null)
            {
                roomFormData = JsonConvert.DeserializeObject<RoomFormData>(roomFormDatajs) ?? new RoomFormData();
            }
            else
            {
                TempData["ErrorMessage"] = "Session Has Expired Please Restart the Booking Process";
                return RedirectToAction("Index", "Home");
            }



            var requestBody = new
            {
                arrivalDate,
                departureDate,
                companyCode = roomFormData.orgCode,
                roomFormData.adultCount,
                childCount = roomFormData.childrenCount,
                roomCount = roomFormData.roomsCount,
                orgOUD = roomFormData.oud,
                city = (string?)null
            };
            var roomBody = JsonConvert.SerializeObject(requestBody);
            var roomContent = new StringContent(roomBody, Encoding.UTF8, "application/json");
            string? token = identificationResult?.UserData?.Token;

            _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _v7Client.PostAsync("hotel/getrooms", roomContent);
            var availableRooms = new GetRooms();
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                availableRooms = data != null ? JsonConvert.DeserializeObject<GetRooms>(data) : new GetRooms();
                if (availableRooms != null)
                {
                    availableRooms.HotelName = roomFormData.Name;
                    availableRooms.RoomFormData = roomFormData;
                }
                return View(availableRooms);
            }
            else
            {
                if (availableRooms != null)
                {
                    availableRooms.HotelName = roomFormData.Name;
                    availableRooms.RoomFormData = roomFormData;
                }
                return View(availableRooms);
            }
        }

    }
    
}
